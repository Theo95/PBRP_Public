using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using System;
using System.Linq;

namespace PBRP
{
    public class FactionVehicleCommands : Script
    {
        [Command("fsave")]
        public void SaveFactionVehicle(Client player, int factionid = -1)
        {
            Faction faction;
            Player user = Player.PlayerData[player];
            try { faction = Faction.FactionData.Single(f => f.Id == user.FactionId); }
                catch { API.sendChatMessageToPlayer(player, "~r~ERROR: You are not in a faction."); return; }

            if (factionid != -1)
            {
                if (user.MasterAccount.AdminLevel > 2)
                {
                    if (user.Client.isInVehicle)
                    {
                        if (Vehicle.VehicleData[player.vehicle].IsAdminVehicle)
                        {
                            NetHandle veh = Vehicle.VehicleData[player.vehicle].Entity.handle;
                            Vehicle.VehicleData[veh].IsAdminVehicle = false;
                            Vehicle.VehicleData[veh].LicensePlate = "PD 001";
                            Vehicle.VehicleData[veh].Health = 1000;
                            Vehicle.VehicleData[veh].LicensePlateStyle = 4;
                            Vehicle.VehicleData[veh].DoorData = new int[4] { 0, 0, 0, 0 };
                            Vehicle.VehicleData[veh].WindowData = new int[4] { 0, 0, 0, 0 };
                            Vehicle.VehicleData[veh].FactionId = factionid;
                            Vehicle.VehicleData[veh].SavePosition = Vehicle.VehicleData[player.vehicle].Entity.position;
                            Vehicle.VehicleData[veh].SaveRotation = Vehicle.VehicleData[player.vehicle].Entity.rotation;
                            VehicleRepository.AddNewVehicle(Vehicle.VehicleData[veh]);
                            API.sendChatMessageToPlayer(player, String.Format("~y~You successfully saved the position of the {0}", player.vehicle.displayName));
                        }
                    }
                }
                else { Message.NotAuthorised(player); return; }
            }
            else
            { 
                if (user.MasterAccount.AdminLevel > 2 || faction.Ranks[user.FactionRank - 1].RepositionVehicles)
                {
                    if (Vehicle.VehicleData[player.vehicle].FactionId == user.FactionId)
                    {
                        NetHandle veh = Vehicle.VehicleData[player.vehicle].Entity.handle;
                        Vehicle.VehicleData[veh].SavePosition = Vehicle.VehicleData[player.vehicle].Entity.position;
                        Vehicle.VehicleData[veh].SaveRotation = Vehicle.VehicleData[player.vehicle].Entity.rotation;
                        VehicleRepository.UpdateAsync(Vehicle.VehicleData[veh]);
                        API.sendChatMessageToPlayer(player, String.Format("~y~You successfully saved the position of the {0}", player.vehicle.displayName));
                    }
                }
            }
        }

        [Command("frespawn")]
        public void FactionRespawnVehicles(Client player)
        {
            Player user = Player.PlayerData[player];
            Faction faction;
            try { faction = Faction.FactionData.Single(f => f.Id == user.FactionId); }
            catch { API.sendChatMessageToPlayer(player, "~r~ERROR: You are not in a faction."); return; }

            if (faction.Ranks[user.FactionRank - 1].CanFrespawn)
            {
                int rspCount = 0;
                int count = 0;
                foreach (Vehicle v in Vehicle.VehicleData.Where(i => i.Value.FactionId == faction.Id).Select(i => i.Value))
                {
                    if (!v.Entity.occupants.Any())
                    {
                        v.Entity.position = v.SavePosition;
                        v.Entity.rotation = v.SaveRotation;

                        API.setVehicleLocked(v.Entity, true);
                        v.Locked = true;
                        API.setVehicleEngineStatus(v.Entity, false);
                        v.IsEngineOn = false;
                        v.Entity.repair();
                        API.setVehicleSpecialLightStatus(v.Entity, false);
                        
                        rspCount++;
                    }
                    count++;
                }
                API.SendMessageToAllFactionMemebers(faction, String.Format("~o~{0} out of {1} faction vehicles have been respawned.", rspCount, count));
            }
            else API.SendErrorNotification(player, "You can't respawn faction vehicles at your rank.");
        }
    }
}
