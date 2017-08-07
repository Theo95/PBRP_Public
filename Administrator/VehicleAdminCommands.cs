using System;
using System.Globalization;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace PBRP
{
    public class VehicleAdminCommands : Script
    {
        [Command("testplategen", GreedyArg = true)]
        public void GeneratePlate(Client player, string format)
        {
            API.sendChatMessageToPlayer(player, Globals.GenerateLicensePlate(format));
        }
        [Command("veh", "")]
        public void SpawnAdminVehicle(Client sender, string vehicleName, string color1 = "0", string color2 = "0")
        {
            if(vehicleName.Length < 2) { API.sendChatMessageToPlayer(sender, "/veh [Model name] [Optional: Color 1] [Optional: Color 2]"); return; }
            if (Master.MasterData.Single(m => m.Client == sender).AdminLevel < 2) { Message.NotAuthorised(sender); return; }
            
            var rot = API.getEntityRotation(sender.handle);
            try
            {
                GrandTheftMultiplayer.Server.Elements.Vehicle vehicle = API.createVehicle(API.vehicleNameToModel(vehicleName), sender.position, new Vector3(0, 0, rot.Z), 0, 0);
                if (color1.Length > 3)
                {
                    int red = int.Parse(color1.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                    int green = int.Parse(color1.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                    int blue = int.Parse(color1.Substring(4, 2), NumberStyles.AllowHexSpecifier);
                    vehicle.customPrimaryColor = new Color(red, green, blue);
                    if (color2.Length > 3)
                    {
                        int red2 = int.Parse(color2.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                        int green2 = int.Parse(color2.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                        int blue2 = int.Parse(color2.Substring(4, 2), NumberStyles.AllowHexSpecifier);
                        
                        vehicle.customSecondaryColor = new Color(red2, green2, blue2);
                    }
                    else
                    {
                        vehicle.primaryColor = int.Parse(color2);
                    }
                }
                else
                {
                    vehicle.secondaryColor = int.Parse(color1);
                }

                Vehicle.VehicleData.Add(vehicle, new Vehicle
                {
                    Entity = vehicle,
                    Color1 = color1,
                    Color2 = color2,
                    Model = (int)API.vehicleNameToModel(vehicleName),
                    UnoccupiedPosition = vehicle.position,
                    UnoccupiedRotation = vehicle.rotation,
                    IsAdminVehicle = true,
                    OwnerId = -1,
                    FactionId = -1,
                    Dimension = 0,
                    Fuel = 100
                });

                vehicle.CreateID();
                API.setPlayerIntoVehicle(sender, vehicle, -1);
                vehicle.numberPlate = string.Format("ADM{0:000}", vehicle.ID());
                API.triggerClientEvent(sender, "getModelDimensions", vehicle.model);
            }
            catch
            {
                API.SendErrorNotification(sender, "The vehicle you attempted to spawn does not exist.");
            }
        }

        [Command("destroyallveh", "")]
        public void DestroyAllAdminVehicles(Client sender)
        {
            if (Master.MasterData.Single(m => m.Client == sender).AdminLevel < 3) { Message.NotAuthorised(sender); return; }

            int count = 0;
            foreach(Vehicle v in Vehicle.VehicleData.Values)
            {
                if (v.IsAdminVehicle)
                {
                    for (int i = 0; i < 2000; i++)
                    {
                        if (Vehicle.IDs[i] == v.Entity)
                        {
                            Vehicle.IDs[i] = null;
                            break;
                        }
                    }
                    API.deleteEntity(v.Entity.handle);
                    v.Entity = null;
                    count++;
                }
            }

            API.sendChatMessageToPlayer(sender, String.Format("You successfully deleted {0} admin vehicles", count));
            foreach (var i in Vehicle.VehicleData.Where(d => d.Value.Entity == null).ToList())
            {
                Vehicle.VehicleData.Remove(i.Key);
            }

        }

        [Command("destroyveh", "")]
        public void DestroyAdminVehicle(Client sender, int id = -1)
        {
            Player player = Player.PlayerData[sender];
            if (player.MasterAccount.AdminLevel < 3) { Message.NotAuthorised(sender); return; }

            if(sender.isInVehicle && Vehicle.VehicleData[sender.vehicle].IsAdminVehicle)
            {
                Vehicle ve = Vehicle.VehicleData[sender.vehicle];
                API.sendChatMessageToPlayer(sender, String.Format("You successfully deleted an admin vehicle ({0})", sender.vehicle.displayName));
               
                ve.Entity.Delete();
                return;
            }

            NetHandle closest = Globals.GetClosestVehicle(sender, 6);

            if (closest.IsNull) { API.sendChatMessageToPlayer(sender, "You aren't near any admin vehicle"); return; }
            
            Vehicle veh = Vehicle.VehicleData[closest];

            if (veh.IsAdminVehicle)
            {
                API.sendChatMessageToPlayer(sender, String.Format("You successfully deleted an admin vehicle ({0})", veh.Entity.displayName));
                veh.Entity.Delete();
            }
            else API.sendChatMessageToPlayer(sender, "You aren't near any admin vehicle");
        }

        [Command("getv")]
        public void GetVehicleById(Client player, int vehicleId)
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 1) { Message.NotAuthorised(player); return; }
            Vehicle veh;
            try { veh = Vehicle.GetVehicleByID(vehicleId); } catch { API.SendErrorNotification(player, "Invalid vehicle ID"); return; }

            API.createParticleEffectOnPosition("scr_rcbarry1", "scr_alien_teleport", veh.Entity.position, new Vector3(), 1f, veh.Entity.dimension);

            veh.Entity.position = Player.GetPositionInFrontOfPlayer(player, 3f, 1);

            veh.Entity.dimension = player.dimension;

            veh.Entity.rotation = player.rotation.Add(new Vector3(0, 0, 90));

            if (!veh.Entity.occupants.Any())
            {
                veh.UnoccupiedPosition = veh.Entity.position;
                veh.UnoccupiedRotation = veh.Entity.rotation;
            }

            API.SendInfoNotification(player, String.Format("You have teleported a {0}", veh.Entity.displayName));
        }

        [Command("gotov")]
        public void GotoVehicleById(Client player, int vehicleId)
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 1) { Message.NotAuthorised(player); return; }
            Vehicle veh;
            try { veh = Vehicle.GetVehicleByID(vehicleId); } catch { API.SendErrorNotification(player, "Invalid vehicle ID"); return; }

            API.createParticleEffectOnPosition("scr_rcbarry1", "scr_alien_teleport", player.position, new Vector3(), 1f, player.dimension);

            player.FadeOutIn(10, 300);
            player.dimension = veh.Dimension;
            player.movePosition(veh.Entity.position.Add(new Vector3(1.5, 1.5, 0)), 200);

            user.Dimension = veh.Dimension;
            player.rotation = veh.Entity.rotation.Add(new Vector3(0, 0, 90));

            if (!veh.Entity.occupants.Any())
            {
                if (veh.Entity.position.DistanceTo(new Vector3(0, 0, 0)) < 10f)
                {
                    veh.UnoccupiedPosition = veh.Entity.position;
                    veh.UnoccupiedRotation = veh.Entity.rotation;
                }
            }

            API.SendInfoNotification(player, String.Format("You have teleported to a {0}", veh.Entity.displayName));
        }

        [Command("afix")]
        public void AdminFixVehicle(Client player, int vehicleId = -1)
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel < 1) { Message.NotAuthorised(player); return; }
            if(vehicleId == -1 && !player.isInVehicle) { Message.Info(player, "USAGE:~w~ /afix [Optional: Vehicle ID]"); return; }

            if (player.isInVehicle) player.vehicle.repair();
            else
            {
                try
                {
                    Vehicle veh = Vehicle.GetVehicleByID(vehicleId);
                    veh.Entity.repair();
                    foreach(Client c in veh.Entity.occupants)
                    {
                        if(API.getPlayerVehicleSeat(c) == -1)
                        {
                            Message.AdminMessage(c, String.Format("Admin {0} has repaired your vehicle", player.name));
                            break;
                        }
                    }
                }
                catch
                {
                    API.SendErrorNotification(player, "Invalid vehicle ID");
                }
            }
        }

        [Command("mod")]
        public void DebugMod(Client player, int modType, int mod)
        {
            if (API.isPlayerInAnyVehicle(player))
            {
                API.setVehicleMod(player.vehicle, modType, mod);
            }
        }

        [Command("modcolor")]
        public void ModColor(Client player, int type, int r, int g, int b)
        {
            if (API.isPlayerInAnyVehicle(player))
            {
                if(type == 1)
                {
                    API.setVehicleModColor1(player.vehicle, r, g, b);
                } else if(type == 2)
                {
                    API.setVehicleModColor2(player.vehicle, r, g, b);
                }
            }
        }
    }
}
