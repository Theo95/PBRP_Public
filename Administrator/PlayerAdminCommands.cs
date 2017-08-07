using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace PBRP
{
    public class PlayerAdminCommands : Script
    {

        public PlayerAdminCommands()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "banReasonSubmitted" && (int)arguments[0] == 1)
            {
                Player player = Player.PlayerData[sender];
                if (player.CurrentBanData != null)
                {
                    player.CurrentBanData.BanReason = arguments[1].ToString();
                    BanRepository.AddNewBan(player.CurrentBanData);

                    Player playerBeingBanned = player.CurrentBanData.PlayerBeingBanned;

                    if (player.CurrentBanData.BannedUntil == DateTime.MaxValue)
                    {
                        API.sendChatMessageToPlayer(playerBeingBanned.Client, "~r~You have been permanently banned from Paleto Bay Roleplay by Admin {0}.", sender.name);
                        API.SendErrorNotification(sender,
                            string.Format("{0} has been permbanned.", playerBeingBanned.Client.name));
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(playerBeingBanned.Client,
                            string.Format("~r~You have been temporarily banned from Paleto Bay Roleplay until {0:dd/MM/yy hh:mm tt}.",
                                player.CurrentBanData.BannedUntil));
                        API.SendErrorNotification(sender,
                            string.Format("{0} has been tempbanned until {1:dd/MM/yy hh:mm tt}.",
                                playerBeingBanned.Client.name, player.CurrentBanData.BannedUntil));
                    }

                    API.kickPlayer(playerBeingBanned.Client, "Banned");

                    player.CurrentBanData = null;
                }
            }
        }

        [Command("kick", "")]
        public void KickPlayer(Client sender, string partofName)
        {
            if (Master.MasterData.Single(m => m.Client == sender).AdminLevel < 1) { Message.NotAuthorised(sender); }

        }

        [Command("bann", GreedyArg = true, ACLRequired = false)]
        public void BanPlayer(Client player, string partOfName = "", string duration = "")
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 2) { Message.NotAuthorised(player); return; }
            if(partOfName == "")
            {
                API.sendChatMessageToPlayer(player, "~o~USAGE:~w~ /ban [Part of name / ID] [Duration]");
                API.sendChatMessageToPlayer(player, "Example usages: /ban 0 permanent | /ban 0 2 years | /ban 0 2 days and 1 month");
                return;
            }
            var target = Player.GetPlayerData(partOfName);
            if (target == null) { Message.PlayerNotConnected(player); return; }

            DateTime bannedUntil = Server.Date;
            bool isPermanent = false;
            string[] durationParam = duration.Split(' ');

            for(int i = 0; i < durationParam.Length; i++)
            {
                try
                {
                    switch (durationParam[i])
                    {
                        case "permanent":
                            isPermanent = true;
                            break;
                        case "minute":
                        case "minutes":
                            bannedUntil = bannedUntil.AddMinutes(int.Parse(durationParam[i - 1]));
                            break;
                        case "day":
                        case "days":
                            bannedUntil = bannedUntil.AddDays(int.Parse(durationParam[i - 1]));
                            break;
                        case "weeks":
                        case "week":
                            bannedUntil = bannedUntil.AddDays(int.Parse(durationParam[i - 1]) * 7);
                            break;
                        case "month":
                        case "months":
                            bannedUntil = bannedUntil.AddMonths(int.Parse(durationParam[i - 1]));
                            break;
                        case "year":
                        case "years":
                            bannedUntil = bannedUntil.AddYears(int.Parse(durationParam[i - 1]));
                            break;
                    }
                }
                catch { API.sendChatMessageToPlayer(player,
                    string.Format("~r~ERROR: ~w~ Invalid date offset {0} {1}", durationParam[i - 1], durationParam[i])); return; }
            }

            user.CurrentBanData = new Ban
            {
                BannedId = target.MasterAccount.Id,
                BannedPlayer = target.Id,
                BannedBy = user.MasterAccount.Id,
                BannedByPlayer = user.Id,
                BannedUntil = isPermanent ? DateTime.MaxValue : bannedUntil,
                PlayerBeingBanned = target

            };
            API.triggerClientEvent(player, "confirmInput", "banReasonSubmitted", "Ban Reason", "Please enter the reason for banning " + target.Client.name + ".");
        }

        [Command("unban", GreedyArg = true)]
        public void UnbanPlayer(Client player, string fullPlayerName, string reason)
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 3) { Message.NotAuthorised(player); return; }

            try
            {
                Ban banData = BanRepository.GetActiveBanByPlayerAccount(fullPlayerName);
                Master adminWhoBanned = MasterRepository.GetMasterDataById(banData.BannedBy);

                if (user.MasterAccount.AdminLevel > adminWhoBanned.AdminLevel || user.MasterId == adminWhoBanned.Id)
                {
                    BanRepository.RemoveAsync(banData);

                    API.sendChatMessageToPlayer(player,
                        string.Format("You have successfully unbanned {0}",
                            PlayerRepository.GetPlayerDataByName(fullPlayerName).Username.Roleplay()));
                }
                else { API.SendWarningNotification(player, "You aren't a higher enough administrative level to unban this player."); }
            }
            catch { API.SendErrorNotification(player, "The specified player doesn't exist."); }
        }

        [Command("set", "", AddToHelpmanager = false)]
        public void SetPlayerValue(Client player, string partOfName= "", string option = "", string value = "")
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 2) { Message.NotAuthorised(player); return; }
            if(option == "" || partOfName == "")
            {
                Message.Info(player, "USAGE: /set [Part of Name] [Option] [Value]");
                Message.Info(player, "Options: dimension, skin, faction, rank, health, armor");
                return;
            }
            if(partOfName == "weather")
            {
                try
                {
                    SetWeather(player, int.Parse(option));
                    return;
                }
                catch { API.SendErrorNotification(player, "Invalid weather ID"); return; }
                
            }
            if(partOfName == "time")
            {
                try
                {
                    int time = int.Parse(option);
                    if (time > 0 && time < 24)
                    {
                        API.setTime(time, 0);
                    }
                    return;
                }
                catch { API.SendErrorNotification(player, "Invalid weather ID"); return; }
                
            }
            if(partOfName == "fuel")
            {
                try
                {
                    if(player.isInVehicle)
                    {
                        Vehicle veh = Vehicle.VehicleData[player.vehicle];

                        veh.Fuel = int.Parse(option);

                        API.SendInfoNotification(player,
                            string.Format("You have set the fuel of this {0} to {1}", player.vehicle.displayName,
                                option));
                        return;
                    }
                }
                catch { API.SendErrorNotification(player, "Invalid fuel value"); return; }
            }
            else if(partOfName == "rain")
            {
                SetRainIntensity(player, float.Parse(option));
                return;
            }
            else if (partOfName == "clouds")
            {
                SetCloudy(player, option);
                return;
            }
            else if(partOfName == "windspeed")
            {
                SetWindSpeed(player, float.Parse(option));
                return;
            }
            var target = Player.GetPlayerData(partOfName);
            if(target == null) { Message.PlayerNotConnected(player); return; }
            switch(option)
            {
                case "skin":
                    SetPlayerSkin(user, target, value);
                    break;
                case "faction":
                    SetPlayerFaction(user, target, value);
                    break;
                case "rank":
                    SetPlayerFactionRank(user, target, value);
                    break;
                case "health":
                    SetPlayerHealth(user, target, value);
                    break;
                case "armour":
                case "armor":
                    SetPlayerArmour(user, target, value);
                    break;
                case "dimension":
                    SetPlayerDimension(user, target, value);
                    break;
            }
           PlayerRepository.UpdateAsync(target);
        }

        private void SetPlayerDimension(Player user, Player target, string value)
        {
            try
            {
                target.Client.dimension = int.Parse(value);
                target.Dimension = int.Parse(value);

                API.SendAdminNotification(target.Client, String.Format("Admin {0} has set your dimension to {1}", user.Client.name, value));
            }
            catch
            {
                API.SendErrorNotification(user.Client, "Invalid dimension value.");
            }
        }

        private void SetWeather(Client user, int weather)
        {
            if(weather > 0 && weather < 13)
            {
                API.setWeather(weather);
                Weather we = Weather.WeatherData.FirstOrDefault(w => w.Hour == Server.Date.Hour);
                if (we != null)
                {
                    we.WeatherType = weather;
                    switch(weather)
                    {
                        case 0: we.Description = "Sunny"; we.TempC = 25; break;
                        case 1: we.Description = "Clear"; we.TempC = 22; break;
                        case 2: we.Description = "Cloudy"; we.TempC = 19; break;
                        case 3: we.Description = "Haze"; we.TempC = 19; break;
                        case 4: we.Description = "Foggy"; we.TempC = 14; break;
                        case 5: we.Description = "Overcast"; we.TempC = 11; break;
                        case 6: we.Description = "Rain"; we.TempC = 9; break;
                        case 7: we.Description = "Thunderstorm"; we.TempC = 8; break;
                        case 8: we.Description = "Light Rain"; we.TempC = 10; break;
                        case 9: we.Description = "Unknown"; we.TempC = 18; break;
                        case 10: we.Description = "Light Snow"; we.TempC = -2; break;
                        case 11: we.Description = "Snow & Wind"; we.TempC = -3; break;
                        case 12: we.Description = "Snow"; we.TempC = -5; break;
                    }
                }
            }
            else if(weather == 14)
            {
                //Weather.WeatherType = 12;
                //Weather.RainLevel = 0;
                //Weather.WindSpeed = 0;

                //Weather.SnowActive = true;
                //Weather.DeepPedTracks = true;
                //Weather.DeepVehicleTracks = true;

                API.triggerClientEventForAll("activateSnow", true, true, true);
                API.setWeather(12);
            }
        }


        private void SetRainIntensity(Client user, float intensity)
        {
            API.sendNativeToAllPlayers(Hash._SET_RAIN_FX_INTENSITY, intensity);
        }

        private void SetWindSpeed(Client user, float windSpeed)
        {
            API.sendNativeToAllPlayers(Hash.SET_WIND_SPEED, windSpeed);
        }

        private void SetCloudy(Client user, string opacity)
        {
            API.sendNativeToAllPlayers(Hash._SET_CLOUD_HAT_TRANSITION, opacity, 1);
        }

        private void SetPlayerArmour(Player user, Player target, string value)
        {
            try { target.Armour = int.Parse(value); API.setPlayerArmor(target.Client, (int)target.Armour); }
            catch { API.SendErrorNotification(user.Client, "Invalid armor value."); }
        }

        private void SetPlayerHealth(Player user, Player target, string value)
        {
            try {
                target.Health = int.Parse(value);
                API.setPlayerHealth(target.Client, (int)target.Health);
                target.LeaveInjuredState();

                API.SendAdminNotification(target.Client, String.Format("Admin {0} has set your health to {1}", user.Client.name, value));
            }
            catch{ API.SendErrorNotification(user.Client, "Invalid health value."); }
        }

        [Command("testparticles")]
        public void TestParticle(Client user, string lib, string particle, float scale, float rx = 0, float ry = 0, float rz = 0)
        {
            API.createParticleEffectOnPosition(lib, particle, user.position, new Vector3(rx, ry, rz), scale, user.dimension);
        }

        [Command("goto")]
        public void GoToPlayer(Client player, string partOfName)
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 2) { Message.NotAuthorised(player); return; }
            var target = Player.GetPlayerData(partOfName);
            if (target == null) { Message.PlayerNotConnected(player); return; }

            API.createParticleEffectOnPosition("scr_rcbarry1", "scr_alien_teleport", player.position, new Vector3(), 1.5f, player.dimension);
            player.movePosition(target.Client.position.Add(new Vector3(1, 1, 0)), 200);
            player.dimension = target.Client.dimension;
            API.createParticleEffectOnPosition("scr_rcbarry1", "scr_alien_teleport", player.position, new Vector3(), 1.5f, player.dimension);
            API.createParticleEffectOnPosition("scr_indep_fireworks", "scr_indep_firework_shotburst", player.position, new Vector3(), 0.2f, player.dimension);

            Message.AdminMessage(target.Client, String.Format("Admin {0} has teleported to your position.", player.name));
            API.SendInfoNotification(player, String.Format("You have teleported to {0}", target.Client.name));
        }

        [Command("testanim")]
        public void TestAnimation(Client player, string dict, string animname)
        {
            API.playPlayerAnimation(player, 1, dict, animname);
        }

        [Command("xyz")]
        public void GotoCoord(Client player, float x, float y, float z)
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 5) { Message.NotAuthorised(player); return; }

            player.position = new Vector3(x, y, z);

            Message.AdminMessage(player, "You have teleported to position: X:" + x + " Y:" + y + " Z:" + z);
        }

        [Command("get")]
        public void GetPlayer(Client player, string partOfName)
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 2) { Message.NotAuthorised(player); return; }
            var target = Player.GetPlayerData(partOfName);
            if (target == null) { Message.PlayerNotConnected(player); return; }

            if (target.Client.isInVehicle && target.Client.vehicleSeat == -1)
            {
                NetHandle veh = target.Client.vehicle;
                API.createParticleEffectOnPosition("scr_rcbarry1", "scr_alien_teleport", target.Client.vehicle.position, new Vector3(), 1f, target.Client.vehicle.dimension);
                target.Client.vehicle.position = player.position.Add(new Vector3(2, 2, 0));
                target.Client.vehicle.dimension = player.dimension;
                API.setPlayerIntoVehicle(target.Client, veh, 1);
                target.Client.dimension = player.dimension;
            }
            else
            {
                target.Client.position = player.position.Add(new Vector3(1, 1, 0));
                target.Client.dimension = player.dimension;
            }

            Message.AdminMessage(target.Client, String.Format("Admin {0} has teleported you to their position.", player.name));
            API.sendNotificationToPlayer(player, String.Format("You have teleported {0} to you.", target.Client.name));
        }

        [Command("slap")]
        public void SlapPlayer(Client player, string partOfName)
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 1) { Message.NotAuthorised(player); return; }
            var target = Player.GetPlayerData(partOfName);
            if (target == null) { Message.PlayerNotConnected(player); return; }

            if (target.Client.isInVehicle)
            {
                GrandTheftMultiplayer.Server.Elements.Vehicle veh = target.Client.vehicle;
                veh.position = veh.position.Add(new Vector3(0, 0, 9));
            }
            else
            {
                API.setPlayerVelocity(target.Client, new Vector3(0, 0, 10));
            }

            Message.AdminMessage(target.Client, String.Format("Admin {0} has slapped you.", player.name));
            API.sendNotificationToPlayer(player, String.Format("You have slapped {0}.", target.Client.name));
        }

        [Command("spec", Alias = "spectate")]
        public void SpectatePlayer(Client player, string partOfName = "")
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 1) { Message.NotAuthorised(player); return; }
            var target = Player.GetPlayerData(partOfName);
            if(target == user) { API.sendNotificationToPlayer(player, "You can't spectate yourself"); return; }

            if(!player.spectating && target != null)
            {
                API.setPlayerToSpectatePlayer(player, target.Client);
            }
            else if(player.spectating)
            {
                player.stopSpectating();
            }
            else if (target == null) { Message.PlayerNotConnected(player);
            }
        }

        [Command("freeze")]
        public void FreezePlayer(Client player, string partOfName)
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 1) { Message.NotAuthorised(player); return; }
            var target = Player.GetPlayerData(partOfName);
            if (target == null) { Message.PlayerNotConnected(player); return; }

            API.freezePlayer(target.Client, true);
            Message.AdminMessage(target.Client, String.Format("Admin {0} has frozen you.", player.name));
            API.sendNotificationToPlayer(player, String.Format("You have frozen {0}", target.Client.name));
        }

        [Command("checklicenses")]
        public void CheckPlayerLicenses(Client player, int playerID)
        {
            List<License> licenses = LicenseRepository.GetPlayerLicenses(playerID);
            foreach(License license in licenses)
            {
                API.sendChatMessageToPlayer(player, license.Type.ToString());
            }
        }

        [Command("unfreeze")]
        public void UnfreezePlayer(Client player, string partOfName)
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 1) { Message.NotAuthorised(player); return; }
            var target = Player.GetPlayerData(partOfName);
            if (target == null) { Message.PlayerNotConnected(player); return; }

            API.freezePlayer(target.Client, false);
            Message.AdminMessage(target.Client, String.Format("Admin {0} has unfrozen you.", player.name));
            API.sendNotificationToPlayer(player, String.Format("You have unfrozen {0}", target.Client.name));
        }

        private void SetPlayerFactionRank(Player user, Player target, string value)
        {
            int id;
            if (user.MasterAccount.AdminLevel < 3) { Message.NotAuthorised(user.Client); return; }
            try { id = int.Parse(value); } catch { API.sendChatMessageToPlayer(user.Client, "~r~ERROR: ~w~Invalid rank."); return; }
            if(user.FactionId <= 0) { API.sendChatMessageToPlayer(user.Client, "~r~ERROR: ~w~That player is not in a faction."); return; }

            try
            {
                Rank rank = target.Faction.Ranks[id - 1];
                target.FactionRank = id;

                API.sendNotificationToPlayer(user.Client, String.Format("You have successfully set {0}'s rank to the {1}", target.Client.name, rank.Title));
                API.sendNotificationToPlayer(target.Client, String.Format("Admin {0} has set your faction to {1}", user.Client.name, rank.Title));
            }
            catch { API.sendChatMessageToPlayer(user.Client, "~r~ERROR: ~w~The entered rank does not exist.");
            }
        }

        private void SetPlayerFaction(Player player, Player target, string value, int rank = -1)
        {
            int id;
            Faction faction;
            if (player.MasterAccount.AdminLevel < 3) { Message.NotAuthorised(player.Client); return; }
            try { id = int.Parse(value); } catch { API.sendChatMessageToPlayer(player.Client, "~r~ERROR: ~w~Invalid faction ID."); return; }
            try { faction = Faction.FactionData.Single(f => f.Id == id); }
                catch { API.sendChatMessageToPlayer(player.Client, "~r~ERROR: ~w~Invalid faction ID."); return; }

            target.FactionId = id;
            target.Faction = faction;
            target.FactionRank = 1;
            
            API.sendNotificationToPlayer(player.Client, String.Format("You have successfully set {0}'s faction to the {1}", target.Client.name, faction.Name));
            API.sendNotificationToPlayer(target.Client, String.Format("Admin {0} has set your faction to {1}", player.Client.name, faction.Name));

            PlayerRepository.UpdateAsync(target);
        }

        public void SetPlayerSkin(Player user, Player target, string skinName)
        {
            if (user.MasterAccount.AdminLevel < 3) { Message.NotAuthorised(user.Client); return; }
            if (!Enum.IsDefined(typeof(PedHash), skinName))
            {
                API.sendChatMessageToPlayer(user.Client, "Skin doesn't exist");
                return;
            }

            target.Skin = (int)API.pedNameToModel(skinName);
            API.setPlayerSkin(target.Client, API.pedNameToModel(skinName));
            API.sendNotificationToPlayer(user.Client, String.Format("You have successfully set {0}'s skin", target.Client.name));
            API.sendNotificationToPlayer(target.Client, String.Format("Admin {0} has changed your skin", user.Client.name));

            PlayerRepository.UpdateAsync(target);
          
        }

        [Command("crash")]
        public void CrashPlayer(Client player, string partOfName)
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 5) { Message.NotAuthorised(user.Client); return; }
            var target = Player.GetPlayerData(partOfName);
            //if (target == user) { API.sendNotificationToPlayer(player, "Don't crash yourself noob"); return; }
            if(target == null) { API.SendInfoNotification(player, "That player isn't connected"); return; }

            API.triggerClientEvent(target.Client, "onLegIn");
        }
    }
}
