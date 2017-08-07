using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public static class ShopItemRepository
    {
        public async static void AddNewShopItem(ShopItem item)
        {
            using (var db = new Database())
            {
                db.ShopItems.Add(item);
                await db.SaveChangesAsync();
            }
        }

        public static List<ShopItem> GetShopItemsByBusinessId(int id)
        {
            using (var db = new Database())
            {
                return db.ShopItems.Where(i => i.BusinessId == id).ToList();
            }
        }

       
        // --- Removes

        public async static void RemoveShopItem(ShopItem item)
        {
            using (var db = new Database())
            {
                db.ShopItems.Attach(item);
                db.ShopItems.Remove(item);
                await db.SaveChangesAsync();
            }
        }

        public static async void UpdateAsync(ShopItem item)
        {
            using (var db = new Database())
            {
                db.ShopItems.Attach(item);
                db.Entry(item).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
