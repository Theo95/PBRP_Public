using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public static class FactionRepository
    {
        public static async Task<List<Faction>> GetAllFactions()
        {
            using (var db = new Database())
            {
                return await Task.Run(() => db.Factions.ToList());
            }
        }
        public static Faction GetFactionById(int id)
        {
            using (var db = new Database())
            {
                return db.Factions.Single(f => f.Id == id);
            }
        }

        public async static Task UpdateAsync(Faction item)
        {
            using (var db = new Database())
            {
                db.Factions.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
