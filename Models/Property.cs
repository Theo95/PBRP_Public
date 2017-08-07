using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrandTheftMultiplayer.Server.Managers;

namespace PBRP
{
    public enum PropertyType
    {
        General = 0, // No Owner/Inventory, just enter/exit
        Residential = 1, // Owner/co-signer. Inventory
        Commericial = 2,  // Owner/Faction-Owner. Inventory. Sale Inventory.
        Industrial = 3,  // Owner/Faction-Owner. Inventory. Production of some kind.
        Service = 4 //Provides a specific service
    }

    public enum PropertyOwnerType
    {
        Unowned = -1,
        Player = 0,
        Faction = 1
    }
    [Table("properties")]
    public abstract class Property
    {
        protected Property() {
            // --- Set Defaults for Properties as this should be edited after creation
            OwnerType = -1;
            OwnerId = -1;

            IsEnterable = false;
            ExitX = ExitY = ExitZ = 0.0f;
            Dimension = 0;

            Inventory = new List<Inventory>();
            DoormanagerDoors = new List<int>();
            DoormanagerDoorLocations = new List<Vector3>();
        }

        [Key]
        public int Id { get; set; }

        public PropertyType Type { get; set; }
        public int OwnerType { get; set; }
        public int OwnerId { get; set; } // Can only be owned if TypeID is...
        public string Key { get; set; }

        public string Name { get; set; } // Displayed, label, name

        public int Interior { get; set; }

        // -- Enter being WHERE you enter
        public double EnterX { get; set; }
        public double EnterY { get; set; }
        public double EnterZ { get; set; }

        // -- Exit being EXIT you enterY
        public bool IsEnterable { get; set; }
        public double ExitX { get; set; }
        public double ExitY { get; set; }
        public double ExitZ { get; set; }

        public double ExitRZ { get; set; }

        public int Dimension { get; set; }
        public bool IsLocked { get; set; }

        // --- Inventories
        [NotMapped]
        public List<Inventory> Inventory { get; set; }

        // --- Door manager doors
        [NotMapped]
        public List<int> DoormanagerDoors { get; set; }
        [NotMapped]
        public List<Vector3> DoormanagerDoorLocations { get; set; }

        // --- Get and set Positions for the enter/exit
        [NotMapped]
        public Vector3 EnterPosition
        {
            get => new Vector3(EnterX, EnterY, EnterZ);
            set { EnterX = value.X; EnterY = value.Y; EnterZ = value.Z; }
        }

        [NotMapped]
        public Vector3 ExitPosition
        {
            get => new Vector3(ExitX, ExitY, ExitZ);
            set { ExitX = value.X; ExitY = value.Y; ExitZ = value.Z; }
        }

        // --- Misc data
        [NotMapped]
        public NetHandle pickupHandle { get; set; }

        [NotMapped]
        public NetHandle labelHandle { get; set; }

        [NotMapped]
        public ColShape EntranceColShape { get; set; }

        [NotMapped]
        public ColShape ExitColShape { get; set; }

        [NotMapped]
        public static Globals.Colour industrialColour = new Globals.Colour(235, 172, 36);

        [NotMapped]
        public static Globals.Colour commercialColour = new Globals.Colour(25, 159, 235);

        [NotMapped]
        public static Globals.Colour residentialColour = new Globals.Colour(25, 235, 60);

        [NotMapped]
        public static Globals.Colour generalColour = new Globals.Colour(255, 255, 255);
    }
}