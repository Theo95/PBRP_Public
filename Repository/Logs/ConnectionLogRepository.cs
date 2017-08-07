﻿using PBRP.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP.Logs
{
    public class ConnectionLogRepository
    {
        public async static void AddNew(ConnectionLog item)
        {
            using (var db = new LogDb())
            {
                db.Connections.Add(item);
                await db.SaveChangesAsync();
            }
        }

        public async static void UpdateAsync(ConnectionLog item)
        {
            using (var db = new LogDb())
            {
                db.Connections.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

    }
}
