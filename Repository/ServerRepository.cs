using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public static class ServerRepository
    {
        public static Server LoadServerData()
        {
            using (Database db = new Database())
            {
                return db.Servers.ToList()[0];
            }
        }

        public static async void UpdateAsync()
        {
            Server serverData = new Server();
            using (var db = new Database())
            {
                serverData.DateTime = Server.Date;
                serverData.Id = 1;
                db.Servers.Attach(serverData);
                db.Entry(serverData).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
