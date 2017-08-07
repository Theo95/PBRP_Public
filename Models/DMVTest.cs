using System.Collections.Generic;
using GrandTheftMultiplayer.Shared.Math;

namespace PBRP
{
    public class DMVTest
    {
        int i = 0;
        public Vehicle testVehicle;
        readonly List<int> speedFlags = new List<int>();
        public Player testee;
        public int testType;

        public List<Vector3> checkpoints;
        public List<int> speedLimits;

        public bool complete;

        public DMVTest(Player player)
        {
            testee = player;

        }

        public void BeginTest()
        {
            DrivingTestManager.AddCheckpointForPlayer(testee, checkpoints[i], (speedLimits == null) ? 0 : speedLimits[i]);
        }

        public void AddSpeedFlag(int speed)
        {
            speedFlags.Add(speed);
        }

        public bool IsPlayerInTestVehicle()
        {
            return (testee.Client.vehicle.handle == testVehicle.Entity);
        }

        public void PlayerEnteredCheckpoint()
        {

            if(i < checkpoints.Count -1)
            {
                i++;
               DrivingTestManager.AddCheckpointForPlayer(testee, checkpoints[i], (speedLimits == null) ? 0 : speedLimits[i]); //If theres no speed limit use a dummy speed
            }
            else
            {
                EndOfCheckpoints();
            }  
        }

        public void EndOfCheckpoints()
        { 
            DrivingTestManager.SendTestEndMessage(testee, true);
        }

        public void HandleTestResults(int vHealth)
        {
            bool fail;
            double vDamage = ((1000 - vHealth) / 1000f) * 100;
            if (testType == 1 || testType == 2 || testType == 3)
            {
                if (speedFlags.Count > checkpoints.Count / 4 || vDamage > checkpoints.Count / 4)
                {
                    fail = true;
                }
                else
                {
                    fail = false;
                }
            } else if (testType == 6 || testType == 5 || testType == 4)
            {
                fail = vDamage > 0;
            } else
            {
                fail = true;
            }
            if (!fail)
            {
                License license = new License()
                {
                    PlayerId = testee.Id,
                    Type = (LicenseType)testType
                };
                LicenseRepository.AddLicense(license);
            }
            DrivingTestManager.DisplayTestResults(this, testee, "C Class", speedFlags.Count, vDamage, fail);
            

        }

    }
}
