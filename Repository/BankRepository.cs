using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBRP
{
    public static class BankRepository
    {
        public static void AddNewBankAccount(BankAccount item)
        {
            using (var db = new Database())
            {
                db.BankAccounts.Add(item);
                db.SaveChanges();
            }
        }

        public static BankAccount GetAccountByCardNumber(long cardNumber)
        {
            using (var db = new Database())
            {
                return db.BankAccounts.FirstOrDefault(i => i.CardNumber == cardNumber);
            }
        }

        public static BankAccount GetAccountById(int id)
        {
            using (var db = new Database())
            {
                return db.BankAccounts.Single(i => i.Id == id);
            }
        }

        public async static Task<List<BankAccount>> GetAccountsByRegisteredOwnerAsync(int id)
        {
            using (var db = new Database())
            {
                return await Task.Run(() =>
                {
                    return db.BankAccounts.Where(i => i.RegisterOwnerId == id).ToList();
                });
            }
        }
       
        public static async void UpdateAsync(BankAccount item)
        {
            using (var db = new Database())
            {
                db.BankAccounts.Attach(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }

        }
    }
}
