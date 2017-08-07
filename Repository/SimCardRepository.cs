using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public class SimCardRepository
    {
        public async static Task AddNew(SimCard sim)
        {
            using (var db = new Database())
            {
                db.SimCard.Add(sim);
                await db.SaveChangesAsync();
            }
        }
        public static SimCard GetSimCardById(int id)
        {
            using (var db = new Database())
            {
                return db.SimCard.Where(p => p.Id == id).FirstOrDefault();
            }
        }
        public static SimCard GetSimCardByNumber(string number)
        {
            using (var db = new Database())
            {
                return db.SimCard.Where(p => p.Number == number).FirstOrDefault();
            }
        }

        public static async void UpdateAsync(SimCard item)
        {
            using (var db = new Database())
            {
                db.SimCard.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
