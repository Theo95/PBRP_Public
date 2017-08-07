using System.Collections.Generic;
using System.Linq;

namespace PBRP
{
    public static class GasStationRepository
    {

        public static List<GasStation> GetAllGasStations()
        {
            using (var db = new Database())
            {
                return db.GasStations.ToList();
            }
        }

        public static async void UpdateAsync(GasStation item)
        {
            using (var db = new Database())
            {
                db.GasStations.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }

        }
    }
}
