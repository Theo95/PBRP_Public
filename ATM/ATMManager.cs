using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using PBRP.Logs;
using System.Collections.Generic;
using System.Linq;

namespace PBRP
{
    public class ATMManager : Script
    {
        private static readonly List<ColShape> ATMEnterPoint = new List<ColShape>();
        public ATMManager()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
            API.onResourceStart += OnServerStart;
            API.onEntityEnterColShape += OnEntityEnterColShape;
            API.onEntityExitColShape += OnEntityLeaveColShape;
        }

        private void OnEntityLeaveColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                foreach (ColShape atm in ATMEnterPoint)
                {
                    if (colshape == atm)
                    {

                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "onLeaveATMCol");
                        break;
                    }
                }
            }
        }

        private void OnEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                foreach (ColShape atm in ATMEnterPoint)
                {
                    if (colshape == atm)
                    {

                        Player p = Player.PlayerData[API.getPlayerFromHandle(entity)];
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "onEnterATMCol");
                        break;
                    }
                }
            }
        }

        private void OnServerStart()
        {
            foreach(Vector3 pos in PlayerPositionAtATM.Keys)
            {
                ColShape atmEntry = API.createCylinderColShape(pos, 1.5f, 2f);
                ATMEnterPoint.Add(atmEntry);
            }
        }

        private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "ActivateATM")
            {
                foreach (Vector3 atm in PlayerPositionAtATM.Keys)
                {
                    if (sender.position.DistanceTo(atm) < 1.5f)
                    {
                        List<Vector3> offsets = PlayerPositionAtATM[atm];
                        sender.position = atm;
                        sender.rotation = offsets[0];

                        API.triggerClientEvent(sender, "onExecuteATM", offsets[1], offsets[2]);

                        Player.PlayerData[sender].InEvent = PlayerEvent.UsingATM;
                        API.playPlayerScenario(sender, "PROP_HUMAN_ATM");
                        return;
                    }
                }
            }
            else if (eventName == "chooseBankCard")
            {
                Player p = Player.PlayerData[sender];
                p.AccessingBank = -1;
                p.TransactionType = -1;

                p.AwaitingInventorySelection = InventoryType.BankCard;
                InventoryManager.UpdatePlayerInventory(p);
            }
            else if(eventName == "closeATM")
            {
                try
                {
                    Player p = Player.PlayerData[sender];
                    p.AccessingBank = -1;
                    p.TransactionType = -1;

                    p.InEvent = PlayerEvent.None;
                    API.stopPlayerAnimation(sender);
                }
                catch
                {
                    // ignored
                }
            }
            else if (eventName == "ATMCardSelected")
            {
                Player p = Player.PlayerData[sender];
                try
                {
                    BankAccount bankAccount = BankRepository.GetAccountByCardNumber(long.Parse(p.Inventory.First(i => i.Id == (int)arguments[0]).Value));

                    p.AccessingBank = bankAccount.Id;
                    p.TransactionType = -1;

                    API.triggerClientEvent(sender, "enterATMPin");
                    InventoryManager.HidePlayerInventory(p, true);
                }
                catch
                {
                    API.triggerClientEvent(sender, "invalidCardATM");
                }
            }
            else if(eventName == "atmValidatePin")
            {
                Player p = Player.PlayerData[sender];
                BankAccount bankAccount = BankRepository.GetAccountById(p.AccessingBank);

                Player bankOwner = PlayerRepository.GetPlayerDataById(bankAccount.RegisterOwnerId);
                if ((string)arguments[0] == bankAccount.Pin)
                {
                    API.triggerClientEvent(sender, "atmCorrectPin", bankOwner.Username.Roleplay(), bankAccount.CardNumber.ToString());
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "Booo");
                }
            }
            else if(eventName == "atmRequestBalance")
            {
                Player p = Player.PlayerData[sender];
                BankAccount bankAccount = BankRepository.GetAccountById(p.AccessingBank);

                API.triggerClientEvent(sender, "atmBalanceReturn", bankAccount.Balance.ToString("C0"));
            }
            else if(eventName == "atmWithdrawMoney")
            {
                Player p = Player.PlayerData[sender];
                BankAccount bankAccount = BankRepository.GetAccountById(p.AccessingBank);
                long value = long.Parse(arguments[0].ToString());
                if (value <= bankAccount.Balance)
                {
                    if(value <= 1000 && value > 0)
                    {
                        Inventory cashItem = Inventory.CreateCashInventoryItem(value);
                        cashItem.OwnerType = InventoryOwnerType.Player;
                        cashItem.OwnerId = p.Id;
                        if (!cashItem.AddToPlayer(p, true))
                        {
                            API.SendErrorNotification(sender, "You don't have enough space to withdraw this amount of cash.", 7);
                            return;
                        }

                        InventoryRepository.AddNewInventoryItem(cashItem);
                        p.Inventory.Add(cashItem);
                        bankAccount.Balance -= value;

                        CashLogRepository.AddNew(new CashLog(bankAccount.Id, p.Id, value, MoneyTransferMethod.ATMWithdraw));
                        BankRepository.UpdateAsync(bankAccount);

                        API.triggerClientEvent(sender, "atmWithdrawComplete", bankAccount.Balance);
                        
                    }
                    else { API.triggerClientEvent(sender, "atmWithdrawError", 3, "<span id='withdrawErrorMsg' style='color:#f00'>EXCEEDED DAILY LIMIT</span>");  }
                }
                else API.triggerClientEvent(sender, "atmWithdrawError", 2, "<span style='color:#f00'>Insufficient funds</span>");
            }
            else if(eventName == "atmChangePin")
            {
                Player p = Player.PlayerData[sender];
                BankAccount bankAccount = BankRepository.GetAccountById(p.AccessingBank);

                int value = int.Parse(arguments[0].ToString());
                if (value > 999 && value < 10000)
                {
                    string encPin = value.ToString();
                    bankAccount.Pin = encPin;

                    p.AccessingBank = -1;
                    p.TransactionType = -1;

                    BankRepository.UpdateAsync(bankAccount);

                    IEnumerable<Inventory> bankCards = p.Inventory.Where(e => e.Type == InventoryType.BankCard);
                    API.triggerClientEvent(sender, "showATMMenu", string.Join(",", bankCards.Select(e => e.Id).ToList()),
                        string.Join(",", bankCards.Select(e => e.Name).ToList()), string.Join(".", bankCards.Select(e => Inventory.GetInventoryImage[e.Type]).ToList()),
                        string.Join(",", bankCards.Select(e => e.Quantity).ToList()));
                }
            }
        }

        public static void OnATMCardSelected(Player p, Inventory inv)
        {
            try
            {
                BankAccount bankAccount = BankRepository.GetAccountByCardNumber(long.Parse(inv.Value));

                p.AccessingBank = bankAccount.Id;
                p.TransactionType = -1;

                API.shared.triggerClientEvent(p.Client, "enterATMPin");
                InventoryManager.HidePlayerInventory(p, true);
            }
            catch
            {
                API.shared.triggerClientEvent(p.Client, "invalidCardATM");
            }
        }

        public static Dictionary<Vector3, List<Vector3>> PlayerPositionAtATM = new Dictionary<Vector3, List<Vector3>>()
        {
            //{ new Vector3(154.950, 6641.938, 31.62891), new List<Vector3>() { new Vector3(0, 0, -49.62966), new Vector3(155.8008, 6642.84, 31.78489), new Vector3(156.0066, 6643.042, 31.69789) } }, // Fuel Station ATM 1
            { new Vector3(-96.83015, 6454.933, 31.45679), new List<Vector3>() { new Vector3(0, 0, 44.57663), new Vector3(-97.27374, 6455.351, 31.65679), new Vector3(-97.4795350463867, 6455.55307421875, 31.5697916870117) } }, //Left Bank ATM
            { new Vector3(174.9007, 6637.213, 31.5731), new List<Vector3>() { new Vector3(0, 0, 44.11382), new Vector3(174.1872, 6637.835, 31.78309), new Vector3(173.98137956543, 6638.03744921875, 31.6960944061279) } }, //Gas Station other
            { new Vector3(-95.12697, 6456.741, 31.45732), new List<Vector3>() { new Vector3(0, 0, 45.51324), new Vector3(-95.49973, 6457.076, 31.66732), new Vector3(-95.7055253417969, 6457.278171875, 31.5803164367676) } }, //Right Bank ATM
            { new Vector3(155.4139, 6642.384, 31.61554), new List<Vector3>() { new Vector3(0, 0, -45.04911), new Vector3(155.7905, 6642.842, 31.73554), new Vector3(156.0765, 6643.122, 31.64854) } }, //Gas Station Main
            { new Vector3(-133.6448, 6365.838, 31.47529), new List<Vector3>() { new Vector3(0, 0, -43.11412), new Vector3(-133.0434, 6366.51, 31.61029), new Vector3(-132.77, 6366.802, 31.52329) } }, //Cool atm man
            { new Vector3(-283.5352, 6225.64, 31.49426), new List<Vector3>() { new Vector3(0, 0, -44.13143), new Vector3(-283.1049, 6226.058, 31.64076), new Vector3(-282.8263, 6226.345, 31.55376) } }, //BarberATM) }
            {new Vector3(-387.438, 6045.493, 31.50012), new List<Vector3>() { new Vector3(0, 0, -45.12672), new Vector3(-386.8577, 6046.044, 31.62012), new Vector3(-386.5742, 6046.326, 31.53312) } }
        };

    }
}
