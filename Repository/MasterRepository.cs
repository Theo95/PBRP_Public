using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public static class MasterRepository
    {
        public async static Task<Master> GetMasterDataByName(string masterName)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.Master.FirstOrDefault(m => m.Username == masterName);
                });

            }
        }

        public async static Task<Master> GetMasterDataByIdAsync(int id)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.Master.FirstOrDefault(m => m.Id == id);
                });

            }
        }

        public static Master GetMasterDataById(int id)
        {
            using (var db = new Database())
            {
                return db.Master.FirstOrDefault(m => m.Id == id);
            }
        }

        public async static Task<Master> GetMasterDataByEmail(string emailAddress)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.Master.FirstOrDefault(m => m.Email == emailAddress);
                });

            }
        }

        public static async void UpdateAsync(Master item)
        {
            using (var db = new Database())
            {
                db.Master.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
