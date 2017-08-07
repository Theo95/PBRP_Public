namespace PBRP
{
    public class Rank
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int FactionId { get; set; }

        public bool CanFire { get; set; }
        public bool CanHire { get; set; }

        public bool CanPromote { get; set; }
        public bool CanDemote { get; set; }

        public bool CanFrespawn { get; set; }

        public int OrderIndex { get; set; }

        public bool CanToggleFactionChat { get; set; }

        public bool RepositionVehicles { get; set; }
        public bool IsLeader { get; set; }

    }
}
