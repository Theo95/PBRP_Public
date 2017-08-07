using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.Managers;
using Newtonsoft.Json;

namespace PBRP
{ 
    public class GeneralVehicleCommands : Script
    {

        [Command("lock")]
        public void LockVehicle(Client player)
        {
            Player p = Player.PlayerData[player];
            NetHandle closestVeh = Globals.GetClosestOwnedOrFactionVehicle(p, p.FactionId, 10);
            if(API.doesEntityExist(closestVeh) || player.isInVehicle)
            {
                if (player.isInVehicle) closestVeh = player.vehicle;
                Vehicle.VehicleData[closestVeh].Lock();
            }
        }

        [Command("trunk")]
        public void OpenTrunk(Client player)
        {
            GrandTheftMultiplayer.Server.Elements.Vehicle veh = Globals.GetClosestVehicle(player, 10);
            Vehicle v = Vehicle.VehicleData[veh];
            Player p = Player.PlayerData[player];

            if (veh.isDoorOpen(5))
            {
                veh.closeDoor(5);
            }
            else if (!v.Locked)
            {
                veh.openDoor(5);
                p.VehicleInteractingWith = v;
                p.InEvent = PlayerEvent.AccessingInventory;
                InventoryManager.ShowTrunkInventory(p, v);
            }
        }

        [Command("engine")]
        public void ToggleVehicleEngine(Client player)
        {
            Player user = Player.PlayerData[player];
            if(player.isInVehicle)
            {
                if (user.Inventory.Count(k => k.Value == Vehicle.VehicleData[player.vehicle].Key) == 1 || Vehicle.VehicleData[player.vehicle].FactionId == user.FactionId ||
                Vehicle.VehicleData[player.vehicle].KeyInIgnition)
                {
                    Vehicle veh = Vehicle.VehicleData[player.vehicle];
                    if(veh.OwnerId != -1)
                    {
                        Inventory key = null;
                        if (!veh.IsEngineOn)
                        {
                            key = veh.TrunkItems.FirstOrDefault(k => k.Value == veh.Key && k.OwnerId == veh.Id && k.OwnerType == InventoryOwnerType.Vehicle);

                            if(key != null)
                            {
                                if(!key.AddToPlayer(user, true)) { API.SendWarningNotification(player, "You don't have enough space in your inventory for a this car key"); return; }

                                key.OwnerId = user.Id;
                                key.OwnerType = InventoryOwnerType.Player;
 
                                user.Inventory.Add(key);

                                if (user.OwnedVehicles.All(v => v.Id != veh.Id))
                                {
                                    API.SendInfoNotification(user.Client,
                                        $"The key to this {veh.Entity.displayName} has been added to you inventory.");
                                }
                                veh.TrunkItems.Remove(key);
                                veh.KeyInIgnition = false;
                                veh.IsEngineOn = false;
                                InventoryRepository.UpdateAsync(key);
                                VehicleRepository.UpdateAsync(veh);
                                return;
                            }

                            key = user.Inventory.FirstOrDefault(k => k.Value == veh.Key);

                            if (key != null)
                            {
                                if (veh.Fuel > 0 && veh.Health > 110)
                                {
                                    veh.CalculateTaintedFuel();
                                    veh.KeyInIgnition = true;

                                    key.OwnerId = veh.Id;
                                    key.OwnerType = InventoryOwnerType.Vehicle;
                                    user.Inventory.Remove(key);
                                    veh.TrunkItems.Add(key);
                                    //API.triggerClientEvent(player, "dashboard_update_fuel", veh.Fuel);
                                }
                                else
                                { API.SendErrorNotification(player, "Engine failed to start..."); return; }
                                InventoryRepository.UpdateAsync(key);
                            }
                        }
                        else
                        {
                            key = veh.TrunkItems.FirstOrDefault(k => k.Value == veh.Key && k.OwnerId == veh.Id && k.OwnerType == InventoryOwnerType.Vehicle);
                            if (key != null)
                            {
                                if (!key.AddToPlayer(user, true)) { API.SendWarningNotification(player, "You don't have enough space in your inventory for a this car key"); return; }
                                key.OwnerId = user.Id;
                                key.OwnerType = InventoryOwnerType.Player;
                         
                                user.Inventory.Add(key);

                                if (user.OwnedVehicles.All(v => v.Id != veh.Id))
                                {
                                    API.sendNotificationToPlayer(user.Client,
                                        $"The key to this {veh.Entity.displayName} has been added to you inventory.");
                                }
                                veh.TrunkItems.Remove(key);

                                veh.KeyInIgnition = false;
                                InventoryRepository.UpdateAsync(key);
                            }
                        }
                    }
                    
                    veh.IsEngineOn = !veh.IsEngineOn;
                    API.setVehicleEngineStatus(player.vehicle, veh.IsEngineOn);

                    API.sendNotificationToPlayer(player, veh.IsEngineOn ? "~g~Engine On" : "~r~Engine Off", true);

                    VehicleRepository.UpdateAsync(veh);
                }
                if (Vehicle.VehicleData[player.vehicle].SidejobUserId == user.Id)
                {
                    Vehicle veh = Vehicle.VehicleData[player.vehicle];
                    veh.IsEngineOn = !veh.IsEngineOn;
                    API.setVehicleEngineStatus(player.vehicle, veh.IsEngineOn);

                    API.sendNotificationToPlayer(player, veh.IsEngineOn ? "~g~Engine On" : "~r~Engine Off", true);

                    Vehicle.VehicleData[player.vehicle] = veh;
                    return;
                }
            }
        }

        [Command("seatbelt")]
        public void SetPlayerSeatbelt(Client sender)
        {
            if (sender.isInVehicle)
            {
                sender.seatbelt = !sender.seatbelt;

                API.SendCloseMessage(sender, 15f, "~#C2A2DA~",
                    sender.seatbelt
                        ? String.Format("{0} puts on their seatbelt.", API.getPlayerName(sender))
                        : String.Format("{0} removes their seatbelt.", API.getPlayerName(sender)));
            }
        }

        [Command("togspeed")]
        public void TogSpeed(Client player)
        {
            API.triggerClientEvent(player, "toggle_speedo_display");
        }

        [Command("testvehtorque")]
        public void TestingTorque(Client player, float torque)
        {
            if(player.isInVehicle)
            {
                Task.Run(() =>
                {
                    while (player.vehicle.engineStatus)
                    {
                        Thread.Sleep(1);
                        player.vehicle.engineTorqueMultiplier = torque;
                    }
                });
            }
        }

        [Command("testvehpower")]
        public void TestingPower(Client player, float torque)
        {
            if (player.isInVehicle)
            {
                player.vehicle.enginePowerMultiplier = torque;
            }
        }
    }
}
