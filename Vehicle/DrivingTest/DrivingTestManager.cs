using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.API;
using System.Threading;
using GrandTheftMultiplayer.Server.Managers;

namespace PBRP
{
    public class DrivingTestManager : Script
    {
        //,  , , , , , , };
        //, , , , , , , };
        // , , , , , , , };
        //List<Vector3> carTestTestCheckpoints = new List<Vector3> { new Vector3(-355.8239, 6301.882, 29.30285),
        //    new Vector3(-185.1787, 6459.077, 30.29364), new Vector3(83.2481, 6594.842, 30.9136), new Vector3(239.0933, 6556.114, 30.94211),
        //    new Vector3(840.8983, 6488.66, 21.8401), new Vector3(1452.776 , 6454.278, 20.71706), new Vector3(1934.495 ,6252.288 ,42.96312 ),
        //    new Vector3( 2547.674, 5329.537, 44.0735), new Vector3( 2438.39, 5132.689, 46.40901), new Vector3( 2130.731, 5227.147, 57.93306),
        //    new Vector3( 1976.042, 5143.327, 42.90797), new Vector3( 2048.698, 5031.756, 40.67209), new Vector3( 2194.423, 4875.664, 42.72592),
        //    new Vector3( 2193.091,4744.069 , 40.41739), new Vector3( 2456.331, 4609.281, 36.40019), new Vector3(2659.016 , 4812.307, 33.15432),
        //    new Vector3( 2785.028, 4934.437, 33.17263), new Vector3( 2739.364, 5054.087, 40.50758), new Vector3( 2633.673,5162.542 , 44.24769),
        //    new Vector3( 2422.112, 5753.864, 45.04983), new Vector3( 2111.546, 6096.104, 50.53423), new Vector3( 1909.887, 6342.538, 42.03036),
        //    new Vector3( 1664.255,6395.3 ,29.44884 ), new Vector3( 1205.907, 6496.803, 20.38505), new Vector3( 788.2864, 6507.893, 23.85465),
        //    new Vector3( 331.096, 6577.696, 28.32033), new Vector3( 62.54784, 6449.534, 30.81625), new Vector3( -111.3657, 6301.282, 30.89694),
        //    new Vector3( -189.5425, 6346.577, 30.98028), new Vector3( -314.6634, 6247.349, 30.91499), new Vector3( -340.303, 6246.402, 30.99117)};
        List<Vector3> carTestCheckpoints = new List<Vector3> { new Vector3(-336.3008, 6266.416, 30.90741), new Vector3(-394.5571, 6289.651, 29.05563), new Vector3(-414.8651, 6221.643, 30.87473), new Vector3(-366.302, 6168.763, 30.78558), new Vector3(-322.4813, 6207.486, 30.78405), new Vector3(-327.23, 6255.128, 30.97483) };
        List<int> carTestSpeeds = new List<int> { 35, 35, 35, 75, 75, 75, 75, 75, 55, 55, 45, 45, 45, 45, 55, 55, 55, 55, 75, 75, 75, 75, 75, 75, 75, 75, 75, 35, 35, 35, 35, 35 };
        List<Vector3> boatCheckpoints = new List<Vector3> { new Vector3(-1602.872, 5275.304, 0.5696181), new Vector3(-1580.25, 5380.23, 1.936208), new Vector3(-1561.779, 5484.929, 0.9014336), new Vector3(-1666.486, 5520.511, 0.3260038), new Vector3(-1727.035, 5425.878, 0.9892352), new Vector3(-1785.664, 5315.618, 0.7948016), new Vector3(-1846.759, 5196.26, 1.1987), new Vector3(-1901.735, 5102.621, 1.615599), new Vector3(-1936.885, 5015.594, 0.1506463), new Vector3(-1983.553, 4929.266, 0.696165), new Vector3(-2038.54, 4836.537, 0.6991981), new Vector3(-2167.568, 4869.626, 0.7328623), new Vector3(-2116.775, 4934.995, 1.868814), new Vector3(-2091.552, 5048.952, 3.20979), new Vector3(-2108.428, 5159.041, 1.30536), new Vector3(-2106.301, 5248.1, 0.8830601), new Vector3(-2107.125, 5340.802, 1.774249), new Vector3(-2077.313, 5439.432, 1.620431), new Vector3(-1987.246, 5503.783, 3.484241), new Vector3(-1926.587, 5416.958, 1.317537), new Vector3(-1854.845, 5394.793, 1.867714), new Vector3(-1781.413, 5433.267, -0.5084282), new Vector3(-1728.564, 5470.511, 1.118055), new Vector3(-1644.832, 5516.88, 2.632271), new Vector3(-1517.587, 5443.671, 0.8662105), new Vector3(-1521.199, 5306.747, 0.6427101), new Vector3(-1604.566, 5262.503, 0.1236739) };
        List<Vector3> heliCheckpoints = new List<Vector3> { new Vector3(1770.347, 3236.076, 87.00728), new Vector3(1778.872, 3233.205, 185.7524), new Vector3(1853.666, 3316.561, 179.9827), new Vector3(2002.655, 3451.184, 170.694), new Vector3(2169.569, 3630.602, 172.2125), new Vector3(2274.637, 3833.395, 182.993), new Vector3(2366.754, 4092.615, 197.2346), new Vector3(2450.469, 4328.186, 204.292), new Vector3(2513.975, 4583.563, 207.8321), new Vector3(2498.323, 4855.747, 206.8256), new Vector3(2404.854, 5068.884, 206.8241), new Vector3(2132.574, 5220.875, 216.1497), new Vector3(1883.613, 5216.419, 214.6756), new Vector3(1705.73, 5091.875, 212.433), new Vector3(1568.324, 4892.163, 210.1374), new Vector3(1493.424, 4695.872, 208.7411), new Vector3(1414.151, 4488.186, 207.312), new Vector3(1249.556, 4379.94, 209.0499), new Vector3(921.1342, 4295.93, 212.5431), new Vector3(617.0785, 4278.51, 216.334), new Vector3(362.1025, 4281.996, 217.4792), new Vector3(87.12811, 4285.756, 218.707), new Vector3(-141.2257, 4087.7, 223.3126), new Vector3(-223.9741, 3797.748, 216.8898), new Vector3(-101.0086, 3487.721, 206.5536), new Vector3(149.5457, 3276.564, 208.6426), new Vector3(540.5969, 3202.623, 220.3685), new Vector3(805.0975, 3186.436, 219.2766), new Vector3(1129.742, 3216.381, 175.5464), new Vector3(1418.962, 3236.198, 164.8289), new Vector3(1607.239, 3257.671, 137.2641), new Vector3(1772.333, 3240.137, 43.12413), };
        List<Vector3> planeCheckpoints = new List<Vector3> { new Vector3(1707.654, 3252.355, 40.94781), new Vector3(1025.367, 3071.278, 76.42781), new Vector3(306.6087, 2944.401, 216.0672), new Vector3(-124.3139, 3151.124, 327.2876), new Vector3(-311.2762, 3776.854, 504.8465), new Vector3(-343.7665, 4180.873, 497.1657), new Vector3(-409.5186, 4902.875, 537.472), new Vector3(-340.6141, 5573.152, 554.8505), new Vector3(-6.306272, 6061.328, 538.9282), new Vector3(400.7025, 6269.28, 535.8164), new Vector3(950.6526, 6249.733, 541.9714), new Vector3(1415.296, 6086.707, 543.706), new Vector3(1798.158, 5748.498, 509.9955), new Vector3(1973.429, 5570.103, 442.0832), new Vector3(2118.687, 5411.573, 378.2003), new Vector3(2323.225, 5152.187, 302.2531), new Vector3(2478.38, 4901.767, 249.2188), new Vector3(2620.787, 4559.812, 217.5082), new Vector3(2639.268, 4194.542, 184.4947), new Vector3(2441.041, 3832.029, 179.6581), new Vector3(2202.752, 3556.065, 140.9223), new Vector3(1951.036, 3360.439, 91.54873), new Vector3(1618.419, 3227.007, 56.08144), new Vector3(1531.249, 3204.217, 40.83105), new Vector3(1340.097, 3152.659, 42.14334) };
        List<VehicleHash> testCars = new List<VehicleHash> { VehicleHash.Emperor, VehicleHash.Oracle, VehicleHash.Premier, VehicleHash.Primo, VehicleHash.Stanier, VehicleHash.Tailgater, VehicleHash.Washington };
        enum DrivingTestTypes { Car = 1, Bike, Truck, Helicopter, Plane, Boat };
        public static List<DMVTest> activeTests = new List<DMVTest>();

        public DrivingTestManager()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
            API.onPlayerExitVehicle += OnPlayerExitVehicle;
            API.onPlayerDisconnected += OnPlayerDisconnected;
        }

        public void OnClientEventTrigger(Client client, string eventName, params object[] args)
        {
            if (eventName == "player_entered_blip")
            {
                DMVTest d = GetTestInstanceByPlayer(Player.PlayerData[client]);
                if (d.IsPlayerInTestVehicle())
                {
                    d.PlayerEnteredCheckpoint();
                }
                else
                {
                    API.sendChatMessageToPlayer(client, "Get back in your car!");
                }
            }
            else if (eventName == "player_found_speeding")
            {
                DMVTest d = GetTestInstanceByPlayer(Player.PlayerData[client]);
                d.AddSpeedFlag(Convert.ToInt32(args[0]));
            }
            else if (eventName == "request_test_results")
            {
                DMVTest d = GetTestInstanceByPlayer(Player.PlayerData[client]);
                d.HandleTestResults(Convert.ToInt32(args[0]));
            }
            else if (eventName == "launch_test_type")
            {
                API.sendChatMessageToPlayer(client, "Test type recieved: " + args[0].ToString());
                if ((int)args[0] == 1 || (int)args[0] == 2 || (int)args[0] == 3)
                {
                    LaunchTest(client, Convert.ToInt32(args[0]));
                }
                else if ((int)args[0] == 6)
                {
                    API.sendChatMessageToPlayer(client, "Triggering Client Event");
                    API.triggerClientEvent(client, "show_boattest_marker");
                }
                else if ((int)args[0] == 5)
                {
                    API.triggerClientEvent(client, "show_planetest_marker");
                }
                else if ((int)args[0] == 4)
                {
                    API.triggerClientEvent(client, "show_helitest_marker");
                }
            }
            else if (eventName == "start_boat_test")
            {
                LaunchTest(client, 6);
            }
            else if (eventName == "start_plane_test")
            {
                LaunchTest(client, 5);
            }
            else if (eventName == "start_heli_test")
            {
                LaunchTest(client, 4);
            }
        }



        public void OnPlayerExitVehicle(Client client, NetHandle vehicle)
        {
            Player player = Player.PlayerData[client];
            foreach (DMVTest test in activeTests)
            {
                if (test.testee == player)
                {
                    if (test.testVehicle.Entity == vehicle)
                    {
                        if (!test.complete)
                        {
                            EndTest(client);
                            break;
                        }
                    }
                }
            }
        }

        public void OnPlayerDisconnected(Client client, String reason)
        {
            try
            {
                Player player = Player.PlayerData[client];
                foreach (DMVTest test in activeTests)
                {
                    if (test.testee == player)
                    {
                        EndTest(client);
                    }
                }
            }
            catch
            {
                //Little fucker didn't login
            }

        }

        [Command("dtest")]
        public void dtest(Client player)
        {
            API.triggerClientEvent(player, "show_test_menu");
        }

        public void LaunchTest(Client player, int type)
        {
            if (type == 1 || type == 2 || type == 3)
            {
                Task.Run(() =>
                {
                    int timeout = 30;
                    bool safe = true;
                    while (Vehicle.VehicleData.Values.Where(v => v.Entity.position.DistanceTo(new Vector3(-342.2152, 6245.498, 30.98962)) <= 5).Count() != 0)
                    {
                        if (timeout <= 0)
                        {
                            API.sendChatMessageToPlayer(player, "Timeout");
                            safe = false;
                            break;
                        }
                        Thread.Sleep(1000);
                        timeout--;
                    }
                    if (safe)
                    {
                        InitTest(player, type);
                    }


                });

            }
            if (type == 4)
            {
                InitTest(player, type);
            }
            if (type == 6)
            {
                InitTest(player, type);
            }
            if (type == 5)
            {
                InitTest(player, type);
            }
        }

        public void InitTest(Client player, int type)
        {
            DMVTest test = new DMVTest(Player.PlayerData[player]) {testType = type};
            if (test.testType == 1 || test.testType == 2 || test.testType == 3)
            {
                API.triggerClientEvent(player, "load_test_interface");
            }

            VehicleHash? model = null;
            Vector3 spawnLoc = null;
            if (type == 1)
            {
                Random rand = new Random();
                int r = rand.Next(testCars.Count);
                model = testCars[r];
                spawnLoc = new Vector3(-342.2152, 6245.498, 30.98962);
            }
            else if (type == 2)
            {
                model = VehicleHash.Sanchez;
                spawnLoc = new Vector3(-342.2152, 6245.498, 30.98962);
            }
            else if (type == 3)
            {
                model = VehicleHash.Boxville;
                spawnLoc = new Vector3(-342.2152, 6245.498, 30.98962);
            }
            else if (type == 4)
            {
                model = VehicleHash.Maverick;
                spawnLoc = new Vector3(1725.108, 3260.298, 41.77298);
            }
            else if (type == 5)
            {
                model = VehicleHash.Vestra;
                spawnLoc = new Vector3(1725.108, 3260.298, 41.77298);
            }
            else if (type == 6)
            {
                model = VehicleHash.Dinghy;
                spawnLoc = new Vector3(-1601.602, 5259.89, 0.1100717);
            }

            Vehicle testVeh = new Vehicle()
            {
                Model = (int)model,
                Color1 = "1",
                Color2 = "1",
                Entity = API.createVehicle((VehicleHash)model, spawnLoc, new Vector3(0, 0, 0), 0, 0)

            };
            test.testVehicle = testVeh;
            Vehicle.VehicleData.Add(testVeh.Entity, testVeh);

            if (type == 1 || type == 2 || type == 3)
            {
                test.checkpoints = carTestCheckpoints;
                test.speedLimits = carTestSpeeds;
            }
            else if (type == 4)
            {
                test.checkpoints = heliCheckpoints;
                test.speedLimits = null;
            }
            else if (type == 5)
            {
                test.checkpoints = planeCheckpoints;
                test.speedLimits = null;
            }
            else if (type == 6)
            {
                test.checkpoints = boatCheckpoints;
                test.speedLimits = null;
            }

            activeTests.Add(test);
            API.setPlayerIntoVehicle(player, test.testVehicle.Entity, -1);
            test.BeginTest();
        }

        [Command("endtest")]
        public void EndTest(Client player)
        {
            SendTestEndMessage(Player.PlayerData[player], false);
            DMVTest test = GetTestInstanceByPlayer(Player.PlayerData[player]);
            API.deleteEntity(test.testVehicle.Entity);
            activeTests.Remove(test);
            API.SendErrorNotification(player, "You have cancelled your driving test.");
        }

        public static void AddCheckpointForPlayer(Player player, Vector3 loc, int maxSpeed)
        {
            API.shared.sendChatMessageToPlayer(player.Client, "Sending checkpoint command with max speeed of " + maxSpeed.ToString() + " and location of " + loc.X.ToString() + loc.Y.ToString() + loc.Z.ToString());
            API.shared.triggerClientEvent(player.Client, "place_client_blip", loc.X, loc.Y, loc.Z, maxSpeed);
        }

        public static void SendTestEndMessage(Player player, bool complete)
        {
            activeTests.Where(t => t.testee == player).First().complete = complete;
            API.shared.triggerClientEvent(player.Client, "end_curr_test", complete);
        }

        public DMVTest GetTestInstanceByPlayer(Player player)
        {
            foreach (DMVTest test in activeTests)
            {
                if (test.testee == player)
                {
                    return test;
                }
            }
            return null;
        }

        public static void DisplayTestResults(DMVTest test, Player player, string vehClass, int speeding, double damage, bool result)
        {
            if (test.testType == 6)
            {
                player.Client.position = new Vector3(-1610.545, 5258.955, 3.974101);
            }
            API.shared.triggerClientEvent(player.Client, "show_test_result", DateTime.Today.ToShortDateString(), player.Client.name.Split(' ')[1], player.Client.name.Split(' ')[0], vehClass, speeding, damage, result);
            API.shared.deleteEntity(test.testVehicle.Entity);
            activeTests.Remove(test);
        }
    }
}
