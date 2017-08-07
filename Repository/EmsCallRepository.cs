using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP
{
    public class EmsCallRepository
    {
        public async static void AddEmsCall(EmsCall call)
        {
            using (var db = new Database())
            {
                db.EmsCall.Add(call);
                await db.SaveChangesAsync();
            }
        }

        public static EmsCall getEmsCallById(int id)
        {
            using (var db = new Database())
            {
                try
                {
                    return db.EmsCall.Where(i => i.id == id).First();
                }
                catch { return null; }
            }
        }

        public static List<EmsCall> getRangeOfEmsCallsDesc(int offset)
        {
            using (var db = new Database())
            {
                try
                {
                    return db.EmsCall.OrderByDescending(p => p.CallTime).Skip(offset).Take(20).ToList();
                }
                catch { return null; }
            }
        }

        public static List<EmsCall> getAll()
        {
            using (var db = new Database())
            {
                try
                {
                    Console.WriteLine("We got here");
                    List<EmsCall> test = db.EmsCall.OrderByDescending(p => p.CallTime).ToList();
                    Console.WriteLine(test[0].CallerNameGiven);
                    return test;
                }
                catch
                {
                    return null;
                }
            }
        }

    }
}