using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public static class VehicleRepository
    {
        public async static void AddNewVehicle(Vehicle vehicle)
        {
            using (var db = new Database())
            {
                db.Vehicles.Add(vehicle);
                await db.SaveChangesAsync();
            }
        }
        public static async Task<List<Vehicle>> GetAllVehicleData()
        {
            using (var db = new Database())
            {
                return await Task.Run(() => db.Vehicles.ToList());
            }
        }

        public static Vehicle GetVehicleById(int id)
        {
            using (var db = new Database())
            {
                return db.Vehicles.FirstOrDefault(v => v.Id == id);
            }
        }

        public async static Task<List<Vehicle>> GetAllVehiclesByFactionId(int id)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.Vehicles.Where(v => v.FactionId == id).ToList();
                });
            }
        }

        public async static Task<List<Vehicle>> GetAllVehiclesByLicensePlate(string license)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.Vehicles.Where(v => v.LicensePlate == license).ToList();
                });
            }
        }

        public async static Task<List<Vehicle>> GetAllVehiclesByOwnerId(int id)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.Vehicles.Where(v => v.OwnerId == id).ToList();
                });
            }
        }


        public static async void UpdateAsync(Vehicle item)

        {
            using (var db = new Database())
            {
                db.Vehicles.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }

        }
    }
}
