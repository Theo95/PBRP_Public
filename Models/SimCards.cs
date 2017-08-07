namespace PBRP
{
    public class SimCard
    {
        public const int CONTACT_CAPACITY = 10;
        public int Id { get; set; }
        public string Number { get; set; }
        public long Credit { get; set; }
        public bool IsBlocked { get; set; }
    }
}
