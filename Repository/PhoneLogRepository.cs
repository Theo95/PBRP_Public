using System.Collections.Generic;
using System.Linq;

namespace PBRP
{
    public class PhoneLogRepository
    {
        public static void AddPhoneLog(PhoneLog phone)
        {
            using (var db = new Database())
            {
                db.PhoneLog.Add(phone);
                db.SaveChanges();
            }
        }

        public static PhoneLog GetPhoneLogById(int id)
        {
            using (var db = new Database())
            {
                return db.PhoneLog.FirstOrDefault(p => p.Id == id);
            }
        }

        public static List<PhoneLog> GetPhoneLogsByNumber(string number)
        {
            using (var db = new Database())
            {
                return db.PhoneLog.Where(pl => pl.NumberTo == number || pl.NumberFrom == number).OrderByDescending(pl => pl.SentAt).ToList();
            }
        }

        public static List<PhoneLog> GetPhoneLogsOfTypeByIMEI(PhoneLogType type, long number)
        {
            using (var db = new Database())
            {
                return db.PhoneLog.Where(pl => (pl.IMEITo == number || pl.IMEIFrom == number) && pl.Type == type).OrderByDescending(pl => pl.SentAt).ToList();
            }
        }

        public static List<PhoneLog> GetPhoneMessagesNumberToIMEI(string number, long number2)
        {
            using (var db = new Database())
            {
                return db.PhoneLog.Where(pl => ((pl.NumberTo == number && pl.IMEIFrom == number2) || (pl.NumberFrom == number && pl.IMEITo == number2)) && pl.Type == PhoneLogType.SMS).OrderBy(m => m.SentAt).ToList();
            }
        }

        public static async void UpdateAsync(PhoneLog item)
        {
            using (var db = new Database())
            {
                db.PhoneLog.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public static async void Remove(PhoneLog item)
        {
            using (var db = new Database())
            {
                db.PhoneLog.Attach(item);
                db.PhoneLog.Remove(item);
                await db.SaveChangesAsync();
            }
        }

        public static async void Remove(int item)
        {
            using (var db = new Database())
            {
                PhoneLog pl = db.PhoneLog.FirstOrDefault(p => p.Id == item);
                if (pl == null) return;
                db.PhoneLog.Attach(pl);
                db.PhoneLog.Remove(pl);
                await db.SaveChangesAsync();
            }
        }
    }
}
