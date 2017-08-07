using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Managers;

namespace PBRP
{
    public class Fishing : Script
    {
        private List<Fishtype> FishTypes = new List<Fishtype>();
        private List<Vector3> FishingLocations = new List<Vector3>();
        private Vector3 TopleftCoord = new Vector3(-1800.0000, 4700.0000, 0.0);
        private Vector3 BotrightCoord = new Vector3(2500.0000, 0.0000, 0.0);
        private Vector3 SellFishSpot = new Vector3(-1601.265, 5204.94, 4.310092);
        private ColShape SellFishColShape;

        
        public enum FishLocation
        {
            Sea = 0,
            DeepSea = 1,
            River = 2,
            DeepRiver = 3
        }
        public class Fishtype
        {
            public Fishtype(string FishName, int MaxWeight, int MinWeight, int WeightPrice, int Rarity, FishLocation FishLoc) : base()
            {
                this.FishName = FishName;
                this.MaxWeight = MaxWeight;
                this.MinWeight = MinWeight;
                this.WeightPrice = WeightPrice;
                this.Rarity = Rarity;
                this.FishLoc = FishLoc;
            }

            public string FishName { get; private set; }
            public int MaxWeight { get; private set; }
            public int MinWeight { get; private set; }
            public int WeightPrice { get; private set; }
            public int Rarity { get; private set; }
            public  FishLocation FishLoc { get; private set; }
        }

        public Fishing()
        {
            createFishLocations();
            createFishTypes();
            API.createTextLabel("/sellfish", SellFishSpot, 20F, 1F, true, 0);
            SellFishColShape = API.createSphereColShape(SellFishSpot, 3F);


            API.onClientEventTrigger += OnClientEvent;
            API.onEntityEnterColShape += OnEntityEnterColShapeHandler;
        }

       

        private void createFishTypes()
        {
            FishTypes.Add(new Fishtype("Nothing", 0, 0, 0, 80, FishLocation.River));
            FishTypes.Add(new Fishtype("Bass", 25,7, 8, 70, FishLocation.River));
            FishTypes.Add(new Fishtype("Salmon", 15,3, 7, 50, FishLocation.River));
            FishTypes.Add(new Fishtype("Carp", 20,5, 12, 40, FishLocation.River));
            FishTypes.Add(new Fishtype("Pike", 16,4, 5, 20, FishLocation.River));
            FishTypes.Add(new Fishtype("Trout", 14,2, 9, 0, FishLocation.River));

            FishTypes.Add(new Fishtype("Nothing", 0, 0, 0, 80, FishLocation.DeepRiver));
            FishTypes.Add(new Fishtype("Bass", 29,11, 8, 70, FishLocation.DeepRiver));
            FishTypes.Add(new Fishtype("Salmon", 19,7, 7, 50, FishLocation.DeepRiver));
            FishTypes.Add(new Fishtype("Carp", 24,9, 12, 45, FishLocation.DeepRiver));
            FishTypes.Add(new Fishtype("Pike", 20,8, 5, 20, FishLocation.DeepRiver));
            FishTypes.Add(new Fishtype("Trout", 18,6, 9, 0, FishLocation.DeepRiver));



            FishTypes.Add(new Fishtype("Nothing", 0, 0, 0, 75, FishLocation.Sea));
            FishTypes.Add(new Fishtype("Snapper", 22,4, 8, 60, FishLocation.Sea));
            FishTypes.Add(new Fishtype("Catfish", 20, 8, 12, 45, FishLocation.Sea));
            FishTypes.Add(new Fishtype("Seabass",21, 10, 7, 30, FishLocation.Sea));
            FishTypes.Add(new Fishtype("Haddock", 25, 16, 6, 15, FishLocation.Sea));
            FishTypes.Add(new Fishtype("Cod", 18, 10, 10, 0, FishLocation.Sea));



            FishTypes.Add(new Fishtype("Nothing", 0, 0, 0, 85, FishLocation.DeepSea));
            FishTypes.Add(new Fishtype("Snapper", 26, 8, 8, 70, FishLocation.DeepSea));
            FishTypes.Add(new Fishtype("Catfish", 24, 12, 12, 55, FishLocation.DeepSea));
            FishTypes.Add(new Fishtype("Seabass", 25, 14, 7, 40, FishLocation.DeepSea));
            FishTypes.Add(new Fishtype("Haddock", 29, 20, 6, 25, FishLocation.DeepSea));
            FishTypes.Add(new Fishtype("Cod", 22, 14, 10, 7, FishLocation.DeepSea));
            FishTypes.Add(new Fishtype("Small shark", 25, 10, 17, 0, FishLocation.DeepSea));
        }
        private void createFishLocations()
        {
            FishingLocations.Add(new Vector3(-287.6312, 6621.184, 6.337915));
            FishingLocations.Add(new Vector3(-285.8766, 6633.767, 6.927557));
            FishingLocations.Add(new Vector3(-275.4037, 6646.807, 7.06242));
            FishingLocations.Add(new Vector3(-261.6408, 6645.587, 7.998896));
            FishingLocations.Add(new Vector3(-1004.774, 6271.664, 0.6287376));
            FishingLocations.Add(new Vector3(-1614.446, 5265.827, 4.34039));
            FishingLocations.Add(new Vector3(-979.5212, 4373.746, 10.44343));
            FishingLocations.Add(new Vector3(713.2251, 4093.287, 35.07999));
            FishingLocations.Add(new Vector3(1298.406, 4214.4, 33.66142));
            FishingLocations.Add(new Vector3(1333.777, 4225.744, 33.68709));
            FishingLocations.Add(new Vector3(2214.113, 4576.734, 30.13947));
            FishingLocations.Add(new Vector3(1885.737, 4000.184, 31.20777));
            FishingLocations.Add(new Vector3(1731.823, 3989.231, 30.76721));
            FishingLocations.Add(new Vector3(1480.953, 3928.552, 29.9509));
            FishingLocations.Add(new Vector3(1423.345, 3855.333, 31.0484));
            FishingLocations.Add(new Vector3(-1525.667, 1494.441, 110.3588));
            FishingLocations.Add(new Vector3(-2077.552, 2600.051, 2.708231));
            FishingLocations.Add(new Vector3(-2303.198, 2562.092, 0.9336813));
            FishingLocations.Add(new Vector3(3375.925, 5182.401, -0.06063855));
            FishingLocations.Add(new Vector3(3870.235, 4463.192, 2.364209));
        }
        private void OnClientEvent(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "caughtFish")
            {
                caughtFish(sender);
            }
            else if(eventName == "stopFish")
            {
                API.SendInfoNotification(sender, "You reeled in too soon.");
                stopfish(sender);
            }
        }

        private void OnEntityEnterColShapeHandler(ColShape shape, NetHandle entity)
        {

            if (!entity.IsNull && API.getPlayerFromHandle(entity) != null)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (shape.Equals(SellFishColShape))
                {
                    deleteSidejobLocationMarker(player);
                }
            }
        }


        private void caughtFish(Client player)
        {
            Player user = Player.PlayerData[player];
            user.CaughtFish = true;
            string fishname = user.NewFish.FishName;
            int fishweight = user.NewFishWeight;
            int price = user.NewFish.WeightPrice;
            API.SendSuccessNotification(player, "You've caught a " + fishname + " weighing " + fishweight + " pounds.");
            //user.Inventory.Add(
            Inventory fish = new Inventory()
            {
                Name = fishname + " (" + fishweight + "lbs)",
                Type = InventoryType.Fish,
                Value = price*fishweight + "," + Globals.GetUniqueString(),
                Quantity = 1,
                OwnerId = user.Id,
                OwnerType = InventoryOwnerType.Player,
            };
            InventoryRepository.AddNewInventoryItem(fish);
            fish.Id = InventoryRepository.GetInventoryItemOfTypeByValue(InventoryType.Fish, fish.Value).Id;
            user.Inventory.Add(fish);
            stopfish(player);
        }

        private bool isPlayerOnBoat(Client player)
        {
            GrandTheftMultiplayer.Server.Elements.Vehicle veh = Globals.GetClosestVehicle(player, 4);
            if (veh != null && API.getVehicleClassName(API.getVehicleClass((VehicleHash)veh.model.GetHashCode())).Equals("Boats"))
            {
                return API.fetchNativeFromPlayer<bool>(player, Hash.IS_ENTITY_IN_WATER, veh);
            }
            else return false;
        }

        private bool doesPlayerHaveRod(Client player)
        {
            return true;
        }

        private bool hasFishSpace(Client player)
        {
            Player user = Player.PlayerData[player];
            if(user.Inventory.Count(b => b.Type.Equals(InventoryType.Fish)) >= 5 ) return false;
            return true;
        }

        private FishLocation getPlayerFishingLoc(Client player)
        {
            float depth = API.fetchNativeFromPlayer<float>(player, Hash.GET_ENTITY_HEIGHT_ABOVE_GROUND, player.handle);
            if (player.position.X > TopleftCoord.X && player.position.X < BotrightCoord.X
                && player.position.Y < TopleftCoord.Y && player.position.Y > BotrightCoord.Y)
            {
                if (depth > 25) return FishLocation.DeepRiver;
                return FishLocation.River;
            }
            else
            {
                if (depth > 50) return FishLocation.DeepSea;
                return FishLocation.Sea;
            }

        }

        private bool isPlayerNearFishingSpot(Client player)
        {
            foreach (Vector3 loc in FishingLocations)
            {
                List<Client> players = API.getPlayersInRadiusOfPosition(10F, loc);
                if (players.Contains(player) && player.position.Z > (loc.Z - 2)) return true;
            }
            return false;
        }

        private void startFish(Client player)
        {
            playFishingAnimation(player);
            int delay = 5000 + ((int)(API.random() * 30)) * 1000;
            API.triggerClientEvent(player, "startFish");
            //API.sendChatMessageToPlayer(player, delay.ToString());
            API.delay(delay, true, () =>
            {
                if(Player.PlayerData[player].IsFishing)
                    catchFish(player);
            });
        }

        private void catchFish(Client player)
        {
            IEnumerable<Fishtype> PossibleFish = FishTypes.Where(f => f.FishLoc.Equals(getPlayerFishingLoc(player)));
            int chance = (int)(API.random() * 100);
            Fishtype CaughtFish = PossibleFish.Where(f => f.Rarity <= chance).FirstOrDefault();
            if(CaughtFish!= null)
            {
                if(CaughtFish.MinWeight != 0)
                {
                    API.SendInfoNotification(player, "Seems like you caught something, reel it in, quick! ((Tap left/right))");
                    int weight = CaughtFish.MinWeight + (int)(API.random() * (CaughtFish.MaxWeight - CaughtFish.MinWeight + 1));
                    API.playPlayerAnimation(player, (int)((1 << 0)), "amb@world_human_stand_fishing@idle_a", "idle_c");
                    API.triggerClientEvent(player, "catchFish", weight, CaughtFish.FishName);
                    Player user = Player.PlayerData[player];
                    user.NewFishWeight = weight;
                    user.NewFish = CaughtFish;
                    API.delay(5000, true, () =>
                    {
                        user = Player.PlayerData[player];
                        if (!user.CaughtFish)
                        {
                            API.SendInfoNotification (player, "The fish got away.");
                            stopfish(player);
                        }
                        else
                        {
                            user.CaughtFish = false;
                        }
                    });
                }
                else
                {
                    API.SendInfoNotification (player, "Nothing seems to nibble on your rod.");

                    stopfish(player);
                }
            }
            else
            {
                API.SendErrorNotification (player, "Something went wrong, please submit a bug report.");

                stopfish(player);
            }
        }

        private void playFishingAnimation(Client player)
        {
            Player user = Player.PlayerData[player];
            if (user.FishingRod != null && API.doesEntityExist(user.FishingRod)) API.deleteEntity(user.FishingRod);
            user.FishingRod = API.createObject(1338703913, player.position, player.rotation);
            API.attachEntityToEntity(user.FishingRod, player, "SKEL_L_Hand", new Vector3(0.13f, 0.1f, 0.01f), new Vector3(180f, 90f, 70f));

            //{"idle_c","amb@world_human_stand_fishing@idle_a
            API.stopPlayerAnimation(player);
            API.playPlayerAnimation(player, (int)((1 << 0))  , "amb@world_human_stand_fishing@idle_a", "idle_a");    
        }

        [Command("fish")]
        public void fish(Client player)
        {
            Player user = Player.PlayerData[player];
            if (!user.IsFishing)
            {
                if (player.vehicle != null)
                {
                    API.SendErrorNotification (player, "You can't fish when driving in a vehicle!");
                    return;
                }
                if (!doesPlayerHaveRod(player))
                {
                    API.SendErrorNotification (player, "You can't fish without a fishing rod!");
                    return;
                }
                if(!hasFishSpace(player))
                {
                    API.SendErrorNotification (player, "You can't hold any more fish!");
                    return;
                }
                if (isPlayerOnBoat(player) || isPlayerNearFishingSpot(player))
                {
                    user.IsFishing = true;
                    startFish(player);
                    return;
                }
                API.SendErrorNotification (player, "You're not at a correct fishing location or on a boat!");
            }
        }
        [Command("stopfish")]
        public void stopfish(Client player)
        {
            API.triggerClientEvent(player, "stopFish");
            Player user = Player.PlayerData[player];
            if (user.FishingRod != null && API.doesEntityExist(user.FishingRod)) API.deleteEntity(user.FishingRod);
            API.stopPlayerAnimation(player);
            user.IsFishing = false;
        }

        //[Command("coolbox")]
        //Good values: 0.52 -0.25 0 40 -120 20
        //public void coolbox(Client player, float xoff = 0.0F, float yoff = 0.0F, float zoff = 0.0F, float xrot = 0.0F, float yrot = 0.0F, float zrot = 0.0F)
        //{
        //    if (coolboxtest != null && API.doesEntityExist(coolboxtest)) API.deleteEntity(coolboxtest);
        //    coolboxtest = API.createObject(1925308724, player.position, player.rotation);
        //    API.attachEntityToEntity(coolboxtest, player, "SKEL_L_Hand", new Vector3(xoff, yoff, zoff), new Vector3(xrot, yrot, zrot));
        //}
        [Command("sellfish")]
        public void sellfish(Client player)
        {
            if(API.getPlayersInRadiusOfPosition(4F, SellFishSpot).Contains(player))
            {
                Player user = Player.PlayerData[player];
                if (user.Inventory.Any(f => f.Type.Equals(InventoryType.Fish)))
                {
                    int count = 0;
                    int price = 0;
                    List<Inventory> fishlist = user.Inventory.Where(f => f.Type.Equals(InventoryType.Fish)).ToList();
                    foreach (Inventory i in fishlist)
                    {
                        count++;
                        price += Int32.Parse(i.Value.Split(',')[0]);
                        user.Inventory.Remove(i);
                        InventoryRepository.RemoveInventoryItem(i);
                    }
                    API.SendInfoNotification(player, "Sold "+ count + " fish for a total of $" + price+".");
                    user.Money += price;
                }
                else
                {
                    API.SendErrorNotification(player, "You don't have any fish to sell.");
                }
            }
            else
            {
                API.SendInfoNotification(player, "Marker added to the selling point.");
                showSidejobLocationMarker(player, SellFishSpot.Subtract(new Vector3(0, 0, 1)));
            }
            
        }


        private void showSidejobLocationMarker(Client player, Vector3 position)
        {
            deleteSidejobLocationMarker(player);
            API.triggerClientEvent(player, "toggleFarmjobMarker", true, position);
            API.triggerClientEvent(player, "toggleFarmjobBlip", true, position);
        }

        private void deleteSidejobLocationMarker(Client player)
        {
            API.triggerClientEvent(player, "toggleFarmjobMarker", false, player.position);
            API.triggerClientEvent(player, "toggleFarmjobBlip", false, player.position);
        }

    }
}