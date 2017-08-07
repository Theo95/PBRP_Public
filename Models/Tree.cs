using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using System.Collections.Generic;

namespace PBRP
{
    public enum TreeHeight
    {
        Small = 0,
        Medium = 1,
        Big = 2
    }
    public class Tree
    {
        public Tree(Vector3 pos, TreeHeight height)
        {
            Position = pos;
            RemainingWood = height == TreeHeight.Big ? 20 + API.shared.RandomNumber(-5, 5) : height == TreeHeight.Medium ? 10 + API.shared.RandomNumber(-3, 3) : 5 + API.shared.RandomNumber(-1, 1);
            StrikesRequired = height == TreeHeight.Big ? 8 + API.shared.RandomNumber(-2, 2) : height == TreeHeight.Medium ? 6 + API.shared.RandomNumber(-1, 1) : 3 + API.shared.RandomNumber(-1, 1);
            CurrentStrikes = 0;
            SpawnedLogs = new List<Object>();
        }
        public Vector3 Position { get; set; }
        public int RemainingWood { get; set; }
        public int StrikesRequired { get; set; }
        public int CurrentStrikes { get; set; }

        public List<Object> SpawnedLogs { get; set; }

    }
}
