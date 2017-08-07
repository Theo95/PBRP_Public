using System.Data.Entity;

namespace PBRP
{
    public partial class Database : DbContext
    {
        public static string connString = "connString goes here";

        public Database() : base(connString) { }

        public DbSet<Player> Player { get; set; }
        public DbSet<Skin> Skin { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<GasStation> GasStations { get; set; }
        public DbSet<Master> Master { get; set; }
        public DbSet<Faction> Factions { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Inventory> InventoryItems { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<ShopItem> ShopItems { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<Phone> Phone { get; set; }
        public DbSet<SimCard> SimCard { get; set; }
        public DbSet<PhoneLog> PhoneLog { get; set; }
        public DbSet<PhoneApp> PhoneApp { get; set; }
        public DbSet<PhoneContact> PhoneContact { get; set; }
        public DbSet<Prison> Prison { get; set; }
        public DbSet<PDLog> PDLog { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<EmsCall> EmsCall { get; set; }
 
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }    
}
