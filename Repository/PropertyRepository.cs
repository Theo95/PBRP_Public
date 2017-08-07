using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PBRP
{
    public static class PropertyRepository
    {
        public async static Task AddNewProperty(Property property)
        {
            using (var db = new Database())
            {
                db.Properties.Add(property);
                await db.SaveChangesAsync();
            }
        }

        public async static void DeleteProperty(Property property)
        {
            using (var db = new Database())
            {
                db.Properties.Attach(property);
                db.Properties.Remove(property);
                await db.SaveChangesAsync();
            }
        }

        public async static Task<List<Property>> GetAllProperties()
        {
            using (var db = new Database())
            {
                return await db.Properties.ToListAsync();
            }
        }

        public async static Task<List<Property>> GetAllHouses()
        {
            using (var db = new Database())
            {
                return await Task.Run(() => {
                    return db.Properties.Where(p => p.Type == PropertyType.Residential).ToList();
                });
            }
        }

        public async static Task<List<Business>> GetAllBusinesses()
        {
            using (var db = new Database())
            {
                return await db.Businesses.Where(p => p.Type == PropertyType.Commericial || p.Type == PropertyType.Service ||
                p.Type == PropertyType.Industrial).ToListAsync();
            }
        }

        public async static Task UpdateAsync(Property property)
        {
            using (var db = new Database())
            {
                db.Properties.Attach(property);
                db.Entry(property).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public static Task<List<Property>> GetAllPropertiesByTypeID(PropertyType typeID)
        {
            using (var db = new Database())
            {
                return db.Properties.Where(p => p.Type == typeID).ToListAsync();
            }
        }

        public static Property GetPropertyByPropertyName(string propertyName)
        {
            using (var db = new Database())
            {
                return db.Properties.Where(p => p.Name == propertyName).FirstOrDefault();
            }
        }

        public static Property GetPropertyByPropertyId(int ID)
        {
            using (var db = new Database())
            {
                return db.Properties.Where(p => p.Id == ID).FirstOrDefault();
            }
        }
    }
}