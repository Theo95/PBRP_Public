using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public static class RankRepository
    {
        public static async Task<List<Rank>> GetRanksByFactionId(int id)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                 {
                     return db.Ranks.Where(r => r.FactionId == id).OrderBy(o => o.OrderIndex).ToList();
                 });

            }
        }

        public static Rank GetRankByFactionAndId(int faction, int id)
        {
            using (var db = new Database())
            {
                try
                {
                    return db.Ranks.Single(r => r.FactionId == faction && r.OrderIndex == id);
                }
                catch { return null; }
            }
        }

        public static async Task UpdateAsync(Rank item)
        {
            using (var db = new Database())
            {
                db.Ranks.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
              
        }
    }
}
