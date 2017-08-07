using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.API;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public class ScoreboardManager : Script
    {
        public ScoreboardManager()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "ScoreboardPopulate")
            {
                PopulateScoreboard(sender);
            }
            else if(eventName == "SettingsSetPerformance")
            {
                Player p = Player.PlayerData[sender];

                p.MasterAccount.PerformanceSetting = (int)arguments[0];
                API.SendInfoNotification(sender, "Performance Setting Changed", 5);

                MasterRepository.UpdateAsync(p.MasterAccount);
            }
            else if(eventName == "KickPlayer")
            {
                Player p = Player.GetPlayerData(arguments[0].ToString());
                if (p == null) { PopulateScoreboard(sender); return; }
                Player.PlayerData[sender].ScoreboardActionPlayer = p;
                API.ShowInputPrompt(sender, "KickPlayerConfirm", "Kick Player: " + p.Username.Roleplay(),
                    "Please provide the reason for kicking " + p.Username.Roleplay() + "?", "Kick", "Cancel");
            }
            else if(eventName == "KickPlayerConfirm")
            {
                if((int)arguments[0] == 1)
                {
                    Player.PlayerData[sender].ScoreboardActionPlayer.Client.kick(arguments[1].ToString());
                }
            }

            else if(eventName == "SpecPlayer")
            {
                new PlayerAdminCommands().SpectatePlayer(sender, arguments[0].ToString());
            }
            else if(eventName == "TeleportToPlayer")
            {
                new PlayerAdminCommands().GoToPlayer(sender, arguments[0].ToString());
            }
        }

        public static void PopulateScoreboard(Client player = null)
        {
            List<Player> pData = Player.PlayerData.Values.ToList();
            var jsonData = JsonConvert.SerializeObject(pData.Select(p => new { realId = p.RealId, p.Username, p.Level, p.Ping }));

            Task.Run(() => {
                if (player == null)
                {
                    foreach (Client c in Player.IDs)
                    {
                        Player p = Player.PlayerData[c];
                        API.shared.sleep(100);
                        API.shared.triggerClientEvent(c, "scoreboardUpdateData", jsonData,
                            p.MasterAccount.AdminLevel > 0 ? 1 : 0);
                    }
                }
                else
                {
                    API.shared.triggerClientEvent(player, "scoreboardUpdateData", jsonData,
                        Player.PlayerData[player].MasterAccount.AdminLevel > 0 ? 1 : 0);
                }
            });
        }
    }
}
