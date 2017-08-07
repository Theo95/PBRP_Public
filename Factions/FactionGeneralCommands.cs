using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using System;
using System.Linq;

namespace PBRP
{
    public class FactionGeneralCommands : Script
    {

        [Command("f", GreedyArg = true)]
        public void FactionChat(Client player, string message)
        {
            Faction faction;
            Player user = Player.PlayerData[player];
            try { faction = Faction.FactionData.Single(f => f.Id == user.FactionId); }
            catch { API.SendErrorNotification(player, "You are not in a faction."); return; }

            if (!faction.FactionChatDisabled)
            {
                API.SendMessageToAllFactionMemebers(faction,
                    String.Format("~b~[ {0} {1} ] {2}", faction.Ranks[user.FactionRank - 1].Title, player.name, message));
            }
            else API.SendErrorNotification(player, "Faction chat is disabled.");
        }

        [Command("togglef")]
        public async void DisableFactionChat(Client player)
        {
            Faction faction;
            Player user = Player.PlayerData[player];
            faction = Faction.FactionData.FirstOrDefault(f => f.Id == user.FactionId); 

            if(faction == null) { 
                API.SendErrorNotification(player, "You are not in a faction.");
                return;
            }

            if (faction.Ranks[user.FactionRank - 1].CanToggleFactionChat)
            {
                API.SendMessageToAllFactionMemebers(faction,
                    faction.FactionChatDisabled
                        ? String.Format("~y~ {0} {1} has enabled faction chat.",
                            faction.Ranks[user.FactionRank - 1].Title, player.name)
                        : String.Format("~y~ {0} {1} has disabled faction chat.",
                            faction.Ranks[user.FactionRank - 1].Title, player.name));

                faction.FactionChatDisabled = !faction.FactionChatDisabled;

                await FactionRepository.UpdateAsync(faction);
            }
            else API.SendErrorNotification(player, "Your rank doesn't allow you to use this command.");
        }
    }
}
