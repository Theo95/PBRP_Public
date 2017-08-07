using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public static class VehicleExtensions
    {
        public static int GetDriver(this GrandTheftMultiplayer.Server.Elements.Vehicle veh)
        {
            int id = API.shared.fetchNativeFromPlayer<int>(Player.IDs.Where(p => p != null).First(), Hash.GET_PED_IN_VEHICLE_SEAT, veh, -1);
            Console.WriteLine(id);
            return id;
        }

        public static Client GetOccupant(this GrandTheftMultiplayer.Server.Elements.Vehicle veh, int seat)
        {
           return API.shared.getVehicleOccupants(veh)[seat - 1];
        }

        public static long Price(this VehicleHash veh)
        {
            string priceString = "";
            switch (API.shared.getVehicleClassName(API.shared.getVehicleClass(veh)))
            {
                case "Sedans":
                    priceString = VehDealer.VehicleDataForSedans[veh][0];
                    break;
                default:
                    priceString = "$0";
                    break;
            }
            return long.Parse(priceString,
                NumberStyles.AllowCurrencySymbol |
                NumberStyles.AllowThousands |
                NumberStyles.AllowDecimalPoint);
        }

        public static long Price(this GrandTheftMultiplayer.Server.Elements.Vehicle veh)
        {
            return ((VehicleHash)veh.GetHashCode()).Price();
        }

        public static string Image(this VehicleHash model)
        {
            switch (API.shared.getVehicleClassName(API.shared.getVehicleClass(model)))
            {
                case "Sedans":
                    return VehDealer.VehicleDataForSedans[model][2];
                case "Coupes":
                    return VehDealer.VehicleDataForCoupes[model][2];
                default:
                    return "";
            }
        }

        public static string Name(this GrandTheftMultiplayer.Server.Elements.Vehicle veh)
        {
            return VehDealer.VehicleNameAndManufactuer[(VehicleHash)veh.model];
        }

        public static void CreateID(this GrandTheftMultiplayer.Server.Elements.Vehicle veh)
        {
            for (int i = 0; i < 2000; i++)
            {
                if (Vehicle.IDs[i] == null)
                {
                    Vehicle.IDs[i] = veh;
                    return;
                }
            }
        }

        public static int ID(this GrandTheftMultiplayer.Server.Elements.Vehicle veh)
        {
            for (int i = 0; i < 2000; i++)
            {
                if (Vehicle.IDs[i] == veh)
                {
                    return i;
                }
            }
            return -1;
        }

        public static void Delete(this GrandTheftMultiplayer.Server.Elements.Vehicle veh, bool removeVehicleData = true)
        {
            for (int i = 0; i < 2000; i++)
            {
                if (Vehicle.IDs[i] == veh)
                {
                    Vehicle.IDs[i] = null;
                    break;
                }
            }

            if (removeVehicleData)
                Vehicle.VehicleData.Remove(veh);
            API.shared.deleteEntity(veh);
        }

        public static void AttachTrailer(this GrandTheftMultiplayer.Server.Elements.Vehicle veh, NetHandle trailer)
        {
            foreach (Client c in API.shared.getAllPlayers())
            {
                if (c.vehicle != veh)
                    API.shared.sendNativeToPlayer(c, Hash.ATTACH_VEHICLE_TO_TRAILER, veh, Vehicle.VehicleData[trailer].Entity, 10f);
            }

        }

        public static void DetachTrailer(this GrandTheftMultiplayer.Server.Elements.Vehicle veh)
        {
            foreach (Client c in API.shared.getAllPlayers())
            {
                if (c.vehicle != veh)
                    API.shared.sendNativeToAllPlayers(Hash.DETACH_VEHICLE_FROM_TRAILER, veh);
            }
            Task.Run(() =>
            {
                API.shared.sleep(50);
                Vector3 pos = API.shared.getEntityPosition(veh);
                API.shared.sleep(450);
                API.shared.setEntityPosition(veh, pos);
            });
        }

        public static void SetLightBrightness(this GrandTheftMultiplayer.Server.Elements.Vehicle veh, float multiplier)
        {
            API.shared.sendNativeToAllPlayers(Hash.SET_VEHICLE_LIGHT_MULTIPLIER, veh, multiplier);
        }

        public static void SetLightState(this GrandTheftMultiplayer.Server.Elements.Vehicle veh, int state)
        {
            API.shared.sendNativeToAllPlayers((ulong)0x1FD09E7390A74D54, veh, state);
            API.shared.sendNativeToAllPlayers(Hash.SET_VEHICLE_LIGHTS, veh, state);
        }

        public static FuelType FuelType(this Vehicle veh)
        {
            return Globals.GetFuelType(veh.Model);
        }

        public static VehicleTrunkPosition TrunkPosition(this Vehicle veh)
        {
            return Globals.GetTrunkPosition(veh.Model);
        }
    }
}
