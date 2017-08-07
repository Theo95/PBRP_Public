using System.Collections.Generic;
using System.Linq;

namespace PBRP
{
    public static class LicenseRepository
    {
        public async static void AddLicense(License license)
        {
            using (var db = new Database())
            {
                db.Licenses.Add(license);
                await db.SaveChangesAsync();
            }
        }
        public static List<License> GetPlayerLicenses(int playerID)
        {
            using(var db = new Database())
            {
                return db.Licenses.Where(i => i.PlayerId == playerID).ToList();
            }
        }

        

    }
}
