using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP.Logs
{
    public enum MoneyTransferMethod
    {
        Cash = 0,
        ATMWithdraw = 1,
        BankWithdraw = 2,
        BankDeposit = 3,
        BankTransfer = 4,
        BusinessPurchase = 5,
        VehiclePurchaseCash = 6,
        VehicleDepositCash = 7,
        VehicleDepositBank = 8,
        VehiclePurchaseBank = 9,
        VehicleDepositRefund = 10,

    }
    [Table("cash")]
    public class CashLog
    {
        public CashLog(int moneyFrom, int moneyTo, long amount, MoneyTransferMethod transfer)
        {
            MoneyTransferredFrom = moneyFrom;
            MoneyTransferredTo = moneyTo;
            Amount = amount;
            TransferMethod = transfer;
            DateOfTransfer = Server.Date;
        }
        [Key]
        public int Id { get; set; }
        public int MoneyTransferredFrom { get; set; }
        public int MoneyTransferredTo { get; set; }
        public long Amount { get; set; }
        public MoneyTransferMethod TransferMethod { get; set; }
        public DateTime DateOfTransfer { get; set; }
    }
}
