using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using System.Linq;

namespace PBRP
{
    public class InteractionManager : Script
    {

        public InteractionManager()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "ShowInteractionForVehicle")
            {
                Vehicle closestTo = null;
                Player p = Player.PlayerData[sender];
                foreach (object veh in arguments)
                {
                    Vehicle vehicle = Vehicle.VehicleData[(NetHandle)veh];
                    Inventory key;
                    if ((key = p.Inventory.Where(i => i.Value == vehicle.Key).FirstOrDefault()) != null || vehicle.FactionId == p.FactionId)
                    {
                        if (closestTo == null) { closestTo = vehicle; continue; }

                        if (vehicle.Entity.position.DistanceTo(sender.position) < closestTo.Entity.position.DistanceTo(sender.position)) {
                            closestTo = vehicle;
                        }
                    }
                }
                if (closestTo != null)
                {
                    var lockText = "";
                    lockText = closestTo.Entity.locked ? "UNLOCK" : "LOCK";
                    API.triggerClientEvent(sender, "showVehicleInteractionMenu", closestTo.Entity.handle,
                        !closestTo.Entity.isDoorOpen(5)
                            ? JsonConvert.SerializeObject(new string[2] {lockText, "OPEN"})
                            : JsonConvert.SerializeObject(new string[3] {lockText, "CLOSE", "ACCESS"}));
                }
            }
            else if(eventName == "PerformVehicleInteraction")
            {
                NetHandle vehicle = (NetHandle)arguments[0];
                if (!API.doesEntityExist(vehicle)) return;
                Player p = Player.PlayerData[sender];
                Vehicle v = Vehicle.VehicleData[vehicle];

                if (p.Inventory.Count(k => k.Value == v.Key) != 1 && v.FactionId != p.FactionId) return;
                switch(arguments[1].ToString())
                {
                    case "LOCK":
                    case "UNLOCK":
                        v.Lock();
                        break;
                    case "OPEN":
                    case "CLOSE":
                        Vector3 trunkPos = new Vector3();

                        if (v.TrunkPosition() == VehicleTrunkPosition.Back || v.TrunkPosition() == VehicleTrunkPosition.Front)
                            trunkPos = v.Entity.position.Forward(v.Entity.rotation.Z, (float)arguments[3]);

                        if (sender.position.DistanceTo(trunkPos) < 0.8)
                        {
                            if (!v.Entity.isDoorOpen(5))
                            {
                                if (!v.Locked)
                                {
                                    v.Entity.openDoor(5);
                                    API.triggerClientEvent(sender, "showVehicleInteractionMenu", vehicle, JsonConvert.SerializeObject(new string[3] { "LOCK", "CLOSE", "ACCESS" }));
                                }
                            }
                            else v.Entity.closeDoor(5);
                        }
                        break;
                    case "ACCESS":

                        break;
                }
            }
        }
    }
}
