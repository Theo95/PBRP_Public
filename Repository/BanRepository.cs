using System.Linq;

namespace PBRP
{
    public static class BanRepository
    {
        public async static void AddNewBan(Ban item)
        {
            using (var db = new Database())
            {
                db.Bans.Add(item);
                await db.SaveChangesAsync();
            }
        }

        public static Ban GetActiveBanBySocialClubName(string scName)
        {
            using (var db = new Database())
            {
                try
                {
                    Master master = db.Master.First(m => m.SocialClubName == scName);
                    Ban ban = db.Bans.FirstOrDefault(b => b.BannedId == master.Id);
                    return ban;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static Ban GetActiveBanByPlayerAccount(string playerName)
        {
            using (var db = new Database())
            {
                try
                {
                    Player player = db.Player.First(p => p.Username == playerName);
                    Ban ban = db.Bans.Single(b => b.BannedId == player.MasterId);
                    return ban;
                }
                catch { return null; }

            }
        }

        public async static void UpdateAsync(Ban item)
        {
            using (var db = new Database())
            {
                db.Bans.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public async static void RemoveAsync(Ban item)
        {
            using (var db = new Database())
            {
                db.Bans.Remove(item);
                await db.SaveChangesAsync();
            }
        }
    }
}
