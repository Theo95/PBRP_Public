using GrandTheftMultiplayer.Server.Managers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBRP
{
    public class GasStation
    {
        [NotMapped]
        public static List<GasStation> GasStations { get; set; }

        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int CurrentPetrol { get; set; }
        public int MaxPetrol { get; set; }
        public int PetrolPrice { get; set; }
        public int CurrentDiesel { get; set; }
        public int MaxDiesel { get; set; }
        public int DieselPrice { get; set; }

        public double RefillAreaX1 { get; set; }
        public double RefillAreaY1 { get; set; }
        public double RefillAreaZ1 { get; set; }
        public double RefillAreaR1 { get; set; }
        public double RefillAreaX2 { get; set; }
        public double RefillAreaY2 { get; set; }
        public double RefillAreaZ2 { get; set; }
        public double RefillAreaR2 { get; set; }

        [NotMapped]
        public ColShape RefillArea1 { get; set; }

        [NotMapped]
        public ColShape RefillArea2 { get; set; }
    }
}
