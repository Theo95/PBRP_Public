using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PBRP
{
    public class FactionManager : Script
    {
        public FactionManager()
        {
            API.onResourceStart += OnResourceStart;
        }

        private async void OnResourceStart()
        {
            Faction.FactionData = await FactionRepository.GetAllFactions();

            foreach(Faction f in Faction.FactionData)
            {
                f.Ranks = await RankRepository.GetRanksByFactionId(f.Id);

                f.Vehicles = await VehicleRepository.GetAllVehiclesByFactionId(f.Id);

                await Task.Run(async () =>
                {
                    foreach (Vehicle v in f.Vehicles)
                    {
                        GrandTheftMultiplayer.Server.Elements.Vehicle vehicle = API.createVehicle((VehicleHash)v.Model, v.SavePosition, v.SaveRotation, 0, 0, v.Dimension);

                        v.Entity = vehicle;

                        v.UnoccupiedPosition = v.SavePosition;
                        v.UnoccupiedRotation = v.SaveRotation;

                        if (v.Color1.Length > 3)
                            v.Entity.customPrimaryColor = v.CustomColor(v.Color1);
                        else
                            v.Entity.primaryColor = int.Parse(v.Color1);
                        if (v.Color2.Length > 3)
                            v.Entity.customSecondaryColor = v.CustomColor(v.Color2);
                        else
                            v.Entity.secondaryColor = int.Parse(v.Color2);

                        for (int i = 0; i < 2000; i++)
                        {
                            if (Vehicle.IDs[i] != null) continue;
                            Vehicle.IDs[i] = vehicle;
                            break;
                        }

                        v.TrunkItems = new List<Inventory>();
                        
                        v.IsEngineOn = false;
                        vehicle.engineStatus = v.IsEngineOn;
                        vehicle.health = (float)v.Health;
                        vehicle.invincible = false;
                        vehicle.numberPlate = v.LicensePlate;
                        vehicle.numberPlateStyle = v.LicensePlateStyle;
                        v.IsAdminVehicle = false;
                        v.IsDealerVehicle = false;
                        //for (int w = 0; w < v.WindowData.Length; w++)
                        //    API.breakVehicleWindow(vehicle, w, v.WindowData[w] == 1 ? true : false);

                        API.setVehicleLocked(vehicle, v.Locked);

                        //c = 1;
                        //foreach (int d in v.DoorData) {
                        //    if (d == 1)
                        //        vehicle.breakDoor(c);
                        //    c++;
                        //}

                        v.Entity = vehicle;
                        Vehicle.VehicleData.Add(vehicle, v);
                        await Task.Delay(100);
                    }
                });
            }
        }

        public static bool IsPlayerAnOfficer(Player player) => player.Faction?.Type == 1;
    }
}
