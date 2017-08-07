using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBRP
{
    [Table("factions")]
    public class Faction
    {
        [NotMapped]
        public static List<Faction> FactionData = new List<Faction>();

        public Faction() { }
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        public string Name { get; set; }
        public int Type { get; set; }
        public int Bank { get; set; }
        public int RadioFrequency { get; set; }

        public bool FactionChatDisabled { get; set; }

        [NotMapped]
        public List<Rank> Ranks = new List<Rank>();

        [NotMapped]
        public List<Vehicle> Vehicles = new List<Vehicle>();


    }
}
