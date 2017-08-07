using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Constant;
using System.Threading.Tasks;
using System.Threading;
using GrandTheftMultiplayer.Server.API;

namespace PBRP
{
    public enum VehicleModType
    {
        Spoilers = 0,
        FrontBumper = 1,
        RearBumper = 2,
        SideSkirt = 3,
        Exhaust = 4,
        Frame = 5,
        Grille = 6,
        Hood = 7,
        Fender = 8,
        RightFender = 9,
        Roof = 10,
        Engine = 11,
        Brakes = 12,
        Transmission = 13,
        Horns = 14, //(modIndex from 0 to 51)
        Suspension = 15,
        Armor = 16,
        Turbo = 18,
        Xenon = 22,
        FrontWheels = 23,
        BackWheels = 24, //only for motocycles
        Plateholders = 25,
        TrimDesign = 27,
        Ornaments = 28,
        DialDesign = 30,
        SteeringWheel = 33,
        ShifterLeavers = 34,
        Plaques = 35,
        Hydraulics = 38,
        Livery = 48
    }

    public enum VehicleTrunkPosition
    {
        Front = 0,
        Back = 1,
        Motorcycle = 2
    }

    public enum FuelType
    {
        Petrol = 0,
        Diesel = 1,
        Electric = 2,
        Aviation = 3,

    }
    [Table("vehicles")]
    public class Vehicle
    {
        [NotMapped]
        public static Dictionary<NetHandle, Vehicle> VehicleData = new Dictionary<NetHandle, Vehicle>();

        public delegate void onUnoccupiedVehiclePosChanged(Vehicle vehicle);
        public static event onUnoccupiedVehiclePosChanged OnUnoccupiedVehicleUpdate = delegate { };

        [NotMapped]
        public static GrandTheftMultiplayer.Server.Elements.Vehicle[] IDs = new GrandTheftMultiplayer.Server.Elements.Vehicle[2000];

        public Vehicle()
        {
            RepairType = 0;
            RepairCost = 0;
            RepairTime = DateTime.MinValue;
        }
        public Vehicle(GrandTheftMultiplayer.Server.Elements.Vehicle veh)
        {
            Entity = veh;
        }
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [NotMapped]
        public GrandTheftMultiplayer.Server.Elements.Vehicle Entity { get; set; }

        public int Model { get; set; }
        public string DisplayName => API.shared.getVehicleDisplayName((VehicleHash)Model);
        public string Name => VehDealer.VehicleNameAndManufactuer[(VehicleHash)Model];
        public string Color1 { get; set; }
        public string Color2 { get; set; }
        public int Dimension { get; set; }

        public string Key { get; set; }

        public bool KeyInIgnition { get; set; }

        public bool Locked { get; set; }
        public bool IsEngineOn { get; set; }

        public double Health { get; set; }
        public double BodyHealth { get; set; }
        public double DirtLevel { get; set; }
        public double PaintFade { get; set; }

        [NotMapped]
        public double CruiseSpeed { get; set; }
        [NotMapped]
        public bool CruiseDisabled { get; set; }

        public double Fuel { get; set; }
        [NotMapped]
        public double RefillPetrol { get; set; }
        [NotMapped]
        public double RefillDiesel { get; set; }

        public double EngineDamagePerSecond { get; set; }

        public int FactionId { get; set; }
        public int OwnerId { get; set; }

        public string LicensePlate { get; set; }
        public int LicensePlateStyle { get; set; }

        public DateTime RepairTime { get; set; }
        public double RepairCost { get; set; }
        public VehicleRepairType RepairType { get; set; }

        [NotMapped]
        public bool RepairComplete => RepairTime <= Server.Date;

        [NotMapped]
        public int LightState { get; set; }

        [Column("DoorData")]
        public string doorData { get; set; }
        [NotMapped]
        public int[] DoorData { get => doorData.Split(',').Select(int.Parse).ToArray();
            set => doorData = string.Join(",", value);
        }
        [Column("WindowData")]
        public string windowData { get; set; }
        [NotMapped]
        public int[] WindowData { get => windowData.Split(',').Select(int.Parse).ToArray();
            set => windowData = string.Join(",", value);
        }

        public string TyreData { get; set; }

        [NotMapped]
        public int[] TyresData { get => TyreData.Split(',').Select(int.Parse).ToArray();
            set => TyreData = string.Join(",", value);
        }

        public double PosX { get; set; }
        public double PosY { get; set; }
        public double PosZ { get; set; }

        [NotMapped]
        public Vector3 SavePosition {
            get { return new Vector3(PosX, PosY, PosZ); }
            set { PosX = value.X; PosY = value.Y; PosZ = value.Z; }
        }

        [NotMapped]
        private Vector3 unoccupiedPos;
        [NotMapped]
        public Vector3 UnoccupiedPosition
        {
            get => unoccupiedPos;
            set { unoccupiedPos = value; OnUnoccupiedVehicleUpdate(this); }
        }

        [NotMapped]
        public Vector3 UnoccupiedRotation { get; set; }

        public double RotX { get; set; }
        public double RotY { get; set; }
        public double RotZ { get; set; }
        [NotMapped]
        public Ped DealershipEmployee { get; set; }

        [NotMapped]
        public Vector3 SaveRotation
        {
            get => new Vector3(RotX, RotY, RotZ);
            set { RotX = value.X; RotY = value.Y; RotZ = value.Z; }
        }
        [NotMapped]
        public bool IsAdminVehicle { get; set; }

        [NotMapped]
        public bool IsDealerVehicle { get; set; }

        [NotMapped]
        public List<Inventory> TrunkItems { get; set; }

        [NotMapped]
        public int TestDriveDamageWarnings { get; set; }
        [NotMapped]
        public int TestDriveWindowSmashWarnings { get; set; }
        [NotMapped]
        public int TestDriveSpeedingWarnings { get; set; }

        [NotMapped]
        public NetHandle Trailer { get; set; }

        public int? SidejobId { get; set; }

        [NotMapped]
        public int SidejobUserId { get; set; }

        public static event VehicleTakeDamage onVehicleTakeDamage;
        public delegate void VehicleTakeDamage(Vehicle v);

        public static event VehicleSecond onVehicleSecond;
        public delegate void VehicleSecond(Vehicle v);

        public static Vehicle GetVehicleByID(int vehicleId)
        {
            try
            {
                if (IDs[vehicleId] == null) return null;
                Vehicle veh = VehicleData[IDs[vehicleId]];
                return veh;
            }
            catch
            {
                return null;
            }
        }

        public Color CustomColor(string color)
        {
            int red = int.Parse(color.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            int green = int.Parse(color.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            int blue = int.Parse(color.Substring(4, 2), NumberStyles.AllowHexSpecifier);
            return new Color(red, green, blue);
        }

        public void CalculateTaintedFuel()
        {
            FuelType expectedFuel = this.FuelType();
            double taintedFuel = 0;
            double normalFuel = 0;

            if (Math.Abs(RefillDiesel) < 1 && Math.Abs(RefillPetrol) < 1) return;

            if (expectedFuel == FuelType.Petrol) { taintedFuel = RefillDiesel; normalFuel = RefillPetrol + Fuel; }
            else { taintedFuel = RefillPetrol; normalFuel = RefillDiesel + Fuel; }
            Console.WriteLine("{0} {1}", taintedFuel, normalFuel);

            if (taintedFuel > (taintedFuel + normalFuel) * 0.06)
            {
                EngineDamagePerSecond = (taintedFuel / (taintedFuel + normalFuel)) * 12;
            }
            Fuel += RefillPetrol + RefillDiesel;
            RefillPetrol = 0;
            RefillDiesel = 0;
        }

        public void OnSecond()
        {
            onVehicleSecond(this);
        }

        public void Lock()
        {
            Locked = !Locked;
            Entity.locked = Locked;

            Task.Run(async () =>
            {
                API.shared.sendNativeToAllPlayers(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Entity, 1, true);
                API.shared.sendNativeToAllPlayers(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Entity, 0, true);
                API.shared.sendNativeToAllPlayers(Hash.SET_VEHICLE_INTERIORLIGHT, Entity, true);
                API.shared.sendNativeToAllPlayers(Hash.SET_VEHICLE_BRAKE_LIGHTS, Entity, true);
                await Task.Delay(300);
                API.shared.sendNativeToAllPlayers(Hash.SET_VEHICLE_INTERIORLIGHT, Entity, false);
                await Task.Delay(300);
                API.shared.sendNativeToAllPlayers(Hash.START_VEHICLE_HORN, Entity, 0, 30);
                API.shared.sendNativeToAllPlayers(Hash.SET_VEHICLE_INTERIORLIGHT, Entity, true);
                await Task.Delay(300);
                API.shared.sendNativeToAllPlayers(Hash.SET_VEHICLE_BRAKE_LIGHTS, Entity, false);
                API.shared.sendNativeToAllPlayers(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Entity, 1, false);
                API.shared.sendNativeToAllPlayers(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Entity, 0, false);
            });
        }
    }
}
