using System.ComponentModel.DataAnnotations.Schema;

namespace PBRP
{
    public enum BankAccountType
    {
        Current = 0,
        Savings = 1,
        Business = 2
    }
    public enum BankAccountLockedType
    {
        FailedPin = 0,
        FrozenAssetsPolice = 1,
        FrozenAssetsGovernment = 2,
        AdminLocked = 3,
        Fraud = 4
    }
    [Table("bankaccount")]
    public class BankAccount
    {
        public int Id { get; set; }
        public int RegisterOwnerId { get; set; }
        public long Balance { get; set; }
        public string Pin { get; set; }
        public bool Locked { get; set; }
        public long CardNumber { get; set; }
        public BankAccountType Type { get; set; }
        public int FailedPinAttempts { get; set; }
        public BankAccountLockedType LockedType { get; set; }
    }
}
