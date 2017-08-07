using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public static class PlayerRepository
    {
        public static async void AddNewPlayerAsync(Player player)
        {
            using (var db = new Database())
            {
                db.Player.Add(player);
                await db.SaveChangesAsync();
            }
        }

        public static int AddNewPlayer(Player player)
        {
            using (var db = new Database())
            {
                db.Player.Add(player);
                db.SaveChanges();
                return player.Id;
            }
        }

        public async static Task<Player> GetPlayerDataByNameAsync(string playerName)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                 {
                     return db.Player.FirstOrDefault(p => p.Username == playerName);
                 });

            }
        }

        public static int GetPlayersWithUsernameCount(string playerName)
        {
            using (var db = new Database())
            {
                return db.Player.Count(p => p.Username == playerName);
            }
        }

        public static Player GetPlayerDataByName(string playerName)
        {
            using (var db = new Database())
            {
                return db.Player.FirstOrDefault(p => p.Username == playerName);
            }
        }

        public async static Task<List<Player>> GetAllPlayerDataByMasterAccount(Master master)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.Player.Where(p => p.MasterId == master.Id).ToList();
                });
            }
        }

        public static Player GetPlayerDataById(int id)
        {
            using (var db = new Database())
            {
                
                return db.Player.FirstOrDefault(p => p.Id == id);
            }
        }

        public static async void UpdateAsync(Player item)
        {
            using (var db = new Database())
            {
                db.Player.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
              
        }
    }
}
