using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;

namespace PBRP
{
    public class VehicleManager : Script
    {
        List<Vehicle> CrusieControlVehicles = new List<Vehicle>();
        static DateTime updateTimeout = DateTime.Now;
        static long tick = 0;
        static int tickTick = 0;
        public VehicleManager()
        {
            API.onPlayerEnterVehicle += OnPlayerEnterVehicle;
            API.onPlayerExitVehicle += OnPlayerExitVehicle;
            API.onClientEventTrigger += OnClientEventTrigger;
            API.onVehicleTrailerChange += OnVehicleTrailerChange;
            API.onUpdate += OnTick;
            ServerInit.OnSecond += OnUpdate;
        }

        private void OnTick()
        {
            tickTick++;
            if(tickTick == 6)
            {
                Vehicle[] cruisingVehs = new Vehicle[CrusieControlVehicles.Count];
                CrusieControlVehicles.CopyTo(cruisingVehs);
                foreach (Vehicle cruise in cruisingVehs)
                {
                    if (cruise.Fuel > 0)
                        API.sendNativeToAllPlayers(Hash.SET_VEHICLE_FORWARD_SPEED, cruise.Entity, cruise.CruiseSpeed);
                    else
                        CrusieControlVehicles.Remove(cruise);
                }
                tickTick = 0;
            }
        }

        private void OnVehicleTrailerChange(NetHandle tower, NetHandle trailer)
        {
            Vehicle.VehicleData[tower].Trailer = trailer;
            if (API.doesEntityExist(trailer))
                Vehicle.VehicleData[tower].Entity.AttachTrailer(trailer);
            else
                Vehicle.VehicleData[tower].Entity.DetachTrailer();
        }

        private void OnUpdate()
        {
            tick++;
            foreach (Vehicle veh in Vehicle.VehicleData.Values)
            {
                veh.OnSecond();
                if (tick % 3 == 0)
                {
                    if (API.shared.getVehicleHealth(veh.Entity) != veh.Health)
                    {
                        var health = API.shared.getVehicleHealth(veh.Entity);
                        if (health < 10 && health > -70) veh.Health = 10;
                        else veh.Health = health;

                        if (veh.Health < 100) veh.IsEngineOn = veh.Entity.engineStatus = false;

                        API.shared.setVehicleHealth(veh.Entity, (float)veh.Health);
                    }
                    if (API.getVehicleOccupants(veh.Entity).Count() > 0)
                    {
                        if (veh.IsDealerVehicle)
                        {
                            Player player = Player.PlayerData.Values.Where(p => p.Id == veh.OwnerId).First();
                            double speed = player.Client.GetSpeed();
                            speed *= 2.23;

                            if (speed > 75)
                            {
                                veh.TestDriveSpeedingWarnings++;
                                if (veh.TestDriveSpeedingWarnings == 4)
                                {
                                    API.SendCloseMessage(veh.Entity, 15f, "Burt Macklin says: Please slow down " + player.Username.Split('_')[0] + ", this isn't a race!");
                                }
                                else if (veh.TestDriveSpeedingWarnings == 8)
                                {
                                    API.SendCloseMessage(veh.Entity, 15f, "Burt Macklin says: This is you last warning! Return to the dealership IMMEDIATELY or I'll be contacting the authorities.");
                                }
                                else if (veh.TestDriveSpeedingWarnings == 12)
                                {
                                    //Call cops
                                }
                            }

                            if (veh.Health < 750)
                            {
                                if (veh.TestDriveDamageWarnings == 3)
                                {
                                    //Call cops
                                    veh.TestDriveDamageWarnings++;
                                }
                            }
                            else if (veh.Health < 800)
                            {
                                if (veh.TestDriveDamageWarnings == 2)
                                {
                                    API.SendCloseMessage(veh.Entity, 15f, "Burt Macklin says: Right! That's enough! Return to dealership now or I'm calling the cops.");
                                    veh.TestDriveDamageWarnings++;
                                }
                            }
                            else if (veh.Health < 950)
                            {
                                if (veh.TestDriveDamageWarnings == 1)
                                {
                                    API.SendCloseMessage(veh.Entity, 15f, "Burt Macklin says: You are really starting to push it now " + player.Username.Split('_')[0] + ", I would consider returning to the dealership soon.");
                                    veh.TestDriveDamageWarnings++;
                                }
                            }
                            else if (veh.Health < 1000)
                            {
                                if (veh.TestDriveDamageWarnings == 0)
                                {
                                    veh.TestDriveDamageWarnings++;
                                    API.SendCloseMessage(veh.Entity, 15f, "Burt Macklin says: Please be careful with the car, if you continue to drive carelessly then you can kiss goodbye to your deposit.");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "onVehicleStreamIn")
            {
                //List<Vehicle> vehicles = Vehicle.VehicleData.Values.Where(v => v.Entity.occupants.Count() == 0).ToList();

                //foreach(Vehicle v in vehicles)
                //{
                //    if (v.UnoccupiedPosition == null || v.UnoccupiedPosition == new Vector3(0, 0, 0))
                //    {
                //        v.Entity.movePosition(v.SavePosition, 100);
                //        v.Entity.moveRotation(v.SaveRotation, 100);
                //        API.sendNativeToPlayer(sender, Hash.SET_ENTITY_COORDS, v.Entity.handle, v.SavePosition.X, v.SavePosition.Y, v.SavePosition.Z, 1, 0, 0, 1);
                //        API.sendNativeToPlayer(sender, Hash.SET_ENTITY_HEADING, v.Entity.handle, v.SaveRotation.Z);
                //    }
                //    else if (sender.position.DistanceTo(v.UnoccupiedPosition) < 600f)
                //    {
                //        API.sendChatMessageToAll("" + API.getVehicleDisplayName((VehicleHash)API.getEntityModel(v.Entity.handle)));
                //        if (v.Entity.position.DistanceTo(v.UnoccupiedPosition) < 5f) continue;
                //        v.Entity.position = v.UnoccupiedPosition.Add(new Vector3(0, 0, 1));
                //        v.Entity.rotation = v.UnoccupiedRotation;
                //        API.sendNativeToPlayer(sender, Hash.SET_ENTITY_COORDS, v.Entity.handle, v.UnoccupiedPosition.X, v.UnoccupiedPosition.Y, v.UnoccupiedPosition.Z + 0.05f, 1, 0 ,0, 0);
                //        API.sendNativeToPlayer(sender, Hash.SET_ENTITY_HEADING, v.Entity.handle, v.UnoccupiedRotation.Z);
                //    }
                //}

                //try
                //{
                //    Vehicle veh = Vehicle.VehicleData[(NetHandle)arguments[0]];

                //    if (API.getEntityFromHandle<GrandTheftMultiplayer.Server.Elements.Vehicle>((NetHandle)arguments[0]).occupants.Count() != 0) return;

                //    if(veh.Entity.position.DistanceTo(veh.UnoccupiedPosition) > 5f)
                //    {

                //    }
                //}
                //catch { }
            }
            else if (eventName == "onVehicleStreamOut")
                OnVehicleStreamOut(sender, (NetHandle)arguments[0]);
            else if (eventName == "VehicleLightToggle")
            {
                Vehicle vehicle = Vehicle.VehicleData[sender.vehicle];
                switch (vehicle.LightState)
                {
                    case 0: { vehicle.Entity.SetLightState(1); vehicle.LightState = 1; break; }
                    case 1: { vehicle.Entity.SetLightState(2); vehicle.Entity.SetLightBrightness(ServerInit.ServerHour >= 4 && ServerInit.ServerMinute <= 7 ? 3.0f : 1.0f); vehicle.LightState = 2; break; }
                    case 2: { vehicle.Entity.SetLightState(2); vehicle.Entity.SetLightBrightness(ServerInit.ServerHour >= 4 && ServerInit.ServerMinute <= 7 ? 5.0f : 3.0f); vehicle.LightState = 0; break; }
                }
            }
            else if (eventName == "ActivateVehicleCruiseControl")
            {
                if (sender.isInVehicle)
                {
                    Vehicle veh = Vehicle.VehicleData[sender.vehicle];
                    if (!veh.CruiseDisabled)
                    {
                        veh.CruiseSpeed = sender.GetSpeed();
                        CrusieControlVehicles.Add(veh);
                    }
                }
            }
            else if (eventName == "DeactivateVehicleCruiseControl")
            {
                Vehicle veh = null;
                Console.WriteLine(arguments[0].GetType());
                if (arguments[0] != null) veh = Vehicle.VehicleData[(NetHandle)arguments[0]];
                else if (sender.isInVehicle) veh = Vehicle.VehicleData[sender.vehicle];

                veh.CruiseSpeed = 0;
                CrusieControlVehicles.Remove(CrusieControlVehicles.FirstOrDefault(cr => cr.Id == veh.Id));
            }
        }

        private void OnVehicleStreamIn(Client player, List<NetHandle> vehicles)
        {
            List<NetHandle> handles = new List<NetHandle>();
            List<Vector3> pos = new List<Vector3>();
            List<Vector3> rot = new List<Vector3>();

            foreach(NetHandle vehicle in vehicles)
            {
                GrandTheftMultiplayer.Server.Elements.Vehicle v = API.getEntityFromHandle<GrandTheftMultiplayer.Server.Elements.Vehicle>(vehicle);
                Console.WriteLine("{0} here", v.handle.Value);
                try
                {
                    foreach (Vehicle veh in Vehicle.VehicleData.Values)
                    {
                        Console.WriteLine("    {0} vehicle", veh.Entity.handle.Value);
                        if (veh.Entity.occupants.Count() == 0)
                        {
                            handles.Add(veh.Entity);
                            pos.Add(veh.UnoccupiedPosition);
                            rot.Add(veh.UnoccupiedRotation);
                        }
                    }
                }
                catch { continue; }
            }
            API.sendChatMessageToAll("" + pos.Count);
            API.triggerClientEvent(player, "ResyncUnoccupiedVehicles", handles, pos, rot);
        }

        private void OnVehicleStreamOut(Client player, NetHandle vehicle)
        {
            
        }

        private void OnPlayerExitVehicle(Client player, NetHandle vehicle)
        {            
            if (Player.PlayerData[player].IncognitoMode) API.setEntityTransparency(vehicle, 255);
            if (API.doesEntityExist(vehicle))
            {
                Vehicle veh = Vehicle.VehicleData[vehicle];
                veh.UnoccupiedPosition = API.getEntityPosition(vehicle);
                veh.UnoccupiedRotation = API.getEntityRotation(vehicle);
            }
        }

        private void OnPlayerEnterVehicle(Client player, NetHandle vehicle)
        {
            if (Player.PlayerData[player].IncognitoMode) API.setEntityTransparency(vehicle, 0);
        }
    }
}
