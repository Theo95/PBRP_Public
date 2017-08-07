using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP
{
    public class DealershipVehicle
    {
        public static int IdInc = 0;
        public static List<DealershipVehicle> VehiclesInCatalog = new List<DealershipVehicle>()
        {
            new DealershipVehicle(VehicleHash.Asea),
            new DealershipVehicle(VehicleHash.Asterope),
            new DealershipVehicle(VehicleHash.Cognoscenti),
            new DealershipVehicle(VehicleHash.Cog55),
            new DealershipVehicle(VehicleHash.Emperor),
            new DealershipVehicle(VehicleHash.Fugitive),
            new DealershipVehicle(VehicleHash.Ingot),
            new DealershipVehicle(VehicleHash.Intruder),
            new DealershipVehicle(VehicleHash.Premier),
            new DealershipVehicle(VehicleHash.Primo),
            new DealershipVehicle(VehicleHash.Regina),
            new DealershipVehicle(VehicleHash.Schafter2)
        };


        public DealershipVehicle(VehicleHash model)
        {
            Id = IdInc++;
            Model = model;
        }
        public int Id { get; set; }
        public VehicleHash Model { get; set; }
        public string Name => VehDealer.VehicleNameAndManufactuer[Model];
        public string ModelName => API.shared.getVehicleDisplayName(Model);
        public long Price => Model.Price();
        public float MaxAcceleration => API.shared.getVehicleMaxAcceleration(Model);
        public float MaxTraction => API.shared.getVehicleMaxTraction(Model);
        public float MaxSpeed => API.shared.getVehicleMaxSpeed(Model);
        public float MaxOccupants => API.shared.getVehicleMaxOccupants(Model);
        public float MaxBraking => API.shared.getVehicleMaxBraking(Model);
        public string Image => Model.Image();

    }
}
