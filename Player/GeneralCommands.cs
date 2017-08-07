using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Managers;
using PBRP.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PBRP
{
    public class GeneralCommands : Script
    {
        public GeneralCommands()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
            API.onChatCommand += API_onChatCommand;
        }

        private void API_onChatCommand(Client sender, string command, CancelEventArgs cancel)
        {
            try
            {
                Console.WriteLine(command);
                Player p = Player.PlayerData[sender];
                if(p.IsLogged)
                {
                    CommandLogRepository.AddNew(new CommandLog(p.MasterId, p.Id, command));
                }
            }
            catch(Exception)
            {

            }
        }

        public static GeneralCommands shared = new GeneralCommands();

        private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "playerPickupWeaponExists")
            {
                if((int)arguments[0] == 1)
                    PickUpItem(sender, true);
            }
        }

        [Command("id", "/id [Part of name]")]
        public void GetPlayerIdFromName(Client sender, string partOfName)
        {
            int count = 0;
            foreach (Player p in Player.PlayerData.Values)
            {
                if (p.Username.Contains(partOfName, StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                    API.sendChatMessageToPlayer(sender, String.Format("ID: {0} - {1}", Globals.GetPlayerID(p.Client), p.Username.Roleplay()));
                }
            }
            if (count == 0) API.sendNotificationToPlayer(sender, "~r~No players found");
        }

        [Command("q")]
        public void QuitServer(Client player)
        {
            API.kickPlayer(player);
        }

        [Command("vehid")]
        public void GetVehiclesID(Client player)
        {
            GrandTheftMultiplayer.Server.Elements.Vehicle veh = Globals.GetClosestVehicle(player, 4);
            if (veh != null) Message.Info(player, "Vehicle ID: " + Globals.GetVehicleID(veh));
            else API.SendWarningNotification(player, "No vehicle in your proximity.");
        }

        [Command("me", GreedyArg = true)] 
        public void MeCommand(Client sender, string message)
        {
            SendCloseMessage(sender, 15.0f, "~#C2A2DA~", API.getPlayerName(sender) + " " + message);
        }
        [Command("do", GreedyArg = true)]
        public void DoCommand(Client sender, string message)
        {
            SendCloseMessage(sender, 15.0f, "~#C2A2DA~", "(( " + message + ". )) " + API.getPlayerName(sender));
        }
        [Command("b", GreedyArg = true)] 
        public void OOCCommand(Client sender, string message)
        {
            SendCloseMessage(sender, 15.0f, "~#009FFB~", API.getPlayerName(sender) + ": " + "(( " + message + " ))");
        }
        [Command("s", Alias = "shout", GreedyArg = true)] 
        public void ShoutCommand(Client sender, string message)
        {
            SendCloseMessage(sender, 25.0f, "~#ffffff~", API.getPlayerName(sender) + " shouts: " + message);
        }
        [Command("w", Alias = "whisper", GreedyArg = true)] 
        public void WhisperCommand(Client sender, string message)
        {
            SendCloseMessage(sender, 7.5f, "~#ffffff~", API.getPlayerName(sender) + " whispers: " + message);
        }

        [Command("pm", GreedyArg = true)]
        public void PmCommand(Client sender, string target, string message)
        {
            Player trgt = Player.GetPlayerData(target);
                
            if(trgt == null) { API.SendErrorNotification(sender, "The player you attempted to PM isn't connected"); return; }
            if(trgt.Client == sender) { API.SendErrorNotification(sender, "You can't send a PM to yourself"); return; }
  
            API.sendChatMessageToPlayer(sender, String.Format("[PM to {0}] {1}",
                trgt.Client.name, message));
            API.sendChatMessageToPlayer(trgt.Client, String.Format("[PM from {0}] {1}",
                sender.name, message));
        }

        [Command("drop")]
        public void DropPlayerItem(Client user, string option = null, string value = null)
        {
            Player player = Player.PlayerData[user];
            if(option == null)
            {
                return;
            }
            switch(option)
            {
                case "weapon":
                    DropPlayerWeapon(player);
                    break;
            }
        }

        private void DropPlayerWeapon(Player player)
        {
            if (player.Client.currentWeapon != 0)
            {
                string weaponName = NameToHash.Weapons.Keys.First(k => NameToHash.Weapons[k] == player.Client.currentWeapon);
                Weapon weap = player.Weapons.Single(w => w.Model.GetHashCode() == player.Client.currentWeapon.GetHashCode());

                Vector3 pos = Player.GetPositionInFrontOfPlayer(player.Client, 1f, -0.5f);
                Vector3 rot = new Vector3(90, 0, 0);

                WeaponLogRepository.AddNew(new WeaponLog(weap.CurrentOwnerId, weap.Id, weap.Type, -1));

                weap.DroppedAt = Server.Date;
                weap.DroppedPos = pos;
                weap.DroppedRot = rot;
                weap.DroppedDimension = player.Client.dimension;
                weap.CurrentOwnerId = -1;
                weap.DroppedObj = API.createObject(NameToHash.WeaponObjects[weaponName], pos, rot, weap.DroppedDimension);
                weap.ApplyPhysics();

                Weapon.DroppedWeapons.Add(weap);
                API.removePlayerWeapon(player.Client, (WeaponHash)weap.Model);
                player.Weapons.Remove(weap);
                
                WeaponRepository.UpdateAsync(weap);
            }
            else API.SendErrorNotification(player.Client, "You can only drop the weapon you are holding.");
        }

        [Command("pickupitem")]
        public void PickUpItem(Client user, bool forcePickup = false)
        {
            Player player = Player.PlayerData[user];
            foreach (Weapon w in Weapon.DroppedWeapons)
            {
                if (w.CurrentOwnerId != -1) continue;
                if (!(w.DroppedPos.DistanceTo(user.position) < 1.5f)) continue;
                API.deleteEntity(w.DroppedObj);
                if (player.Weapons.Any(we => we.Model == w.Model))
                {
                    if (!forcePickup)
                    {
                        API.ShowPopupPrompt(user, "playerPickupWeaponExists", "Existing weapon in inventory",
                            "By picking up this weapon, the ammunition in the weapon will be added to your current weapon but this weapon will be destroyed.");
                        return;
                    }
                    else
                    {
                        int index = player.Weapons.IndexOf(player.Weapons.First(we => we.Model == w.Model));
                        player.Weapons[index].Ammo += w.Ammo;
                        WeaponRepository.UpdateAsync(player.Weapons[index]);
                        WeaponRepository.RemoveAsync(w);
                    }
                }
                else
                {
                    w.CurrentOwnerId = player.Id;
                    player.Weapons.Add(w);
                    WeaponRepository.UpdateAsync(w);
                }
                WeaponLogRepository.AddNew(new WeaponLog(-1, w.Id, w.Type, player.Id));

                API.givePlayerWeapon(user, w.Model, w.Ammo, true, true);
                API.setPlayerWeaponAmmo(user, w.Model, w.Ammo);
                Weapon.DroppedWeapons.Remove(w);
                return;
            }
        }

        [Command("inventory")]
        public void ShowInventory(Client player)
        {
            Player p = Player.PlayerData[player];
            InventoryManager.UpdatePlayerInventory(p);
        }

        [Command("give", GreedyArg = true)]
        public void GiveItemToPlayer(Client player, string partOfName = null, string option = null, string value = null)
        {
            Player user = Player.PlayerData[player];
            var target = Player.GetPlayerData(partOfName);
            if (target == null) { Message.PlayerNotConnected(player); return; }

            if (option == null)
            {
                Message.Info(player, "USAGE: /give [Part of Name] [Option] [Value]");
                Message.Info(player, "Options: weapon, key");
                return;
            }

            switch(option)
            {
                case "weapon":
                    GivePlayerWeapon(user, target, value);
                    break;
            }
        }
        private void GivePlayerWeapon(Player player, Player target, string value)
        {
            try
            {
                string weaponName = NameToHash.Weapons.Keys.First(k => k.Contains(value));
                WeaponHash weapon = NameToHash.Weapons[weaponName];
                List<Weapon> playerWeapons = player.Weapons;

                foreach (Weapon w in playerWeapons)
                {
                    if (w.Model.GetHashCode() == weapon.GetHashCode())
                    {
                        int ammo = API.getPlayerWeaponAmmo(player.Client, weapon);
                        try
                        {
                            if (target.Weapons.Single(s => s.Model == w.Model) != null)
                            {
                                int targetAmmo = API.getPlayerWeaponAmmo(target.Client, weapon);

                                targetAmmo += ammo;
                                API.removePlayerWeapon(player.Client, weapon);
                                API.removePlayerWeapon(target.Client, weapon);
                                API.givePlayerWeapon(target.Client, weapon, targetAmmo, true, true);
                            }
                        }
                        catch
                        {
                            API.removePlayerWeapon(player.Client, weapon);
                            API.givePlayerWeapon(target.Client, weapon, ammo, true, true);
                            API.sendChatMessageToPlayer(player.Client, String.Format("You have given {0} a {1} with {2} ammo", target.Client.name, weaponName, ammo));
                            API.sendChatMessageToPlayer(target.Client, String.Format("You were given a {0} with {1} ammo from {2}", weaponName, ammo, target.Client.name));
                        }
                        break;
                    }
                }
            }
            catch
            {
                API.sendChatMessageToPlayer(player.Client, "issue mate");
            }
        }

        [Command("driver")]
        public void GetDriverData(Client player)
        {
            Console.WriteLine(player.Ped());
            Console.WriteLine(player.vehicle.GetDriver());
        }

        public void AcceptFactionInvite(Client player)
        {
            Player requestBy = Player.PlayerData[player].AwaitingFactionInvite;
            if (requestBy != null)
            {
                Player.PlayerData[player].Faction = requestBy.Faction;
                Player.PlayerData[player].FactionId = requestBy.FactionId;
                Player.PlayerData[player].FactionRank = 1;

                API.sendChatMessageToPlayer(requestBy.Client, String.Format("{0} has accepted your invitation into the {1}", player.name, requestBy.Faction.Name));
                API.sendChatMessageToPlayer(player, String.Format("~y~You have successfully joined the {0}.", requestBy.Faction.Name));

                API.SendMessageToAllFactionMemebers(requestBy.Faction, String.Format("~y~{0} has joined the {1}", player.name, requestBy.Faction));

                Player.PlayerData[player].AwaitingFactionInvite = null;

                PlayerRepository.UpdateAsync(Player.PlayerData[player]);
            }
        }

        public static void SendCloseMessage(Client player, float radius, string sender, string msg)
        {
            List<Client> nearPlayers = API.shared.getPlayersInRadiusOfPlayer(radius, player);
            foreach (Client target in nearPlayers)
            {
                API.shared.sendChatMessageToPlayer(target, sender, msg);
            }
        }

        [Command("savepos", GreedyArg = true)]
        public void SavePosition(Client player, string name)
        {
            if(player.isInVehicle)
            {
                File.AppendAllText("savedpos.txt", String.Format("{0}, {1}, {2} : {3}, {4}, {5} //{6}\n", player.vehicle.position.X, player.vehicle.position.Y, player.vehicle.position.Z,
                    player.vehicle.rotation.X, player.vehicle.rotation.Y, player.vehicle.rotation.Z, name));
                API.SendInfoNotification(player, "Vehicle Position saved as: " + name);
            }
            else
            {
                File.AppendAllText("savedpos.txt", String.Format("{0}, {1}, {2} : {3}, {4}, {5} //{6}\n", player.position.X, player.position.Y, player.position.Z,
                       player.rotation.X, player.rotation.Y, player.rotation.Z, name));
                API.SendInfoNotification(player, "Player Position saved as: " + name);
            }
        }
    }
}
