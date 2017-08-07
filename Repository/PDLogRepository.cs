using System.Collections.Generic;
using System.Linq;

namespace PBRP
{
    public static class PDLogRepository  {
        public static void AddNewSignoutSheet(PDLog sheet) {
            using (var db = new Database()) {
                db.PDLog.Add(sheet);
                db.SaveChanges();
            }
        }

        public static List<PDLog> GetSignoutSheetsByCharacterName(string name) {
            using (var db = new Database()) {
                return db.PDLog.Where(s => s.SigneeName == name).ToList();
            }
        }

        public static List<PDLog> GetSignoutSheetsByProductName(string product) {
            using (var db = new Database()) {
                return db.PDLog.Where(s => s.ProductName == product).ToList();
            }
        }

        public static List<PDLog> GetSignoutSheetsByDate(string date) {
            using (var db = new Database()) {
                return db.PDLog.Where(s => s.DateSignedOut == date).ToList();
            }
        }

        public static List<PDLog> GetSignoutSheetsByTypeID(int id) {
            using (var db = new Database()) {
                return db.PDLog.Where(s => s.LogType == id).ToList();
            }
        }
    }
}
