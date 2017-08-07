using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP.Logs
{
    public class CommandLogRepository
    {
        public async static void AddNew(CommandLog item)
        {
            using (var db = new LogDb())
            {
                db.Commands.Add(item);
                await db.SaveChangesAsync();
            }
        }

        public async static void UpdateAsync(CommandLog item)
        {
            using (var db = new LogDb())
            {
                db.Commands.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
