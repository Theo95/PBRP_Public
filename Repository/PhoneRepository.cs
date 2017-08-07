using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public class PhoneRepository
    {
        public static async Task AddNew(Phone phone)
        {
            using (var db = new Database())
            {
                db.Phone.Add(phone);
                await db.SaveChangesAsync();
            }
        }

        public static Phone GetPhoneById(int id)
        {
            using (var db = new Database())
            {
                return db.Phone.FirstOrDefault(p => p.Id == id);
            }
        }

        public static Phone GetPhoneByIMEI(long IMEI)
        {
            using (var db = new Database())
            {
                return db.Phone.FirstOrDefault(p => p.IMEI == IMEI);
            }
        }

        public static Phone GetPhoneBySim(int id)
        {
            using (var db = new Database())
            {
                return db.Phone.FirstOrDefault(p => p.InstalledSim == id);
            }
        }

        public static async void UpdateAsync(Phone item)
        {
            using (var db = new Database())
            {
                db.Phone.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
