using System.Collections.Generic;
using System.Linq;

namespace PBRP
{
    public class PhoneContactRepository
    {
        public static async void AddPhoneContact(PhoneContact phone)
        {
            using (var db = new Database())
            {
                db.Entry(phone).State = System.Data.Entity.EntityState.Added;
                await db.SaveChangesAsync();
            }
        }

        public static PhoneContact GetPhoneContactById(int id)
        {
            using (var db = new Database())
            {
                return db.PhoneContact.FirstOrDefault(p => p.Id == id);
            }
        }

        public static PhoneContact GetPhoneContactByNumber(string number, Phone phone)
        {
            using (var db = new Database())
            {
                return db.PhoneContact.FirstOrDefault(pl => pl.Number == number && ((pl.IsSimContact && pl.SavedTo == phone.InstalledSim) || (!pl.IsSimContact && pl.SavedTo == phone.Id)));
            }
        }

        public static List<PhoneContact> GetAllContactsOnPhoneAndSim(int phoneId, int simId)
        {
            using (var db = new Database())
            {
                return db.PhoneContact.Where(pc => (pc.IsSimContact && pc.SavedTo == simId) || (!pc.IsSimContact && pc.SavedTo == phoneId)).ToList();
            }
        }

        public static List<PhoneContact> GetAllContactsOnSim(int simId)
        {
            using (var db = new Database())
            {
                return db.PhoneContact.Where(pc => pc.IsSimContact && pc.SavedTo == simId).ToList();
            }
        }

        public static async void UpdateAsync(PhoneContact item)
        {
            using (var db = new Database())
            {
                db.PhoneContact.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public static async void Delete(PhoneContact contact)
        {
            using (var db = new Database())
            {
                db.PhoneContact.Attach(contact);
                db.PhoneContact.Remove(contact);
                await db.SaveChangesAsync();
            }
        }
    }
}
