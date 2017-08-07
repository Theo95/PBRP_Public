using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public class PhoneAppRepository
    {
        public async static Task AddNew(PhoneApp app)
        {
            using (var db = new Database())
            {
                db.PhoneApp.Add(app);
                await db.SaveChangesAsync();
            }
        }
        public static List<PhoneApp> GetPhoneAppsByPhoneId(int id)
        {
            using (var db = new Database())
            {
                return db.PhoneApp.Where(p => p.PhoneId == id).ToList();
            }
        }


        public static async void UpdateAsync(PhoneApp item)
        {
            using (var db = new Database())
            {
                db.PhoneApp.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
