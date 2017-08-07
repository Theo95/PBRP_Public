using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public static class PrisonRepository {
        public async static Task AddNewSentence(Prison prison) {
            using (var db = new Database()) {
                db.Prison.Add(prison);
                await db.SaveChangesAsync();
            }
        }

        public async static Task RemoveSentence(Prison prison) {
            using (var db = new Database()) {
                db.Prison.Attach(prison);
                db.Prison.Remove(prison);
                await db.SaveChangesAsync();
            }
        }

        public static Prison GetPrisonSentenceByCharacterID(int ID) {
            using (var db = new Database()) {
                return db.Prison.FirstOrDefault(p => p.CharacterID == ID);
            }
        }

        public static Prison GetPrisonSentenceByJailorID(int ID) {
            using (var db = new Database()) {
                return db.Prison.FirstOrDefault(p => p.JailorID == ID);
            }
        }
    }
}
