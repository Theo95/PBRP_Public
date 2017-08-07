using PBRP.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP.Logs
{
    public class CashLogRepository
    {
        public async static void AddNew(CashLog item)
        {
            using (var db = new LogDb())
            {
                db.Cash.Add(item);
                await db.SaveChangesAsync();
            }
        }

        public async static void UpdateAsync(CashLog item)
        {
            using (var db = new LogDb())
            {
                db.Cash.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

    }
}
