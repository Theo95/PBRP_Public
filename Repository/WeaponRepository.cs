using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public class WeaponRepository
    {
        public async static Task<int> AddNew(Weapon weapon)
        {
            using (var db = new Database())
            {
                db.Weapons.Add(weapon);
                await db.SaveChangesAsync();
                return weapon.Id;
            }
        }
        public async static Task<List<Weapon>> GetAllWeaponsByPlayerIdAsync(int id)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.Weapons.Where(w => w.CurrentOwnerId == id).ToList();
                });
            }
        }

        public static List<Weapon> GetAllDroppedWeapons ()
        {
            using (var db = new Database())
            {
                return db.Weapons.Where(w => w.CurrentOwnerId == -1).ToList();
            }
        }

        public static List<Weapon> GetAllWeaponsByPlayerId(int id)
        {
            using (var db = new Database())
            {
                return db.Weapons.Where(w => w.CurrentOwnerId == id).ToList();
            }
        }

        public static async void UpdateAsync(Weapon item)
        {
            using (var db = new Database())
            {
                db.Weapons.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public static async void UpdateAllAsync(List<Weapon> playerWeapons)
        {
            using (var db = new Database())
            {
                foreach(Weapon w in playerWeapons)
                {
                    db.Weapons.Attach(w);
                    db.Entry(w).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async static void RemoveAsync(Weapon item)
        {
            using (var db = new Database())
            {
                db.Weapons.Attach(item);
                db.Weapons.Remove(item);
                await db.SaveChangesAsync();
            }
        }

    }
}
