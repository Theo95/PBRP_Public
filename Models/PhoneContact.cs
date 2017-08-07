namespace PBRP
{
    public class PhoneContact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Notes { get; set; }
        public bool IsFavourite { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsSimContact { get; set; }
        public int SavedTo { get; set; }
    }
}
