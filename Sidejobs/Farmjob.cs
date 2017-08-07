using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.API;
using System.Linq;
using GrandTheftMultiplayer.Server.Managers;

namespace PBRP
{
    public class Farmjob : Script
    {

        private Vector3 farmJobLoc;
        private ColShape farmJobLocShape;
        private Vector3 farmEndLoc;
        private ColShape farmEndLocShape;
        private List<Vehicle> farmVehicles;
        private List<Vector3> trailerPositions;
        private List<Vector3> trailerRotations;
        private List<Vector3> deliveryLocations;
        private int currentTrailer = 0;
        private List<ColShape> deliveryColshapes;
        private int timeoutMinutes = 1;

        public Farmjob()
        {
            API.onEntityEnterColShape += OnEntityEnterColShapeHandler;
            
            API.onPlayerExitVehicle += OnPlayerExitVehicle;
            
            API.onVehicleTrailerChange += onVehicleTrailerChange;
           

            farmJobLoc = new Vector3(415.047, 6541.469, 27.63217);
            API.createTextLabel("/farmjob", farmJobLoc, 20F, 1F, true, 0);
            farmJobLocShape = API.createSphereColShape(farmJobLoc, 3F);
            farmEndLoc = new Vector3(424.6619, 6507.301, 27.77033);
            farmEndLocShape = API.createSphereColShape(farmEndLoc, 4F);
            farmVehicles = new List<Vehicle>();
            Vehicle tractor1 = getTractor("37", new Vector3(420.3626, 6527.431, 27.82797), new Vector3(0.0428342, -1.050143, -97.23599), "TR1");       
            farmVehicles.Add(tractor1);
            Vehicle tractor2 = getTractor("35", new Vector3(421.2325, 6533.292, 27.82692), new Vector3(0.0428342,-1.050143,-97.23599), "TR2");            
            farmVehicles.Add(tractor2);
            Vehicle tractor3 = getTractor("59", new Vector3(419.1294, 6521.885, 27.85864), new Vector3(0.0428342, -1.050143, -97.23599), "TR3");
            farmVehicles.Add(tractor3);
            Vehicle tractor4 = getTractor("150", new Vector3(418.4298, 6516.21, 27.81448), new Vector3(0.0428342, -1.050143, -97.23599), "TR4");
            farmVehicles.Add(tractor4);
            Vehicle tractor5 = getTractor("146", new Vector3(403.4, 6484.986, 28.82676), new Vector3(3.844258, -1.702022, -91.5300), "TR5");
            farmVehicles.Add(tractor5);
            Vehicle tractor6 = getTractor("93", new Vector3(403.315, 6478.777, 29.00561), new Vector3(-2.450657, -2.067505, -91.8932), "TR6");
            farmVehicles.Add(tractor6);
            spawnTractors();

            trailerPositions = new List<Vector3>();
            trailerRotations = new List<Vector3>();
            trailerPositions.Add(new Vector3(397.0311, 6629.037, 27.86694));
            trailerPositions.Add(new Vector3(512.4148, 6448.61, 30.50339));
            trailerPositions.Add(new Vector3(185.4993, 6470.135, 31.17613));
            trailerPositions.Add(new Vector3(729.4655, 6478.959, 28.09008));
            trailerPositions.Add(new Vector3(300.8165, 6595.132, 29.20819));
            trailerPositions.Add(new Vector3(149.9711, 6490.107, 30.93204));
            trailerRotations.Add(new Vector3(-0.5222971, -9.595428, -7.119334));
            trailerRotations.Add(new Vector3(-0.4163867, -10.60915, 93.2896));
            trailerRotations.Add(new Vector3(1.113829, -8.883333, 36.04318));
            trailerRotations.Add(new Vector3(2.77688, -10.23533, 80.38898));
            trailerRotations.Add(new Vector3(-0.8690057, -11.16257, -0.1264229));
            trailerRotations.Add(new Vector3(0.1671236, -4.8163, 46.84809));

            deliveryLocations = new List<Vector3>
            {
                new Vector3(1454.727, 6582.644, 12.24485),
                new Vector3(-83.98638, 6483.23, 31.57506),
                new Vector3(-224.7934, 6262.949, 31.59894)
            };
            //deliveryLocations.Add(new Vector3(409.4466, 6624.796, 28.34419));
            deliveryColshapes = new List<ColShape>();
            foreach(Vector3 loc in deliveryLocations)
            {
                deliveryColshapes.Add(API.createSphereColShape(loc, 3F));
            }
        }

        private Vehicle getTractor(string color, Vector3 pos, Vector3 rot, string licenseplate)
        {
            Vehicle tractor = new Vehicle
            {
                Model = -2076478498,
                Dimension = 0,
                Color1 = color,
                Color2 = color,
                Health = 1000.0,
                LicensePlate = licenseplate,
                LicensePlateStyle = 1,
                WindowData = new int[4] {0, 0, 0, 0},
                Fuel = 100,
                Locked = false,
                PosX = pos.X,
                PosY = pos.Y,
                PosZ = pos.Z,
                RotX = rot.X,
                RotY = rot.Y,
                RotZ = rot.Z,
                IsEngineOn = false
            };
            return tractor;
        }

        private void spawnTractors()
        {
            foreach(Vehicle v in farmVehicles)
            {
                spawnTractor(v);
            }
        }

        private void spawnTractor(Vehicle v)
        {
            if(v.Entity == null)
            {
                var vehicle = API.createVehicle((VehicleHash)v.Model, v.SavePosition, v.SaveRotation, 0, 0, v.Dimension);

                v.Entity = vehicle;
                if (v.Color1.Length > 3)
                    v.Entity.customPrimaryColor = v.CustomColor(v.Color1);
                else
                    v.Entity.primaryColor = int.Parse(v.Color1);
                if (v.Color2.Length > 3)
                    v.Entity.customSecondaryColor = v.CustomColor(v.Color2);
                else
                    v.Entity.secondaryColor = int.Parse(v.Color2);

                vehicle.engineStatus = v.IsEngineOn;
                vehicle.health = (float)v.Health;
                vehicle.invincible = false;
                vehicle.numberPlate = v.LicensePlate;
                vehicle.numberPlateStyle = v.LicensePlateStyle;

                for (int w = 0; w < v.WindowData.Length; w++)
                    API.breakVehicleWindow(vehicle, w, v.WindowData[w] == 1 ? true : false);

                API.setVehicleLocked(vehicle, v.Locked);
                Vehicle.VehicleData.Add(vehicle, v);
            }
        }
               


        private void onVehicleTrailerChange(NetHandle vehicle, NetHandle trailer)
        {
            if(!trailer.IsNull && API.doesEntityExist(trailer) && !vehicle.IsNull && API.doesEntityExist(vehicle))//both trailer & vehicle exist
            {
                if(farmVehicles.Where(b => b.Entity.Equals(vehicle)).FirstOrDefault() != null)
                {
                    handleTractorTrailer(farmVehicles.Where(b => b.Entity.Equals(vehicle)).FirstOrDefault(), trailer);
                }
            }
        }

        private void handleTractorTrailer(Vehicle vehicle, NetHandle trailer)
        {
            Client player = API.getVehicleOccupants(vehicle.Entity).FirstOrDefault();
            if(player != null && (API.getEntityData(player, "isDelivering") == null || !API.getEntityData(player, "isDelivering")) 
            && vehicle.SidejobUserId == Player.PlayerData[player].Id && API.getEntityData(vehicle.Entity, "farmTrailer") != null && API.getEntityData(vehicle.Entity, "farmTrailer") == trailer)
            {
                API.setEntityData(player, "isDelivering", true);
                int deliveryId = (int)(API.random() * deliveryLocations.Count);
                API.setEntityData(player, "deliveryId", deliveryId);
                deleteFarmjobLocationMarker(player);
                showFarmjobLocationMarker(player, deliveryLocations.ElementAt(deliveryId));
            }
        }

        private void OnEntityEnterColShapeHandler(ColShape shape, NetHandle entity)
        { 
            if (!entity.IsNull && API.getPlayerFromHandle(entity) != null)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (shape.Equals(farmJobLocShape) && (API.getEntityData(player, "doingFarmjob") == null || !API.getEntityData(player, "doingFarmjob")))
                {
                    deleteFarmjobLocationMarker(player);
                    return;
                }
                if(deliveryColshapes.Contains(shape))
                {
                    if(API.getEntityData(player, "isDelivering") != null && API.getEntityData(player, "isDelivering"))
                    {
                        if (API.getEntityData(player, "deliveryId") != null && API.getEntityData(player, "deliveryId") == deliveryColshapes.IndexOf(shape))
                        {
                            deliverGoods(player);
                        }
                        return;
                    }
                    
                }
                if(shape.Equals(farmEndLocShape))
                {
                    if(API.getEntityData(player, "isReturning") != null && API.getEntityData(player, "isReturning"))
                    {
                        endFarmjob(player);
                    }
                }

            }            
        }

        private void endFarmjob(Client player)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (API.getPlayerVehicle(player) != null && farmVehicles.FirstOrDefault(b => b.Entity == API.getPlayerVehicle(player)) != null)
            {

                player.setData("timeoutFarmJob", Server.Date.Ticks);
                stopFarmSidejob(player);
                int reward = (250 + (int)(API.random() * 500));
                API.sendChatMessageToPlayer(player, "Farmjob completed, $" + reward + " reward!");
                //Actually give the player some money here.
                Player.PlayerData[player].Money += reward;
                Vehicle v = farmVehicles.FirstOrDefault(b => b.Entity == API.getPlayerVehicle(player));
                API.deleteEntity(v?.Entity);
                if (v == null) return;
                v.SidejobUserId = -1;
                v.IsEngineOn = false;
                v.Entity = null;
                spawnTractor(v);
            }
            else
            {
                API.sendChatMessageToPlayer(player, "You need to bring the tractor back to get paid!");
            }
        }

        private void deliverGoods(Client player)
        {
            if (API.doesEntityExist(API.getPlayerVehicle(player)) && farmVehicles.FirstOrDefault(b => b.Entity == API.getPlayerVehicle(player)) != null)
            {
                Vehicle v = farmVehicles.FirstOrDefault(b => b.Entity == API.getPlayerVehicle(player));
                if (v.Entity.trailer != null && API.getEntityData(v.Entity, "farmTrailer") != null && API.getEntityData(v.Entity, "farmTrailer") == v.Entity.trailer)
                {
                    deleteFarmjobLocationMarker(player);
                    v.Entity.DetachTrailer();
                    API.deleteEntity(v.Entity.trailer);
                    API.setEntityData(v.Entity, "farmTrailer", null);
                    API.sendChatMessageToPlayer(player, "Now bring the tractor back to the farm to get your payment!");
                    API.setEntityData(player, "isDelivering", false);
                    API.setEntityData(player, "isReturning", true);
                    showFarmjobLocationMarker(player, farmEndLoc);
                    return;
                }
                
            }
            API.sendChatMessageToPlayer(player, "You actually need to deliver your trailer to get paid..");
            return;
        }

       

        private void OnPlayerExitVehicle(Client player, NetHandle vehicle)
        {
            if(!vehicle.IsNull && API.doesEntityExist(vehicle))
            {
                if(farmVehicles.FirstOrDefault(b => b.Entity.Equals(vehicle)) != null)
                {                    
                    respawnFarmVehicle(player, farmVehicles.FirstOrDefault(b => b.Entity == vehicle));
                }
            }
        }

        private void respawnFarmVehicle(Client player, Vehicle v)
        {
            API.sendChatMessageToPlayer(player, "Get back in the vehicle or the vehicle will despawn!");
            API.delay(10000, true, () =>
            {
                GrandTheftMultiplayer.Server.Elements.Vehicle vehicle = v.Entity;
                if(API.getVehicleOccupants(vehicle).FirstOrDefault() != null &&
                v.SidejobUserId != Player.PlayerData[player].Id)
                {
                    v.SidejobUserId = -1;
                    stopFarmSidejob(player);
                }
                if(API.getVehicleOccupants(vehicle).FirstOrDefault() == null)
                {
                    if(API.getEntityData(v.Entity, "farmTrailer") != null)
                    {
                        API.deleteEntity(API.getEntityData(v.Entity, "farmTrailer"));
                    }
                    API.deleteEntity(vehicle);
                    v.SidejobUserId = -1;
                    v.IsEngineOn = false;
                    v.Entity = null;
                    spawnTractor(v);
                    stopFarmSidejob(player);
                }

            }
            );
        }

        private void stopFarmSidejob(Client player)
        {
            API.setEntityData(player, "doingFarmjob", false);
            API.setEntityData(player, "isDelivering", false);
            API.setEntityData(player, "isReturning", false);
            API.setEntityData(player, "deliveryId", null);
            deleteFarmjobLocationMarker(player);
            API.sendChatMessageToPlayer(player, "Farmjob ended.");
        }
        

        [Command("farmjob")]
        public void farmjob(Client player)
        {
            if (API.getEntityData(player, "doingFarmjob") == null || !API.getEntityData(player, "doingFarmjob"))
            {
                if (player.vehicle != null && farmVehicles.Where(b => b.Entity == player.vehicle).FirstOrDefault() != null)
                {
                    if(player.hasData("timeoutFarmJob") && (Server.Date.Subtract(new DateTime(player.getData("timeoutFarmJob"))).TotalSeconds < timeoutMinutes * 60))
                    //if (API.getEntityData(player, "timeoutFarmJob") != null && (Server.Date.Subtract(new DateTime(API.getEntityData(player, "timeoutFarmJob"))).TotalSeconds < timeoutMinutes * 60))
                        {
                        API.sendChatMessageToPlayer(player, "You can only do this sidejob once every " + timeoutMinutes + " minutes.");
                        return;
                    }
                    deleteFarmjobLocationMarker(player);
                    API.setEntityData(player, "doingFarmjob", true);
                    startFarmjob(player, farmVehicles.Where(b => b.Entity == player.vehicle).FirstOrDefault());
                    return;
                }
                if (API.getPlayersInRadiusOfPosition(2F, farmJobLoc).Where(b => b.handle.Equals(player.handle)).FirstOrDefault() != null)
                {
                    API.sendChatMessageToPlayer(player, "Get inside a tractor and use /farmjob to start the job.");
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "Marker added to the farmjob location!");
                    showFarmjobLocationMarker(player, new Vector3(farmJobLoc.X, farmJobLoc.Y, farmJobLoc.Z - 0.5));
                }
                
            }
            else
            {
                API.sendChatMessageToPlayer(player, "You're already doing the farming job!");
            }
            
        }

        private void startFarmjob(Client player, Vehicle v)
        {
            Player user = Player.PlayerData[player];
            v.SidejobUserId = user.Id;
            createDeliveryLoad(player, v);
        }

        private void createDeliveryLoad(Client player, Vehicle v)
        {
            var vehicle = API.createVehicle(API.vehicleNameToModel("BaleTrailer"), trailerPositions.ElementAt(currentTrailer), trailerRotations.ElementAt(currentTrailer), 59, 59);
            API.triggerClientEvent(player, "toggleFarmjobBlip", true, vehicle.position);
            currentTrailer++;
            if (currentTrailer >= trailerPositions.Count) currentTrailer = 0;
            API.setEntityData(v.Entity, "farmTrailer", vehicle);
            API.sendChatMessageToPlayer(player, "Trailer pickup location marked");
        }

        private bool isPlayerInFarmjobVehicle(Client player)
        {
            return false;
        }

        private void showFarmjobLocationMarker(Client player, Vector3 position)
        {
            deleteFarmjobLocationMarker(player);
            API.triggerClientEvent(player, "toggleFarmjobMarker", true, position.Subtract(new Vector3(0, 0, 1)));
            API.triggerClientEvent(player, "toggleFarmjobBlip", true, position);
        }

        private void deleteFarmjobLocationMarker(Client player)
        {
            API.triggerClientEvent(player, "toggleFarmjobMarker", false, player.position);
            API.triggerClientEvent(player, "toggleFarmjobBlip", false, player.position);
        }

        
    }
}