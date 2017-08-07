using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using System;
using System.ComponentModel;
using System.Linq;

namespace PBRP
{
    public class FactionPlayerCommands : Script
    {

        public FactionPlayerCommands()
        {
            API.onUpdate += OnUpdate;
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        public void AdminChat(Client player, string message)
        {
            //GET INSTANCE OF PLAYER

        }

        private void OnClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "playerHiredIntoFaction")
            {
                if ((int)arguments[0] == 1)
                {
                    Player user = Player.PlayerData[player];
                    if (user.AwaitingFactionInvite != null)
                    {
                        Player.PlayerData[player].Faction = user.AwaitingFactionInvite.Faction;
                        Player.PlayerData[player].FactionId = user.AwaitingFactionInvite.FactionId;
                        Player.PlayerData[player].FactionRank = 1;

                        API.sendChatMessageToPlayer(user.AwaitingFactionInvite.Client, String.Format("~y~ {0} has accepted your invitation into the {1}", player.name, user.AwaitingFactionInvite.Faction.Name));
                        API.sendChatMessageToPlayer(player, String.Format("~y~You have successfully joined the {0}.", user.AwaitingFactionInvite.Faction.Name));

                        API.SendMessageToAllFactionMemebers(user.AwaitingFactionInvite.Faction, String.Format("~y~{0} has joined the {1}", player.name, user.AwaitingFactionInvite.Faction.Name));

                        Player.PlayerData[player].AwaitingFactionInvite = null;

                        PlayerRepository.UpdateAsync(Player.PlayerData[player]);
                    }
                }
            }
        }

        [Command("hire")]
        public void HirePlayerIntoFaction(Client player, string partOfName)
        {
            Faction faction;
            Player user = Player.PlayerData[player];
            try { faction = Faction.FactionData.Single(f => f.Id == user.FactionId); }
            catch { API.sendChatMessageToPlayer(player, "~r~ERROR: You are not in a faction."); return; }

            if (faction.Ranks[user.FactionRank - 1].CanHire)
            {
                try
                {
                    Player target = Player.GetPlayerData(partOfName);
                    if (target.FactionId <= 0)
                    {
                        if (target.AwaitingFactionInvite == null)
                        {
                            target.AwaitingFactionInvite = user;
                            API.ShowPopupPrompt(target.Client, "playerHiredIntoFaction", user.Faction.Name, String.Format("{0} {1} has invited you to join the {2}, do you want to join?",
                                faction.Ranks[user.FactionRank - 1].Title, user.Client.name, faction.Name));
                            API.sendChatMessageToPlayer(player, String.Format("~y~You have invited {0} to join the {1}, use /hire [Part of name] again to cancel the request.",
                                target.Client.name, faction.Name));
                        }
                        else
                        {
                            target.AwaitingFactionInvite = null;
                            API.sendChatMessageToPlayer(player, String.Format("~y~You have cancelled your invite for {0} to join the {1}.",
                                target.Client.name, faction.Name));
                        }
                    }
                    else { API.sendNotificationToPlayer(player, "The specified player is already in a faction."); return; }
                }
                catch { API.sendNotificationToPlayer(player, "The specified player is not connected."); return; }
            }
        }

        [Command("fire")]
        public void FirePlayerFromFaction(Client player, string partOfName)
        {
            Faction faction;
            Player user = Player.PlayerData[player];
            try { faction = Faction.FactionData.Single(f => f.Id == user.FactionId); }
            catch { API.sendChatMessageToPlayer(player, "~r~ERROR: You are not in a faction."); return; }

            if (faction.Ranks[user.FactionRank - 1].CanFire)
            {
                try
                {
                    Player target = Player.GetPlayerData(partOfName);

                    if (target.FactionId == user.FactionId)
                    {
                        target.Faction = null;
                        target.FactionId = 0;
                        target.FactionRank = 0;

                        API.sendChatMessageToPlayer(target.Client, String.Format("~y~{0} {1} has fired you from the {1}.",
                            faction.Ranks[user.FactionRank - 1].Title, user.Client.name, faction.Name));
                        API.sendChatMessageToPlayer(player, String.Format("~y~You have fired {0} from the {1}",
                            target.Client.name, faction.Name));
                    }
                    else { API.sendNotificationToPlayer(player, "The specified player is already in a faction."); return; }
                }
                catch { API.sendNotificationToPlayer(player, "The specified player is not connected."); return; }
            }
        }

        [Command("promote")]
        public void PromoteFactionMember(Client player, string partOfName)
        {
            Faction faction;
            Player user = Player.PlayerData[player];
            try { faction = Faction.FactionData.Single(f => f.Id == user.FactionId); }
            catch { API.sendChatMessageToPlayer(player, "~r~ERROR: You are not in a faction."); return; }

            if (faction.Ranks[user.FactionRank - 1].CanPromote)
            {
                try
                {
                    Player target = Player.GetPlayerData(partOfName);

                    if (target.Faction == user.Faction)
                    {
                        if (user.FactionRank > target.FactionRank)
                        {
                            try
                            {
                                Rank rank = faction.Ranks[target.FactionRank];

                                target.FactionRank += 1;
                                API.sendChatMessageToPlayer(target.Client, String.Format("~y~{0} {1} has promoted you to {1}, congratulations.",
                                    faction.Ranks[user.FactionRank - 1].Title, user.Client.name, rank.Title));
                                API.sendChatMessageToPlayer(player, String.Format("~y~You have promoted {0} to {1}.",
                                    target.Client.name, faction.Ranks[target.FactionRank - 1]));
                            }
                            catch { API.sendChatMessageToPlayer(player, "~r~That player is already the highest rank"); return; }
                        }
                    }
                }
                catch { API.sendNotificationToPlayer(player, "The specified player is not connected."); return; }
            }
        }

        [Command("demote")]
        public void DemoteFactionMember(Client player, string partOfName)
        {
            Faction faction;
            Player user = Player.PlayerData[player];
            try { faction = Faction.FactionData.Single(f => f.Id == user.FactionId); }
            catch { API.sendChatMessageToPlayer(player, "~r~ERROR: You are not in a faction."); return; }

            if (faction.Ranks[user.FactionRank - 1].CanDemote)
            {
                try
                {
                    Player target = Player.GetPlayerData(partOfName);

                    if (user != target)
                    {
                        if (target.Faction == user.Faction)
                        {
                            if (user.FactionRank > 0)
                            {
                                try
                                {
                                    Rank rank = faction.Ranks[target.FactionRank - 2];

                                    target.FactionRank -= 1;
                                    API.sendChatMessageToPlayer(target.Client, String.Format("~y~{0} {1} has demoted you to {1}.",
                                    faction.Ranks[user.FactionRank - 1].Title, user.Client.name, rank.Title));
                                    API.sendChatMessageToPlayer(player, String.Format("~y~You have demoted {0} to {1}.",
                                        target.Client.name, faction.Ranks[target.FactionRank - 1].Title));
                                }
                                catch { API.sendChatMessageToPlayer(player, "~r~That player is already the highest rank"); }
                            }
                            else
                            {
                                FirePlayerFromFaction(player, partOfName);
                            }
                        }
                    }
                    else { API.sendChatMessageToPlayer(player, "~r~ERROR: ~w~You can't promote yourself."); }
                }
                catch { API.sendNotificationToPlayer(player, "The specified player is not connected."); }
            }
        }

        private void OnUpdate()
        {

        }

    }
}
