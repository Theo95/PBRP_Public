using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using PBRP.Logs;

namespace PBRP
{
    public class ConnectionManager : Script
    {
        public ConnectionManager()
        {
            API.onPlayerConnected += OnPlayerConnected;
            API.onPlayerDisconnected += OnPlayerDisconnected;
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if(eventName == "ExecutePlayerLogout")
            {

                API.sendNativeToPlayer(sender, Hash._TRANSITION_TO_BLURRED, 2.0);
                API.triggerClientEvent(sender, "account_prompt_login");
                if (sender.isInVehicle)
                    sender.warpOutOfVehicle();


                sender.ToggleCursorLock(true);
                DisconnectPlayer(sender);

                sender.position = new Vector3(0, 0, 0);
                sender.transparency = 0;
                sender.freeze(true);
                sender.dimension = Globals.GetPlayerID(sender) + 1;
            }
        }

        private void OnPlayerConnected(Client player)
        {
            for(int i = 0; i < API.getMaxPlayers(); i++)
            {
                if (Player.IDs[i] == null)
                {
                    Player.IDs[i] = player;
                    player.dimension = i + 1;
                    break;
                }
            }

            player.transparency = 0;
            player.freeze(true);
            
        }

        private void OnPlayerDisconnected(Client player, string reason)
        {
            Console.WriteLine("{0} has disconnected from the server. Reason: {1}", player.name, reason);
            for (int i = 0; i < API.getMaxPlayers(); i++)
            {
                if (Player.IDs[i].name == player.name)
                {
                    try
                    {
                        DisconnectPlayer(Player.IDs[i]);
                    }
                    catch
                    {
                        Player.PlayerData.Remove(Player.IDs[i]);
                        Master.MasterData.Remove(Master.MasterData.First(m => m.Client.name == player.name));
                    }
                    Player.IDs[i] = null;
                    return;
                }
            }
            Player.PlayerData.Remove(player);
            Master.MasterData.Remove(Master.MasterData.First(m => m.Client.name == player.name));
        }

        public void DisconnectPlayer(Client player)
        {
            Player user = null;

            try
            {
                user = Player.PlayerData[player];
            }
            catch
            {
                user = Player.PlayerData.Values.FirstOrDefault(i => i.Username.Roleplay() == player.name);
            }

            if (user != null)
            {
                if (user.IsLogged)
                {
                    user.IsLogged = false;
                    user.LastPosition = player.position;
                    user.LastRotation = player.rotation;
                    user.Health = player.health;
                    user.Armour = player.armor;
                    user.Dimension = player.dimension;

                    user.ChatIndicatorLabel.text = "";
                    API.deleteEntity(user.ChatIndicatorLabel);

                    user.WeaponSkillData = string.Join(",", user.WeaponSkill.Values);

                    user.SavePlayerVehicles();

                    if (user.InEvent == PlayerEvent.VehicleDealership)
                    {
                        user.LastPosition = new Vector3(-257.5197 + (new Random().Next(-50, 50) / 20), 6211.149 + (new Random().Next(-50, 50) / 20), z: 31.48923);
                        user.LastRotZ = 121.6988;
                    }
                    Vehicle ve = Vehicle.VehicleData.Values.FirstOrDefault(v => v.IsDealerVehicle && v.OwnerId == user.Id);

                    if (ve != null)
                    {
                        API.deleteEntity(ve.DealershipEmployee);
                        ve.Entity.Delete();
                    }

                    if(user.SelectedCash != null) InventoryRepository.UpdateAsync(user.SelectedCash);
                    if (user.SelectedCardAccount != null) BankRepository.UpdateAsync(user.SelectedCardAccount);

                    user.MasterAccount.ActiveConnectionLog.DisconnectTime = Server.Date;
                    ConnectionLogRepository.UpdateAsync(user.MasterAccount.ActiveConnectionLog);
                    WeaponRepository.UpdateAllAsync(user.Weapons);
                    PlayerRepository.UpdateAsync(user);
                    Player.PlayerData.Remove(user.Client);
                }
                else Player.PlayerData.Remove(user.Client);
                Master.MasterData.Remove(user.MasterAccount);

            }
            player.FadeIn(0);
            API.triggerClientEvent(player, "onPlayerDisconnect");
        }
    }
}
