using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;

namespace PBRP
{
    public class PDGeneralCommands : Script
    {

        [Command("cuff", "Syntax: /cuff [Part of name]")]
        public void HandcuffOn(Client player, string target = "") { CuffPlayer(player, target, true); }

        [Command("uncuff", "Syntax: /uncuff [Part of name]" )]
        public void HandcuffOff(Client player, string target = "") { CuffPlayer(player, target, false); }

        public void CuffPlayer(Client player, string target, bool cuff)
        {
            if (!FactionManager.IsPlayerAnOfficer(Player.PlayerData[player])) { Message.NotAuthorised(player); return; }

            Player trg = target == "" ? player.GetClosestPlayer(1) : Player.GetPlayerData(target);
            if(trg.Client == player) { API.SendErrorNotification(player, "You can't " + (cuff ? "cuff" : "uncuff") + " yourself."); return; }
            if(trg != null) {
                trg.IsCuffed = cuff;
                API.sendNativeToPlayer(trg.Client, Hash.SET_ENABLE_HANDCUFFS, trg.Client, trg.IsCuffed);
                if (trg.IsCuffed)
                    API.playPlayerAnimation(trg.Client, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "mp_arresting", "idle");
                else
                    API.stopPlayerAnimation(trg.Client);
            }
        }

        [Command("ticket", "Syntax: /ticket [Part of name]")]
        public void GivePlayerTicket(Client player, string target)
        {
            if (!FactionManager.IsPlayerAnOfficer(Player.PlayerData[player])) { Message.NotAuthorised(player); return; }

            Player trg = Player.GetPlayerData(target);
            if(trg != null)
            {
                if(player.position.DistanceTo(trg.Client.position) > 1.76) { API.SendErrorNotification(player, "You are too far away from that player.", 5); return; }
                API.ShowInputPrompt(player, "OnTicketChargeReason", "Reason for ticket", "Please enter the reason for ticketing " + trg.Username.Roleplay() + ":", "Next", "Cancel");
                Player.PlayerData[player].PlayerInteractingWith = trg;
            }
            else { Message.PlayerNotConnected(player); return; }
        }

        [Command("frisk")]
        public void FriskPlayer(Client player, string target = "")
        {
            if (!FactionManager.IsPlayerAnOfficer(Player.PlayerData[player])) { Message.NotAuthorised(player); return; }

            Player p = Player.PlayerData[player];
            Player trg = target == "" ? player.GetClosestPlayer(1) : Player.GetPlayerData(target);
            //if (trg.Client == player) { API.SendErrorNotification(player, "You can't frisk yourself."); return; }
            if (trg != null)
            {
                if (player.position.DistanceTo(trg.Client.position) > 1.76) { API.SendErrorNotification(player, "You are too far away from that player.", 5); return; }
                API.SendInfoNotification(player, string.Format("Awaiting {0}'s response to your request, please wait...", trg.Client.name), 8);
                API.ShowPopupPrompt(trg.Client, "OnPlayerFriskResponse", "Frisk Request", string.Format("{0} {1} wants to frisk you, do you accept?", p.Faction.Ranks[p.FactionRank].Title, p.Username.Roleplay()), "Accept", "Decline");
                p.PlayerInteractingWith = trg;
                trg.PlayerInteractingWith = p;
                p.InEvent = PlayerEvent.AccessingInventory;
            }
        }

        [Command("cellexittest")]
        public void CellExitTest(Client player) {
            CellManager.StartCellExitSequenceForPlayer(player);
        }
    }
}
