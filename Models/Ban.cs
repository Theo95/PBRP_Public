using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBRP
{
    [Table("bans")]
    public class Ban
    {
        public int Id { get; set; }
        public int BannedId { get; set; }
        public int BannedPlayer { get; set; }
        public int BannedBy { get; set; }
        public int BannedByPlayer { get; set; }
        public DateTime BannedUntil { get; set; }
        public string BanReason { get; set; }

        [NotMapped]
        public Player PlayerBeingBanned { get; set; }

        public void DisplayBanInfo(Client player)
        {
            if (BannedUntil == DateTime.MaxValue)
                API.shared.sendChatMessageToPlayer(player, "~r~You are permanently banned from Paleto Bay Roleplay.");
            else
            {
                if (BannedUntil > Server.Date)
                {
                    API.shared.sendChatMessageToPlayer(player, String.Format("~r~You are temporarily banned from Paleto Bay Roleplay until {0}.",
                        BannedUntil.ToString("dd/MM/yy hh:mm tt")));
                }
                else
                {
                    BanRepository.RemoveAsync(this);
                }
            }
            API.shared.sendChatMessageToPlayer(player, String.Format("~r~For Reason: {0}", BanReason));
            API.shared.sendChatMessageToPlayer(player, "~r~If you wish to appeal the ban, head over the the forums.");
        }
    }
}
