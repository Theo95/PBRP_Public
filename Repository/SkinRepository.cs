using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public static class SkinRepository
    {
        public static int AddNewSkin(Skin skin)
        {
            using (var db = new Database())
            {
                db.Skin.Add(skin);
                db.SaveChanges();
                return skin.Id;
            }
        }
        public async static Task<List<Skin>> GetSkinsByOwnerId(int id)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.Skin.Where(s => s.OwnerId == id).ToList();
                });
            }
        }

        public async static Task<Skin> GetSkinById(int id)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.Skin.Where(s => s.Id == id).FirstOrDefault();
                });
            }
        }

        public static async void UpdateAsync(Skin item)
        {
            using (var db = new Database())
            {
                db.Skin.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
              
        }
    }
}
