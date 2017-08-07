using PBRP.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP.Logs
{
    public class AttackLogRepository
    {
        public async static void AddNew(AttackLog item)
        {
            using (var db = new LogDb())
            {
                db.Deaths.Add(item);
                await db.SaveChangesAsync();
            }
        }

        public async static Task AddNewAsync(AttackLog item)
        {
            using (var db = new LogDb())
            {
                db.Deaths.Add(item);
                await db.SaveChangesAsync();
            }
        }

        public async static void UpdateAsync(AttackLog item)
        {
            using (var db = new LogDb())
            {
                db.Deaths.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
