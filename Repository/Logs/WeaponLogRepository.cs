using PBRP.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP.Logs
{
    public class WeaponLogRepository
    {
        public async static void AddNew(WeaponLog item)
        {
            using (var db = new LogDb())
            {
                db.Weapons.Add(item);
                await db.SaveChangesAsync();
            }
        }

        public async static void UpdateAsync(WeaponLog item)
        {
            using (var db = new LogDb())
            {
                db.Weapons.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

    }
}
