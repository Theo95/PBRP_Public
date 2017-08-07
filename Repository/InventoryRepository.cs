using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public static class InventoryRepository
    {
        public async static void AddNewInventoryItem(Inventory item)
        {
            using (var db = new Database())
            {
                db.InventoryItems.Add(item);
                await db.SaveChangesAsync();
            }
        }

        public static async Task<List<Inventory>> GetAllDroppedInventoryItems()
        {
            using (var db = new Database())
            {
                return await db.InventoryItems.Where(i => i.OwnerId == -1 && i.OwnerType == InventoryOwnerType.Dropped).ToListAsync();
            }
        }

        public static Inventory GetInventoryItemById(int id)
        {
            using (var db = new Database())
            {
                return db.InventoryItems.FirstOrDefault(i => i.Id == id);
            }
        }

        public static async Task<List<Inventory>> GetInventoryByOwnerIdAsync(int id, InventoryOwnerType type = InventoryOwnerType.Player)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.InventoryItems.Where(i => i.OwnerId == id && i.OwnerType == type).ToList();
                });
            }
        }

        public static List<Inventory> GetInventoryByOwnerId(int id, InventoryOwnerType type = InventoryOwnerType.Player)
        {
            using (var db = new Database())
            {
                return db.InventoryItems.Where(i => i.OwnerId == id && i.OwnerType == type).ToList();
            }
        }

        public static Inventory GetInventoryPhoneByIMEI(string value)
        {
            using (var db = new Database())
            {
                List<Inventory> phones = db.InventoryItems.Where(i => i.Value == value).ToList();
                return phones.FirstOrDefault().IsPhone() ? phones.FirstOrDefault() : null;
            }
        }

        public static Inventory GetInventoryItemOfTypeByValue(InventoryType type, string value)
        {
            using (var db = new Database())
            {
                return db.InventoryItems.FirstOrDefault(i => i.Type == type && i.Value == value);
            }
        }

        public async static Task<List<Inventory>> GetInventoryItemByName(string name)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.InventoryItems.Where(i => i.Name == name).ToList();
                });
            }
        }

        // --- Removes

        public async static void RemoveInventoryItem(Inventory item)
        {
            using (var db = new Database())
            {
                db.InventoryItems.Attach(item);
                db.InventoryItems.Remove(item);
                await db.SaveChangesAsync();
            }
        }

        public static async void UpdateAsync(Inventory item)
        {
            using (var db = new Database())
            {
                db.InventoryItems.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
