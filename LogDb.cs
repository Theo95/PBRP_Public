using System.Data.Entity;
using PBRP.Logs;

namespace PBRP
{
    public partial class LogDb : DbContext
    {
        //public static string connString = "connString goes here";

        public LogDb() : base(connString) { }

        public DbSet<ConnectionLog> Connections { get; set; }
        public DbSet<CommandLog> Commands { get; set; }
        public DbSet<WeaponLog> Weapons { get; set; }
        public DbSet<CashLog> Cash { get; set; }
        public DbSet<AttackLog> Deaths { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
            base.OnModelCreating(modelBuilder);
        }
    }    
}
