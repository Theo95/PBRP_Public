using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Server.API;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.Elements;
using System.Linq;
using System;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Managers;
using System.IO;
using Newtonsoft.Json;

namespace PBRP
{
    public class LumberjackManager : Script
    {
        public static List<Tree> CurrentTrees = new List<Tree>()
        {
            new Tree(new Vector3(-644.2843, 5241.361, 75.16234), TreeHeight.Medium),
            new Tree(new Vector3(-633.0884, 5274.938, 69.35484), TreeHeight.Big),
            new Tree(new Vector3(-645.9504, 5269.657, 74.23684), TreeHeight.Medium),
            new Tree(new Vector3(-664.054, 5278.241, 74.8046), TreeHeight.Medium),
            new Tree(new Vector3(-658.9286, 5293.473, 70.58839), TreeHeight.Big),
            new Tree(new Vector3(-453.9567, 5880.067, 33.70094), TreeHeight.Big),
            new Tree(new Vector3(-471.1367, 5851.097, 34.68779), TreeHeight.Big),
            new Tree(new Vector3(-489.6069, 5811.81, 37.58608), TreeHeight.Big),
            new Tree(new Vector3(-503.7051, 5791.845, 37.35069), TreeHeight.Big),
            new Tree(new Vector3(-525.169, 5711.943, 45.39577), TreeHeight.Big),
            new Tree(new Vector3(-547.1465, 5664.079, 41.12268), TreeHeight.Big),
            new Tree(new Vector3(-546.3509, 5653.057, 43.08691), TreeHeight.Big),
            new Tree(new Vector3(-548.5809, 5651.549, 42.66331), TreeHeight.Big),
            new Tree(new Vector3(-564.3171, 5629.354, 41.65461), TreeHeight.Big),
            new Tree(new Vector3(-562.4695, 5624.329, 43.07004), TreeHeight.Big),
            new Tree(new Vector3(-397.6623, 5908.47, 39.2543), TreeHeight.Big),
            new Tree(new Vector3(-368.1669, 5910.239, 45.74691), TreeHeight.Big),
            new Tree(new Vector3(-372.0626, 5908.176, 45.60004), TreeHeight.Big),
            new Tree(new Vector3(-381.2982, 5905.628, 44.2668), TreeHeight.Big),
            new Tree(new Vector3(-399.7911, 5879.148, 47.88667), TreeHeight.Big),
            new Tree(new Vector3(-398.4585, 5874.141, 50.41697), TreeHeight.Big),
            new Tree(new Vector3(-400.1654, 5871.934, 50.00154), TreeHeight.Big),
            new Tree(new Vector3(-423.6455, 5846.73, 45.2104), TreeHeight.Big),
            new Tree(new Vector3(-431.7455, 5841.625, 45.42433), TreeHeight.Big),
            new Tree(new Vector3(-440.9738, 5865.113, 38.83224), TreeHeight.Big),
            new Tree(new Vector3(-439.3395, 5880.727, 38.83361), TreeHeight.Big),
            new Tree(new Vector3(-467.2649, 5825.584, 42.34899), TreeHeight.Big),
            new Tree(new Vector3(-461.1454, 5818.075, 46.23886), TreeHeight.Big),
            new Tree(new Vector3(-446.1401, 5802.584, 52.86063), TreeHeight.Big),
            new Tree(new Vector3(-469.6023, 5813.233, 45.55083), TreeHeight.Big),
            new Tree(new Vector3(-451.7079, 5800.046, 51.46379), TreeHeight.Big),
            new Tree(new Vector3(-452.9373, 5797.148, 51.91463), TreeHeight.Big),
            new Tree(new Vector3(-434.7176, 5795.426, 55.52554), TreeHeight.Big),
            new Tree(new Vector3(-428.843, 5811.499, 54.42279), TreeHeight.Big),
            new Tree(new Vector3(-445.9234, 5789.232, 54.62664), TreeHeight.Big),
            new Tree(new Vector3(-487.6157, 5776.4, 46.8788), TreeHeight.Big),
            new Tree(new Vector3(-481.2604, 5769.511, 51.12848), TreeHeight.Big),
            new Tree(new Vector3(-485.7451, 5764.024, 51.50717), TreeHeight.Big),
            new Tree(new Vector3(-501.8138, 5769.542, 43.93857), TreeHeight.Big),
            new Tree(new Vector3(-470.6377, 5760.624, 56.70031), TreeHeight.Big),
            new Tree(new Vector3(-468.5075, 5750.691, 61.16329), TreeHeight.Big),
            new Tree(new Vector3(-476.1401, 5744.096, 59.06798), TreeHeight.Big),
            new Tree(new Vector3(-475.4462, 5739.295, 60.78265), TreeHeight.Big),
            new Tree(new Vector3(-459.9717, 5719.071, 72.72704), TreeHeight.Big),
            new Tree(new Vector3(-458.3235, 5714.117, 72.37598), TreeHeight.Big),
            new Tree(new Vector3(-461.7025, 5688.483, 70.22221), TreeHeight.Big),
            new Tree(new Vector3(-462.0315, 5680.609, 70.12142), TreeHeight.Big),
            new Tree(new Vector3(-463.6094, 5678.68, 69.42133), TreeHeight.Big),
            new Tree(new Vector3(-457.0303, 5711.624, 71.75053), TreeHeight.Big),
            new Tree(new Vector3(-495.7353, 5679.961, 56.24544), TreeHeight.Big),
            new Tree(new Vector3(-446.5284, 5669.687, 66.42767), TreeHeight.Big),
            new Tree(new Vector3(-486.3002, 5647.969, 59.18896), TreeHeight.Big),
            new Tree(new Vector3(-455.6063, 5648.953, 69.34618), TreeHeight.Big),
            new Tree(new Vector3(-462.7101, 5631.026, 59.42131), TreeHeight.Big),
            new Tree(new Vector3(-459.6141, 5629.278, 59.85978), TreeHeight.Big),
            new Tree(new Vector3(-497.4737, 5639.121, 59.97794), TreeHeight.Big),
            new Tree(new Vector3(-480.6474, 5615.311, 66.16125), TreeHeight.Big),
            new Tree(new Vector3(-462.1051, 5614.063, 66.59734), TreeHeight.Big),
            new Tree(new Vector3(-524.9126, 5561.198, 66.55482), TreeHeight.Big),
            new Tree(new Vector3(-536.7116, 5549.064, 62.92838), TreeHeight.Big),
            new Tree(new Vector3(-533.2047, 5538.268, 64.97083), TreeHeight.Big),
            new Tree(new Vector3(-431.5797, 5909.95, 33.27858), TreeHeight.Medium),
            new Tree(new Vector3(-448.38, 5883.133, 35.13894), TreeHeight.Medium),
            new Tree(new Vector3(-453.1629, 5872.471, 34.59969), TreeHeight.Medium),
            new Tree(new Vector3(-458.5127, 5871.348, 32.97519), TreeHeight.Medium),
            new Tree(new Vector3(-473.4572, 5843.952, 33.93843), TreeHeight.Medium),
            new Tree(new Vector3(-492.1998, 5811.94, 37.16479), TreeHeight.Medium),
            new Tree(new Vector3(-583.2917, 5620.064, 39.01203), TreeHeight.Medium),
            new Tree(new Vector3(-402.3488, 5915.117, 36.38628), TreeHeight.Medium),
            new Tree(new Vector3(-395.3815, 5921.052, 37.26026), TreeHeight.Medium),
            new Tree(new Vector3(-379.8547, 5911.308, 42.90747), TreeHeight.Medium),
            new Tree(new Vector3(-371.9243, 5913.893, 44.64932), TreeHeight.Medium),
            new Tree(new Vector3(-396.275, 5892.233, 43.66689), TreeHeight.Medium),
            new Tree(new Vector3(-433.7951, 5832.302, 47.36206), TreeHeight.Medium),
            new Tree(new Vector3(-432.2818, 5859.721, 43.08466), TreeHeight.Medium),
            new Tree(new Vector3(-459.7917, 5848.184, 35.92212), TreeHeight.Medium),
            new Tree(new Vector3(-461.9721, 5849.793, 34.4403), TreeHeight.Medium),
            new Tree(new Vector3(-469.075, 5830.668, 40.49021), TreeHeight.Medium),
            new Tree(new Vector3(-448.7037, 5808.197, 51.59888), TreeHeight.Medium),
            new Tree(new Vector3(-471.0616, 5793.447, 50.81347), TreeHeight.Medium),
            new Tree(new Vector3(-507.1371, 5754.756, 45.30689), TreeHeight.Medium),
            new Tree(new Vector3(-520.0797, 5715.632, 47.03737), TreeHeight.Medium),
            new Tree(new Vector3(-469.7808, 5754.978, 61.56802), TreeHeight.Medium),
            new Tree(new Vector3(-440.0981, 5801.069, 54.367), TreeHeight.Medium),
            new Tree(new Vector3(-447.9923, 5806.111, 53.72897), TreeHeight.Medium),
            new Tree(new Vector3(-466.2306, 5767.05, 56.52039), TreeHeight.Medium),
            new Tree(new Vector3(-499.3818, 5689.785, 54.66635), TreeHeight.Medium),
            new Tree(new Vector3(-489.8956, 5651.86, 58.87086), TreeHeight.Medium),
            new Tree(new Vector3(-463.6778, 5653.92, 64.62086), TreeHeight.Medium),
            new Tree(new Vector3(-459.4241, 5640.6, 61.6202), TreeHeight.Medium),
            new Tree(new Vector3(-485.5763, 5621.933, 64.63821), TreeHeight.Medium),
            new Tree(new Vector3(-504.7256, 5611.048, 65.40281), TreeHeight.Medium),
            new Tree(new Vector3(-475.066, 5587.61, 71.04847), TreeHeight.Medium),
            new Tree(new Vector3(-476.4231, 5583.647, 70.46448), TreeHeight.Medium),
            new Tree(new Vector3(-460.4423, 5540.696, 77.18829), TreeHeight.Medium),
            new Tree(new Vector3(-462.3178, 5537.057, 77.61019), TreeHeight.Medium),
            new Tree(new Vector3(-461.8264, 5544.349, 76.48743), TreeHeight.Medium),
            new Tree(new Vector3(-486.3093, 5544.754, 75.28596), TreeHeight.Medium),
            new Tree(new Vector3(-519.9109, 5551.326, 69.28662), TreeHeight.Medium),
            new Tree(new Vector3(-507.2589, 5563.844, 70.78999), TreeHeight.Medium),
            new Tree(new Vector3(-523.6323, 5569.986, 66.31336), TreeHeight.Medium),
            new Tree(new Vector3(-537.3508, 5554.6, 62.20042), TreeHeight.Medium),
            new Tree(new Vector3(-517.8383, 5509.389, 73.63343), TreeHeight.Medium),
            new Tree(new Vector3(-429.5642, 5913.658, 32.78287), TreeHeight.Small),
            new Tree(new Vector3(-449.0651, 5886.539, 33.56907), TreeHeight.Small),
            new Tree(new Vector3(-473.1322, 5848.296, 33.67045), TreeHeight.Small),
            new Tree(new Vector3(-506.8833, 5773.356, 40.75261), TreeHeight.Small),
            new Tree(new Vector3(-518.8243, 5761.817, 37.20011), TreeHeight.Small),
            new Tree(new Vector3(-422.967, 5912.009, 35.07752), TreeHeight.Small),
            new Tree(new Vector3(-379.9234, 5916.159, 41.34845), TreeHeight.Small),
            new Tree(new Vector3(-423.0877, 5827.007, 53.27644), TreeHeight.Small),
            new Tree(new Vector3(-432.8523, 5827.284, 48.80376), TreeHeight.Small),
            new Tree(new Vector3(-442.8498, 5832.636, 46.42251), TreeHeight.Small),
            new Tree(new Vector3(-463.4894, 5846.014, 35.59444), TreeHeight.Small),
            new Tree(new Vector3(-474.8229, 5787.436, 49.47002), TreeHeight.Small),
            new Tree(new Vector3(-472.2202, 5784.336, 51.92268), TreeHeight.Small),
            new Tree(new Vector3(-510.6338, 5752.116, 43.6894), TreeHeight.Small),
            new Tree(new Vector3(-507.1287, 5747.614, 46.94093), TreeHeight.Small),
            new Tree(new Vector3(-460.4171, 5756.023, 63.32011), TreeHeight.Small),
            new Tree(new Vector3(-469.9315, 5574.652, 71.66483), TreeHeight.Small)
        };

        public LumberjackManager()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if(eventName == "ValidateLumberjackSwing")
            {
                Vector3 pos = (Vector3)arguments[0];
                Tree treeHit = CurrentTrees.FirstOrDefault(t => t.Position.DistanceTo(pos) < 1.2);

                if (!(treeHit?.RemainingWood >= 1)) return;
                treeHit.CurrentStrikes++;
                if (treeHit.CurrentStrikes < treeHit.StrikesRequired) return;
                treeHit.CurrentStrikes = 0;
                treeHit.RemainingWood--;

                var log = API.createObject(1366334172, treeHit.Position.Add(new Vector3(0.7, 0.7, 1.3)), new Vector3(83, 0, 0));
                API.sendNativeToPlayersInRange(sender.position, 30, Hash.FREEZE_ENTITY_POSITION, log.handle, false);
                API.sendNativeToPlayersInRange(sender.position, 30, Hash.SET_ENTITY_DYNAMIC, log.handle, true);

                treeHit.SpawnedLogs.Add(log);
            }
        }

        [Command("tree")]
        public void CreateTree(Client player, string type = "")
        {
            Vector3 treePos = player.position;

            if(CurrentTrees.FirstOrDefault(t => t.Position.DistanceTo(treePos) < 2.5f) != null)
            {
                API.SendErrorNotification(player, "You've already done this tree", 10);
                return;
            }
            switch(type.ToLower())
            {
                case "big":
                    CurrentTrees.Add(new Tree(treePos, TreeHeight.Big));
                    break;
                case "medium":
                    CurrentTrees.Add(new Tree(treePos, TreeHeight.Big));
                    break;
                case "small":
                    CurrentTrees.Add(new Tree(treePos, TreeHeight.Big));
                    break;
                default:
                    API.sendChatMessageToPlayer(player, "/tree [type]    |   Types: Big, Medium, Small");
                    return;
            }

            File.AppendAllText(type + "trees.txt",
                string.Format("\nnew Tree(new Vector3({0}, {1}, {2}), TreeHeight.{3}{4}),", treePos.X, treePos.Y,
                    treePos.Z, type[0].ToString().ToUpper(), type.TrimStart(type[0])));
        }


        [Command("showtreesnearme")]
        public void ShowTreesNearMe(Client player)
        {
            API.triggerClientEvent(player, "displayNearbyTrees", JsonConvert.SerializeObject(CurrentTrees.Select(t => t.Position)));
        }
    }
}
