using GrandTheftMultiplayer.Server.Elements;
using PBRP.Logs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBRP
{
    public enum MasterStates
    {
        AwaitingQuiz = 1,
        AwaitingApplication = 2,
        ApplicationSubmitted = 3,
        ApplicationTempDenied = 4,
        ApplicationPermDenied = 5,
        Active = 6,
        Disabled = 7
    };

    [Table("masters")]
    public class Master
    {
        [NotMapped]
        public static List<Master> MasterData = new List<Master>();

        [NotMapped]
        public Client Client { get; set; }

        [Column("id")]
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        [NotMapped]
        public int AdminLevel { get; set; }
        public bool HasDevAccess { get; set; }
        public int FailedLoginAttempts { get; set; }
        public bool AccountLocked { get; set; }
        public string SocialClubName { get; set; }
        public int Tester { get; set; }
        public int Developer { get; set; }

        public int KeyInteract { get; set; }
        public int KeyCursor { get; set; }
        public int KeyInventory { get; set; }

        public int PerformanceSetting { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? LatestLogin { get; set; }
        public string LatestIP { get; set; }

        [NotMapped]
        public ConnectionLog ActiveConnectionLog { get; set; }

        [NotMapped]
        public List<Player> Players { get; set; }
        [NotMapped]
        public Player CreatingCharacter { get; set; }
    }
}
