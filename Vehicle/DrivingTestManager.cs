using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

namespace PBRP
{
    class DrivingTestManager : Script
    {
        //,  , , , , , , };
        //, , , , , , , };
        // , , , , , , , };
        List<Vector3> carTestTestCheckpoints = new List<Vector3> { new Vector3(-342.2152, 6245.498, 30.98962), new Vector3(-355.8239, 6301.882, 29.30285),
            new Vector3(-185.1787, 6459.077, 30.29364), new Vector3(83.2481, 6594.842, 30.9136), new Vector3(239.0933, 6556.114, 30.94211),
            new Vector3(840.8983, 6488.66, 21.8401), new Vector3(1452.776 , 6454.278, 20.71706), new Vector3(1934.495 ,6252.288 ,42.96312 ),
            new Vector3( 2547.674, 5329.537, 44.0735), new Vector3( 2438.39, 5132.689, 46.40901), new Vector3( 2130.731, 5227.147, 57.93306),
            new Vector3( 1976.042, 5143.327, 42.90797), new Vector3( 2048.698, 5031.756, 40.67209), new Vector3( 2194.423, 4875.664, 42.72592),
            new Vector3( 2193.091,4744.069 , 40.41739), new Vector3( 2456.331, 4609.281, 36.40019), new Vector3(2659.016 , 4812.307, 33.15432),
            new Vector3( 2785.028, 4934.437, 33.17263), new Vector3( 2739.364, 5054.087, 40.50758), new Vector3( 2633.673,5162.542 , 44.24769),
            new Vector3( 2422.112, 5753.864, 45.04983), new Vector3( 2111.546, 6096.104, 50.53423), new Vector3( 1909.887, 6342.538, 42.03036),
            new Vector3( 1664.255,6395.3 ,29.44884 ), new Vector3( 1205.907, 6496.803, 20.38505), new Vector3( 788.2864, 6507.893, 23.85465),
            new Vector3( 331.096, 6577.696, 28.32033), new Vector3( 62.54784, 6449.534, 30.81625), new Vector3( -111.3657, 6301.282, 30.89694),
            new Vector3( -189.5425, 6346.577, 30.98028), new Vector3( -314.6634, 6247.349, 30.91499), new Vector3( -340.303, 6246.402, 30.99117)};
        //List<Vector3> carTestTestCheckpoints = new List<Vector3> { new Vector3(-336.3008, 6266.416, 30.90741), new Vector3(-394.5571, 6289.651, 29.05563), new Vector3(-414.8651, 6221.643, 30.87473), new Vector3(-366.302, 6168.763, 30.78558), new Vector3(-322.4813, 6207.486, 30.78405), new Vector3(-327.23, 6255.128, 30.97483) };
        List<int> carTestTestSpeeds = new List<int> {35,35,35,35,75,75,75,75,75,55,55,45,45,45,45,55,55,55,55,75,75,75,75,75,75,75,75,75,35,35,35,35,35};
        enum DrivingTestTypes { Car = 1, Bike, Truck, Helicopter, Plane };
        List<DMVTest> activeTests = new List<DMVTest>();

       public DrivingTestManager()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        public void OnClientEventTrigger(Client client, string eventName, params object[] args)
        {
            if(eventName == "player_entered_blip")
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
            else if(eventName == "player_found_speeding")
            {
                DMVTest d = GetTestInstanceByPlayer(Player.PlayerData[client]);
                d.AddSpeedFlag(Convert.ToInt32(args[0]));
            }
            else if(eventName == "request_test_results")
            {
                DMVTest d = GetTestInstanceByPlayer(Player.PlayerData[client]);
                d.HandleTestResults(Convert.ToInt32(args[0]));
            }
        }

       

        [Command("dtest")]
        public void dtest(Client player)
        {
            DMVTest test = new DMVTest(Player.PlayerData[player]);
            API.triggerClientEvent(player, "load_test_interface");
            test.testVehicle = API.createVehicle(VehicleHash.Elegy, new Vector3(-315.8801, 6249.22, 30.89409), new Vector3(2.262521, 2.527319, 56.63427), 0, 0);
            API.setPlayerIntoVehicle(player, test.testVehicle, -1);
            test.testType = (int)DrivingTestTypes.Car;
            test.checkpoints = carTestTestCheckpoints;
            test.speedLimits = carTestTestSpeeds;
            activeTests.Add(test);
            test.BeginTest();
        }

        public void AddCheckpointForPlayer(Player player, Vector3 loc, int maxSpeed, int upcomingMax)
        {
            API.triggerClientEvent(player.Client, "place_client_blip", loc.X, loc.Y, loc.Z, maxSpeed, upcomingMax);
        }

        public void SendTestEndMessage(Player player)
        {
            API.triggerClientEvent(player.Client, "end_curr_test");
        }

        public DMVTest GetTestInstanceByPlayer(Player player)
        {
            foreach(DMVTest test in activeTests)
            {
                if(test.testee == player)
                {
                    return test;
                }
            }
            return null;
        }

        public void DisplayTestResults(DMVTest test, Player player, string vehClass, int speeding, double damage, string result)
        {
            API.triggerClientEvent(player.Client, "show_test_result", vehClass, speeding, damage, result);
            API.deleteEntity(test.testVehicle);
            activeTests.Remove(test);
        }
    }
}
