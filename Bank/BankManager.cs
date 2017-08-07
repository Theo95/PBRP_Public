using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using PBRP.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PBRP
{
    public class BankManager : Script
    {
        public float SavingsInterest = 0.01f;
        public static Ped BankClerk { get; set; }
        private bool AlarmTriggered { get; set; }
        public ColShape BankAccessPoint { get; set; }

        int tick = 0;
        public BankManager()
        {
            API.onResourceStart += OnResourceStart;
            API.onClientEventTrigger += OnClientEventTrigger;
            API.onUpdate += OnUpdate;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            BankAccessPoint = API.createCylinderColShape(new Vector3(-113.4093, 6469.861, 31.62672), 1.5f, 1);
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                if (colshape == BankAccessPoint)
                {
                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "onLeaveBankCol");
                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                if (colshape == BankAccessPoint)
                { 
                    Player p = Player.PlayerData[API.getPlayerFromHandle(entity)];

                    if(p != null)
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "onEnterBankCol");
                }
            }
        }

        private async void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if(eventName == "ActivateBank")
            {
                Player p = Player.PlayerData[sender];
                sender.position = new Vector3(-113.4233, 6469.712, 31.62671);
                sender.rotation = new Vector3(0, 0, -20);

                API.sendNativeToPlayer(sender, Hash.TASK_PAUSE, sender, 240000000);

                API.triggerClientEvent(sender, "onExecuteBank");
                p.InEvent = PlayerEvent.UsingBank;

                API.triggerClientEvent(sender, "showBankOptions");
            }
            if (eventName == "accessAccount")
            {
                Player p = Player.PlayerData[sender];
                p.AccessingBank = -1;
                p.TransactionType = -1;

                p.AwaitingInventorySelection = InventoryType.BankCard;

                InventoryManager.UpdatePlayerInventory(p);
            }
            else if (eventName == "newBankAccountChoice")
            {
                Player p = Player.PlayerData[sender];
                p.AccessingBank = -1;
                p.TransactionType = -1;

                if ((BankAccountType)arguments[0] == BankAccountType.Savings)
                {
                    API.ShowPopupMessage(sender, "Opening Savings Account", "In order to open a new savings account, you will need to make an intial minimum cash deposit of $5,000.", true);

                    p.AwaitingInventorySelection = InventoryType.Money;

                    InventoryManager.UpdatePlayerInventory(p);
                    return;
                } 
                else
                {
                    await CreateNewAccountByType(p, BankAccountType.Current, 0, "");                    
                }
            }
            else if (eventName == "createAccountPinConfirmed")
            {
                Player p = Player.PlayerData[sender];
                p.CreatingAccount.Pin = arguments[0].ToString();

                if (p.CreatingAccount != null)
                    BankRepository.AddNewBankAccount(p.CreatingAccount);
                if (p.CreatingAccountCard != null)
                {
                    p.CreatingAccountCard.AddToPlayer(p, true);
                    InventoryRepository.AddNewInventoryItem(p.CreatingAccountCard);
                }

                p.Inventory.Add(p.CreatingAccountCard);

                API.SendInfoNotification(sender, String.Format("You have successfully created a new {0}", p.CreatingAccount.Type == BankAccountType.Current ? "Current Account" : "Savings Account"), 6);

                p.CreatingAccount = null;
                p.CreatingAccountCard = null;
            }
            else if (eventName == "validatePin")
            {
                Player p = Player.PlayerData[sender];
                BankAccount bankAccount = BankRepository.GetAccountById(p.AccessingBank);

                if ((string)arguments[0] == bankAccount.Pin)
                {
                    API.triggerClientEvent(sender, "correctPinEntered");
                }
                else
                {
                    API.SendErrorNotification(sender, "You have entered the incorrect PIN. Attempts remaining: " + (3 - ++bankAccount.FailedPinAttempts).ToString());
                    if (bankAccount.FailedPinAttempts == 3)
                    {
                        bankAccount.Locked = true;
                        bankAccount.LockedType = BankAccountLockedType.FailedPin;
                    }
                    BankRepository.UpdateAsync(bankAccount);
                }
            }
            else if (eventName == "bankInputAction")
            {
                string title = "";
                string message = "";
                Player p = Player.PlayerData[sender];
                BankAccount bankAccount = BankRepository.GetAccountById(p.AccessingBank);
                p.TransactionType = (int)arguments[0];

                string last4Digits = bankAccount.CardNumber.ToString();
                last4Digits = last4Digits.Substring(last4Digits.Length - 4);

                switch (p.TransactionType)
                {
                    case 0:
                        API.ShowPopupMessage(sender, "Bank Balance", String.Format("Your balance is: {0}", bankAccount.Balance.ToString("C0")), true);
                        return;
                    case 1:
                        p.SelectedCardAccount = null;
                        p.SelectedCash = null;
                        p.AwaitingInventorySelection = InventoryType.Money;
                        API.SendInfoNotification(sender, "Select the cash you wish to deposit into the account ending in " + last4Digits, 10);

                        InventoryManager.UpdatePlayerInventory(p);
                        return;
                    case 2:
                        title = "Withdraw funds";
                        message = String.Format("Please enter the amount you wish to withdraw from account ending in {0}:", last4Digits);
                        break;
                    case 3:
                        title = "Transfer funds";
                        message = "Please enter the account number you wish to transfer funds to:";
                        break;
                    case 4:
                        title = "Change PIN number";
                        message = "Please enter the 4 digit pin you wish to change to below:";
                        break;
                    case 5:
                        API.ShowPopupPrompt(sender, "confirmReplacementCard", "Replacement Bank Card", "Are you sure you want to receive a replacement bank card?", "", "", true);
                        return;
                }

                API.ShowInputPrompt(sender, "transactionInputReceived", title, message, "", "", true);
            }
            else if (eventName == "transactionInputReceived")
            {
                Player p = Player.PlayerData[sender];
                BankAccount bankAccount = BankRepository.GetAccountById(p.AccessingBank);
                if ((int)arguments[0] == 1)
                {
                    long value = 0;
                    try
                    {
                        value = long.Parse(arguments[1].ToString());
                    }
                    catch
                    {
                        var message = $"<span style='color:#f00'>Error: Value is invalid.</span><br />{arguments[3]}";
                        API.ShowInputPrompt(sender, "transactionInputReceived", arguments[2].ToString(), message, "", "", true);
                        return;
                    }


                    switch (p.TransactionType)
                    {
                        case 2:
                            if (bankAccount.Balance >= value && value > 0 && value <= 150001)
                            {
                                Inventory cashItem = Inventory.CreateCashInventoryItem(value);
                                cashItem.OwnerType = InventoryOwnerType.Player;
                                cashItem.OwnerId = p.Id;
                                if(!cashItem.AddToPlayer(p, true))
                                {
                                    API.SendErrorNotification(sender, "You don't have enough space to withdraw this amount of cash.", 7);
                                    return;
                                }

                                InventoryRepository.AddNewInventoryItem(cashItem);
                                p.Inventory.Add(cashItem);
                                CashLogRepository.AddNew(new CashLog(bankAccount.Id, p.Id, value, MoneyTransferMethod.BankWithdraw));
                                bankAccount.Balance -= value;

                                API.SendInfoNotification(p.Client, $"Your new bank balance is {bankAccount.Balance}", 10);
                            }
                            else
                            {
                                API.ShowInputPrompt(sender, "transactionInputReceived", arguments[2].ToString(), "<span style='color:#f00'>Error: Insufficient funds.</span><br />Please enter the amount you wish to withdraw from this account:", "", "", true);
                            }
                            break;
                        case 3:
                            if ((int)arguments[0] == 1)
                            {
                                try
                                {
                                    BankAccount targetAccount = BankRepository.GetAccountByCardNumber(long.Parse(arguments[1].ToString()));
                                    string last4Digits = arguments[1].ToString();
                                    last4Digits = last4Digits.Substring(last4Digits.Length - 4);

                                    p.TransactionType = targetAccount.Id;
                                    API.ShowInputPrompt(sender, "transferAmountToAccount", "Transfer funds to account",
                                        $"Please enter the amount you wish to transfer to the account ending in {last4Digits}", "", "", true);
                                }
                                catch
                                {
                                    API.ShowInputPrompt(sender, "transactionInputReceived", arguments[2].ToString(), "<span style='color:#f00'>Error: Invalid card number</span><br/>Please enter the account number you wish to transfer funds to:", "", "", true);
                                }

                            }
                            break;
                        case 4:
                            if (value > 999 && value < 10000)
                            {
                                string encPin = value.ToString();
                                bankAccount.Pin = encPin;

                                API.SendInfoNotification(sender, "Your new PIN is " + value, 8);
                            }
                            break;
                    }
                    API.triggerClientEvent(sender, "reenableCursor");
                    BankRepository.UpdateAsync(bankAccount);
                    PlayerRepository.UpdateAsync(p);
                }
                else API.triggerClientEvent(sender, "reenableCursor");
            }
            else if (eventName == "confirmReplacementCard")
            {
                if((int)arguments[0] == 1) {
                    bool uniqueNumber = false;
                    Player p = Player.PlayerData[sender];
                    BankAccount bankAccount = BankRepository.GetAccountById(p.AccessingBank);

                    while (!uniqueNumber)
                    {
                        string newCardNumber = await CreateCardNumber(16);
                        Inventory inv = InventoryRepository.GetInventoryItemOfTypeByValue(InventoryType.BankCard, newCardNumber);
                        if (inv == null)
                        {
                            Inventory newCard = new Inventory()
                            {
                                OwnerId = p.Id,
                                Value = newCardNumber,
                                Type = InventoryType.BankCard,
                                Name = "PaletoCard",
                                Quantity = 1
                            };
                            InventoryRepository.AddNewInventoryItem(newCard);
                            p.Inventory.Add(newCard);
                            uniqueNumber = true;
                        }
                    }

                    API.SendInfoNotification(sender, "You have been given a new bank card.");
                    API.triggerClientEvent(sender, "reenableCursor");
                    // Ask if they want the old card destroyed?
                }
            }
            else if (eventName == "transferAmountToAccount")
            {
                Player p = Player.PlayerData[sender];

                BankAccount bankAccount = BankRepository.GetAccountById(p.AccessingBank);
                BankAccount targetAccount = BankRepository.GetAccountById(p.TransactionType);
                long value = 0;

                try { value = long.Parse(arguments[1].ToString()); } catch {
                    var message = $"<span style='color:#f00'>Error: Value is invalid.</span><br />{arguments[3]}";
                    API.triggerClientEvent(sender, "confirmInput", "transactionInputReceived", arguments[2], message);
                    return;
                }

                if (bankAccount.Balance <= value) return;
                bankAccount.Balance -= value;
                targetAccount.Balance += value;

                API.sendChatMessageToPlayer(sender, "Your new balance is " + bankAccount.Balance);

                API.triggerClientEvent(sender, "reenableCursor");

                BankRepository.UpdateAsync(bankAccount);
                BankRepository.UpdateAsync(targetAccount);
            }
            else if (eventName == "validateIDGiven")
            {
                if ((int) arguments[0] == 1)
                {
                }
            }
            else if(eventName == "onBankLeave")
            {
                Player p = Player.PlayerData[sender];
                p.InEvent = PlayerEvent.None;

                if (p.AwaitingInventorySelection == InventoryType.BankCard) p.AwaitingInventorySelection = null;

                API.sendNativeToPlayer(sender, Hash.TASK_PAUSE, sender, -1);
                API.triggerClientEvent(sender, "onEnterBankCol", ((char)p.MasterAccount.KeyInteract).ToString(), p.MasterAccount.KeyInteract);
            }
        }

        public static void OnBankDeposit(Player p, Inventory inv)
        {
            BankAccount bankAccount = BankRepository.GetAccountById(p.AccessingBank);
            bankAccount.Balance += long.Parse(inv.Value);

            InventoryRepository.RemoveInventoryItem(inv);
            p.Inventory.Remove(inv);

            CashLogRepository.AddNew(new CashLog(p.Id, bankAccount.Id, long.Parse(inv.Value), MoneyTransferMethod.BankDeposit));

            API.shared.SendInfoNotification(p.Client,
                $"Your new bank balance is {bankAccount.Balance}", 10);
            BankRepository.UpdateAsync(bankAccount);
            InventoryManager.RefreshPlayerInventory(p);
            InventoryManager.HidePlayerInventory(p, true);
        }

        public static async void OnSavingsDepositPlaced(Player p, Inventory inv)
        {
            if (long.Parse(inv.Value) >= 5000)
            {
                await CreateNewAccountByType(p, BankAccountType.Savings, long.Parse(inv.Value), "");

                p.AwaitingInventorySelection = null;
            }
            else API.shared.SendErrorNotification(p.Client, "Insufficent amount of money selected.", 7);
        }

        private void OnUpdate()
        {
            if(tick == 60)
            {
                List<Player> playerData = Player.PlayerData.Values.ToList();
                foreach (Player p in playerData)
                {
                    if (p.Client.position.DistanceTo(BankClerk.position) < 6.0)
                    {
                        if (p.Client.currentWeapon != WeaponHash.Unarmed)
                        {
                            if (p.Client.position.DistanceTo(BankClerk.position) < 2.0)
                            {
                                API.sendNativeToAllPlayers(Hash.TASK_HANDS_UP, BankClerk, -1);
                            }
                            if (!AlarmTriggered)
                            {
                                BankClerk.movePosition(new Vector3(-111.8503, 6471.595, 31.62671), 1);
                                API.sendNativeToAllPlayers(Hash.TASK_COWER, BankClerk, -1);
                                //API.sendNativeToAllPlayers(Hash.START_ALARM, "PALETO_BAY_SCORE_ALARM", 1);
                                AlarmTriggered = true;
                            }
                        }
                    }
                }
                tick = 0;
            }
            tick++;
        }

        private void OnResourceStart()
        {
            BankClerk = API.createPed(PedHash.BankmanCutscene, new Vector3(-112.1975, 6471.044, 31.62671), 135.1624f);
            API.createObject(277179989, new Vector3(-113.2013, 6470.85693, 31.9489994), new Vector3(-53.6196251, -4.76886415, -4.09004593));
            API.createObject(2088900873, new Vector3(-113.201347, 6470.85693, 31.9489994), new Vector3(-29.2464581, -178.760132, -179.999945));
        }

        public static void OnBankCardSelected(Player player, Inventory inv)
        {
            BankAccount bankAccount = null;
            try
            {
                bankAccount = BankRepository.GetAccountByCardNumber(long.Parse(inv.Value));
                player.AccessingBank = bankAccount.Id;
            }
            catch { API.shared.SendErrorNotification(player.Client, "That card is not associated with any bank account."); return; }

            if(bankAccount.Locked)
            {
                API.shared.SendErrorNotification(player.Client, String.Format("This bank account is locked due to repeated failed PIN attempts.", 4));
                API.shared.SendWarningNotification(player.Client, String.Format("Select a valid drivers license with the name matching the on the account.", 10));

                player.AccessingBank = bankAccount.Id;
                player.AwaitingInventorySelection = InventoryType.DriversLicense;
                return;
            }

            InventoryManager.HidePlayerInventory(player, true);
            API.shared.triggerClientEvent(player.Client, "playerEnterPinBank", 1);
        }

        public static void OnLicenseSelected(Player p, Inventory inv)
        {
            BankAccount bankAccount = BankRepository.GetAccountById(p.AccessingBank);
            
            //Do license stuff

        }

        private static async Task CreateNewAccountByType(Player p, BankAccountType type, long balance, string pin)
        {
            string newCardNumber = await CreateCardNumber(16);

            Inventory newCard = new Inventory()
            {
                OwnerId = p.Id,
                Value = newCardNumber,
                Type = InventoryType.BankCard,
                OwnerType = InventoryOwnerType.Player,
                Name = "PaletoCard",
                Quantity = 1
            };

            if (!newCard.AddToPlayer(p))
            {
                API.shared.SendErrorNotification(p.Client, "No space in your inventory to receive new bank card", 7);
                p.CreatingAccount = null;
                p.CreatingAccountCard = null;
                API.shared.triggerClientEvent(p.Client, "showBankOptions");
                return;
            }

            BankAccount newAccount = new BankAccount()
            {
                RegisterOwnerId = p.Id,
                Balance = balance,
                Pin = pin,
                Locked = false,
                CardNumber = long.Parse(newCardNumber),
                Type = type
            };

            API.shared.triggerClientEvent(p.Client, "playerEnterPinBank", 2);
            p.CreatingAccountCard = newCard;

            p.CreatingAccount = newAccount;
        }

        private static async Task<string> CreateCardNumber(int length)
        {
            return await Task.Run(() =>
            {
                string[] prefixs = { "51", "52", "53", "54", "58", "37", "46", "49" };
                string ccnumber = prefixs[new Random().Next(0, prefixs.Length - 1)];
                bool unique = false;

                while (!unique)
                {
                    while (ccnumber.Length < (length - 1))
                    {
                        double rnd = (new Random().NextDouble() * 1.0f - 0f);

                        ccnumber += Math.Floor(rnd * 10);

                        Thread.Sleep(20);
                    }


                    var reversedCCnumberstring = ccnumber.ToCharArray().Reverse();

                    var reversedCCnumberList = reversedCCnumberstring.Select(c => Convert.ToInt32(c.ToString()));

                    int sum = 0;
                    int pos = 0;
                    int[] reversedCCnumber = reversedCCnumberList.ToArray();

                    while (pos < length - 1)
                    {
                        int odd = reversedCCnumber[pos] * 2;

                        if (odd > 9)
                            odd -= 9;

                        sum += odd;

                        if (pos != (length - 2))
                            sum += reversedCCnumber[pos + 1];

                        pos += 2;
                    }

                    // calculate check digit
                    int checkdigit =
                        Convert.ToInt32((Math.Floor((decimal)sum / 10) + 1) * 10 - sum) % 10;

                    

                    ccnumber += checkdigit;

                    if (BankRepository.GetAccountByCardNumber(long.Parse(ccnumber)) == null)
                        unique = true;
                }
                return ccnumber;
            });

            
        }

        public static bool IsValidCreditCardNumber(string creditCardNumber)
        {
            try
            {
                var reversedNumber = creditCardNumber.ToCharArray().Reverse();

                int mod10Count = 0;
                for (int i = 0; i < reversedNumber.Count(); i++)
                {
                    int augend = Convert.ToInt32(reversedNumber.ElementAt(i).ToString());

                    if (((i + 1) % 2) == 0)
                    {
                        string productstring = (augend * 2).ToString();
                        augend = 0;
                        for (int j = 0; j < productstring.Length; j++)
                        {
                            augend += Convert.ToInt32(productstring.ElementAt(j).ToString());
                        }
                    }
                    mod10Count += augend;
                }

                if ((mod10Count % 10) == 0)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
