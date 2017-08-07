using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Server.API;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace PBRP
{
    class WildlifeHandle
    {
        public int Value { get; set; }
    }

    class WildlifeVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
    public class HuntingManager : Script
    {
        public static List<Vector3[]> WildlifeAreas = new List<Vector3[]>()
        {
            new Vector3[4] { new Vector3(-1185.98, 4891.352, 217.4447), new Vector3(-1142.042, 4979.505, 222.2111), new Vector3(-1024.212, 4915.551, 211.6261), new Vector3(-1099.208, 4855.152, 219.8027) }
        };

        public HuntingManager()
        {
            
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if(eventName == "updateWildlifePositions")
            {
                List<WildlifeHandle> handles = JsonConvert.DeserializeObject<List<WildlifeHandle>>(arguments[0].ToString());
                List<WildlifeVector3> positions = JsonConvert.DeserializeObject<List<WildlifeVector3>>(arguments[1].ToString());

                for(int i = 0; i < handles.Count; i++)
                {
                    Wildlife w = Wildlife.ActiveWildlife.Single(wi => wi.Ped.handle.Value == handles[i].Value);
                    w.CurrentPosition = new Vector3(positions[i].X, positions[i].Y, positions[i].Z);
                }
            }
        }

        public static void Wildlife_Update()
        {
            //List<Client> clientsNearZone = new List<Client>();
            
            //foreach(Vector3[] area in WildlifeAreas)
            //{
            //    clientsNearZone.Add(API.shared.getAllPlayers().Where(c => c.position.DistanceTo(area[0]) < 300 || c.position.DistanceTo(area[3]) < 300).FirstOrDefault());
            //}
            
            //foreach (Wildlife w in Wildlife.ActiveWildlife)
            //{
            //    for(int j = 0; j < WildlifeAreas.Count; j++)
            //    {
            //        if(w.CurrentPosition.IsInArea(WildlifeAreas[j]))
            //        {
            //            w.InZone = j;
            //            break;
            //        }
            //    }

            //    if (clientsNearZone[w.InZone] == null) return;
            //    //foreach (Client c in API.shared.getAllPlayers())
            //    //{
            //    //    if (w.State == AnimalState.Wandering || w.State == AnimalState.Eating)
            //    //    {
            //    //        if (c.position.DistanceTo2D(w.CurrentPosition) < 2 ||
            //    //            w.CurrentPosition.Forward(w.CurrentRotation.Add(new Vector3(0, 0, API.shared.RandomNumber(20, -20))).Z, 5).DistanceTo(c.position) < 2 ||
            //    //            w.CurrentPosition.Forward(w.CurrentRotation.Add(new Vector3(0, 0, API.shared.RandomNumber(20, -20))).Z, 5).DistanceTo(c.position) < 3)
            //    //        {
            //    //            w.State = AnimalState.Fleeing;
            //    //            w.FleeDuration = 5;
            //    //            var pos3 = w.CurrentPosition.Backward(w.CurrentRotation.Add(new Vector3(0, 0, API.shared.RandomNumber(20, -20))).Z + API.shared.RandomNumber(-20, +20), 50f);
            //    //            w.MoveTo(pos3);
            //    //            break;
            //    //        }
            //    //    }
            //    //}
            //    w.movementTick++;

            //    if (w.movementTick >= 10)
            //    {
            //        w.CurrentDesination = w.CurrentPosition.Forward(w.CurrentRotation.Add(new Vector3(0, 0, API.shared.RandomNumber(20, -20))).Z, 20f);
            //        w.MoveTo(w.CurrentDesination);
            //        API.shared.triggerClientEvent(clientsNearZone[w.InZone], "updateWildLifePositions");
            //        Console.WriteLine("Wandering");
            //        w.Hunger += 1.25f;
            //        //if (w.Hunger > 85) w.State = AnimalState.Eating;
            //        w.movementTick = 0;
            //    }

            //    //switch (w.State)
            //    //{
            //    //    case AnimalState.Fleeing:
            //    //        if (w.movementTick >= 12)
            //    //        {
            //    //            if (w.FleeDuration > 0)
            //    //            {
            //    //                w.CurrentPosition.Forward(w.CurrentRotation.Add(new Vector3(0, 0, API.shared.RandomNumber(20, -20))).Z, 50f);
            //    //                w.MoveTo(pos2);
            //    //                w.FleeDuration--;
            //    //                w.Hunger += 3.5f;
            //    //            }
            //    //            else
            //    //            {
            //    //                var pos = w.CurrentPosition.Forward(w.CurrentRotation.Add(new Vector3(0, 0, API.shared.RandomNumber(20, -20))).Z, 50f);
            //    //                w.MoveTo(pos);
            //    //                w.State = AnimalState.Wandering;
            //    //            }
            //    //            w.movementTick = 0;
            //    //        }
            //    //        break;
            //    //    case AnimalState.Wandering:
                           
            //    //        break;
            //    //    case AnimalState.Eating:
            //    //        if (w.Hunger > 10)
            //    //        {
            //    //            Console.WriteLine("Eating");
            //    //            if (!w.IsEating)
            //    //            {
            //    //                w.Ped.playScenario("WORLD_DEER_GRAZING");
            //    //                w.IsEating = true;
            //    //            }
            //    //            w.Hunger -= 2.5f;
            //    //        }
            //    //        else
            //    //        {
            //    //            w.IsEating = false;
            //    //            w.State = AnimalState.Wandering;
            //    //            w.Ped.stopAnimation();
            //    //        }
            //    //        break;
            //    //}

            //}
        }

        private void API_onResourceStart()
        {
            Wildlife.ActiveWildlife = Wildlife.DefaultWildlifeData;
        }
    }
}
