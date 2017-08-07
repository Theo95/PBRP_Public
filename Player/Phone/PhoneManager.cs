using CryptSharp;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PBRP
{
    public class PhoneManager : Script
    {
        private int minCount = 0;

        public PhoneManager()
        {
            API.onClientEventTrigger += OnClientBasePhoneEventTrigger;
            API.onClientEventTrigger += OnClientMessageEventTrigger;
            ServerInit.OnMinute += OnMinute;
        }

        private void OnMinute()
        {
            if(minCount % 5 == 0)
            {
                List<Player> playerData = Player.PlayerData.Values.ToList();
                foreach (Player p in playerData)
                {
                    if(p.PrimaryPhone != null)
                    {
                        API.triggerClientEvent(p.Client, "phoneUpdateClock", ServerInit.ServerHour, ServerInit.ServerMinute);
                    }
                }
            }
        }

        private void OnClientBasePhoneEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "PhoneInsertSimCard")
            {
                Player p = Player.PlayerData[sender];

                if (p.PrimaryPhone.InstalledSim != -1)
                {
                    API.ShowPopupPrompt(sender, "RemoveCurrentSimCard", "Remove SIM?", "Are you sure you want to remove your current sim card from this device?");
                }
                else
                {
                    if (p.AccessingBank <= 0)
                    {
                        if (p.Inventory.Where(e => e.Type == InventoryType.SimCard).Select(e => e.Id).Any())
                        {
                            API.triggerClientEvent(sender, "selectSimCard", string.Join(",", p.Inventory.Where(e => e.Type == InventoryType.SimCard).Select(e => e.Id).ToList()),
                            string.Join(",", p.Inventory.Where(e => e.Type == InventoryType.SimCard).Select(e => e.Name).ToList()),
                            string.Join(".", p.Inventory.Where(e => e.Type == InventoryType.SimCard).Select(e => Inventory.GetInventoryImage[e.Type]).ToList()),
                            string.Join(",", p.Inventory.Where(e => e.Type == InventoryType.SimCard).Select(e => e.Quantity).ToList()));
                        }
                        API.SendErrorNotification(sender, "You don't have any sim card on you.", 8);
                    }
                }
            }
            else if (eventName == "onSimCardSelected")
            {
                Player p = Player.PlayerData[sender];

                SimCard sim = SimCardRepository.GetSimCardByNumber(p.Inventory.First(e => e.Id == (int)arguments[0]).Value);

                if (sim != null)
                {
                    Inventory inv = p.Inventory.FirstOrDefault(i => i.Value == sim.Number);

                    if (inv != null)
                    {
                        p.Inventory.Remove(inv);

                        p.PrimaryPhone.InstalledSim = sim.Id;

                        InventoryRepository.UpdateAsync(inv);
                        PlayerRepository.UpdateAsync(p);
                        PhoneRepository.UpdateAsync(p.PrimaryPhone);

                        API.SendInfoNotification(sender, "You have successfully installed a new sim card.");
                        API.triggerClientEvent(sender, "PhoneSimInstalled");
                    }
                }
            }
            else if (eventName == "RemoveCurrentSimCard")
            {
                if ((int)arguments[0] == 1)
                {
                    Player p = Player.PlayerData[sender];
                    SimCard currentSim = SimCardRepository.GetSimCardById(p.PrimaryPhone.InstalledSim);
                    Inventory inv = InventoryRepository.GetInventoryItemOfTypeByValue(InventoryType.SimCard, currentSim.Number);
                    if (!inv.AddToPlayer()) { API.SendWarningNotification(sender, "You don't have space in your inventory for a sim card"); return; }

                    p.PrimaryPhone.InstalledSim = -1;

                    inv.OwnerId = p.Id;
                    inv.OwnerType = InventoryOwnerType.Player;
                    p.Inventory.Add(inv);

                    InventoryRepository.UpdateAsync(inv);
                    PlayerRepository.UpdateAsync(p);
                    PhoneRepository.UpdateAsync(p.PrimaryPhone);

                    API.SendInfoNotification(sender, "You have removed the sim card from your phone.");
                }
            }
            else if (eventName == "LoadPhoneHomeScreen")
            {
                Player p = Player.PlayerData[sender];
                p.PrimaryPhone.Home(sender);

            }
            else if (eventName == "ShowSettingsInfoPage")
            {
                Player p = Player.PlayerData[sender];
                SimCard currentSim = SimCardRepository.GetSimCardById(p.PrimaryPhone.InstalledSim);

                API.triggerClientEvent(sender, "LoadSettingsInfoPage", p.PrimaryPhone.IMEI.ToString(), currentSim.Number, currentSim.Credit.ToString("C0"));
            }
            else if (eventName == "PhoneMakeCall")
            {
                Player p = Player.PlayerData[sender];
                SimCard simBeingCalled = SimCardRepository.GetSimCardByNumber(arguments[0].ToString());
                SimCard playerSim = SimCardRepository.GetSimCardById(p.PrimaryPhone.InstalledSim);

                if(playerSim == null) { API.triggerClientEvent(sender, "PhoneCallConnection", (int)PhoneCallConnectTypes.SimNotInstalled); return; }

                if (simBeingCalled == null) { API.triggerClientEvent(sender, "PhoneCallConnection", (int)PhoneCallConnectTypes.NumberDoesntExist); return; }

                if(simBeingCalled.Number == playerSim.Number) { API.triggerClientEvent(sender, "PhoneCallConnection", (int)PhoneCallConnectTypes.InCall); return; }
                if(simBeingCalled.Number == "911") { EmergencyCallHandler.EmsCalled(p.Client, playerSim.Number); return; }
                Phone phoneCalled = PhoneRepository.GetPhoneBySim(simBeingCalled.Id);
                if (phoneCalled == null) { API.triggerClientEvent(sender, "PhoneCallConnection", (int)PhoneCallConnectTypes.SimNotInstalled); return; }

                API.triggerClientEvent(sender, "PhoneCallConnection", (int)PhoneCallConnectTypes.Connected);

                Inventory phoneInv = InventoryRepository.GetInventoryPhoneByIMEI(phoneCalled.IMEI.ToString());
                Player playerCalled = Player.PlayerData.Values.FirstOrDefault(ph => ph.Id == phoneInv.OwnerId);

                if (playerCalled == null) { API.triggerClientEvent(sender, "PhoneCallConnection", (int)PhoneCallConnectTypes.PlayerOffline); return; }

                if (!phoneCalled.PoweredOn) { API.triggerClientEvent(sender, "PhoneCallConnection", (int)PhoneCallConnectTypes.PhoneOff); return; }

                PhoneContact contactOnCalled = PhoneContactRepository.GetPhoneContactByNumber(arguments[0].ToString(), phoneCalled);

                if(contactOnCalled != null && contactOnCalled.IsBlocked) { API.triggerClientEvent(sender, "PhoneCallConnection", (int)PhoneCallConnectTypes.BlockedNumber); return; }

                if (phoneCalled.InCallWith != null) { API.triggerClientEvent(sender, "PhoneCallConnection", (int)PhoneCallConnectTypes.InCall); return; }

                if (!phoneCalled.IsPrimary) { InventoryManager.SetPhonePrimary(playerCalled.Client, phoneInv, true); }

                playerCalled.PrimaryPhone.InCallWith = p;
                p.PrimaryPhone.InCallWith = playerCalled;
                p.PrimaryPhone.IsCaller = true;
                playerCalled.PrimaryPhone.IsCaller = false;

                if(contactOnCalled != null)
                    API.triggerClientEvent(playerCalled.Client, "IncomingPhoneCall", contactOnCalled.Name);
                else
                    API.triggerClientEvent(playerCalled.Client, "IncomingPhoneCall", playerSim.Number);
                //API.sendNativeToPlayer(playerCalled.Client, Hash.PLAY_PED_RINGTONE, "Remote_Ring", playerCalled.Client, 1);
            }
            else if (eventName == "PhoneAnswerCall")
            {
                Player p = Player.PlayerData[sender];
                if (p.PrimaryPhone.InCallWith == null) return;
                SimCard callerSim = SimCardRepository.GetSimCardById(p.PrimaryPhone.InCallWith.PrimaryPhone.InstalledSim);

                if (callerSim == null) { API.triggerClientEvent(sender, "PhoneCallConnection", (int)PhoneCallConnectTypes.SimNotInstalled); return; }

                callerSim.Credit -= 1;

                p.PrimaryPhone.CallConnected = true;
                p.PrimaryPhone.InCallWith.PrimaryPhone.CallConnected = true;

                API.triggerClientEvent(p.PrimaryPhone.InCallWith.Client, "PhoneCallAnswered");
                API.triggerClientEvent(sender, "PhoneCallAnswered");
            }
            else if (eventName == "PhoneEndCall")
            {
                Player p = Player.PlayerData[sender];
                p.PrimaryPhone.Speakerphone = false;
                p.PrimaryPhone.MicMuted = false;

                SimCard endedSim = SimCardRepository.GetSimCardById(p.PrimaryPhone.InstalledSim);
                SimCard receiverSim = SimCardRepository.GetSimCardById(p.PrimaryPhone.InCallWith.PrimaryPhone.InstalledSim);

                PhoneLog callLog = new PhoneLog()
                {
                    IMEITo = p.PrimaryPhone.IsCaller ? p.PrimaryPhone.InCallWith.PrimaryPhone.IMEI : p.PrimaryPhone.IMEI,
                    IMEIFrom = p.PrimaryPhone.IsCaller ? p.PrimaryPhone.IMEI : p.PrimaryPhone.InCallWith.PrimaryPhone.IMEI,
                    NumberFrom = p.PrimaryPhone.IsCaller ? endedSim.Number : receiverSim.Number,
                    NumberTo = p.PrimaryPhone.IsCaller ? receiverSim.Number : endedSim.Number,
                    Duration = (int)arguments[0],
                    Type = PhoneLogType.Call,
                    Message = "",
                    SentAt = Server.Date.AddSeconds(-((int)arguments[0])),
                    Viewed = p.PrimaryPhone.CallConnected             
                };

                if (p.PrimaryPhone.CallConnected)
                    API.triggerClientEvent(p.PrimaryPhone.InCallWith.Client, "phoneTerminateCall");

                PhoneLogRepository.AddPhoneLog(callLog);

                p.PrimaryPhone.CallConnected = false;
                p.PrimaryPhone.InCallWith.PrimaryPhone.CallConnected = false;
                p.PrimaryPhone.InCallWith.PrimaryPhone.Speakerphone = false;
                p.PrimaryPhone.InCallWith.PrimaryPhone.MicMuted = false;
                p.PrimaryPhone.InCallWith.PrimaryPhone.InCallWith = null;
                p.PrimaryPhone.InCallWith = null;
                
            }
            else if(eventName == "PhoneToggleSpeaker")
            {
                Player p = Player.PlayerData[sender];
                p.PrimaryPhone.Speakerphone = !p.PrimaryPhone.Speakerphone;
            }
            else if(eventName == "PhoneToggleMic")
            {
                Player p = Player.PlayerData[sender];
                p.PrimaryPhone.MicMuted = !p.PrimaryPhone.MicMuted;
            }
            else if(eventName == "PhoneRecentCalls")
            {
                Player p = Player.PlayerData[sender];

                List<PhoneLog> recentCalls = PhoneLogRepository.GetPhoneLogsOfTypeByIMEI(PhoneLogType.Call, p.PrimaryPhone.IMEI);

                string callData = "";
                int count = 0;

                foreach (PhoneLog pl in recentCalls)
                {
                    PhoneContact pc;
                    if (pl.IMEIFrom == p.PrimaryPhone.IMEI)
                    {
                        pc = PhoneContactRepository.GetPhoneContactByNumber(pl.NumberTo, p.PrimaryPhone);
                        if (pc != null)
                        {
                            if(pc.Name.Length > 15) callData += string.Format("{0},outbound,", pc.Name.Substring(0, 13) + "...");
                            else callData += string.Format("{0},outbound,", pc.Name);
                        }
                        else callData += string.Format("{0},outbound,", pl.NumberTo);
                    }
                    else
                    {
                        pc = PhoneContactRepository.GetPhoneContactByNumber(pl.NumberFrom, p.PrimaryPhone);
                        if (pc != null)
                        {
                            if(pc.Name.Length > 15) callData += string.Format("{0},inbound,", pc.Name.Substring(0, 13) + "...");
                            else callData += string.Format("{0},inbound,", pc.Name);
                        }
                        else callData += string.Format("{0},inbound,", pl.NumberFrom);
                    }

                    if (pl.SentAt > Server.Date.AddDays(-1)) callData += pl.SentAt.ToString("HH:mm") + ",";
                    else if (pl.SentAt > Server.Date.AddDays(-7)) callData += pl.SentAt.ToString("dddd") + ",";
                    else callData += pl.SentAt.ToString("dd/MM/yyyy") + ",";

                    if (pl.Viewed) callData += "yes,";
                    else callData += "no,";
                    count += 4;
                }

                API.triggerClientEvent(sender, "populateRecentCalls", callData, count);
            }
            else if(eventName == "PhoneContactList")
            {
                Player p = Player.PlayerData[sender];

                SimCard playerSIM = SimCardRepository.GetSimCardById(p.PrimaryPhone.InstalledSim);
                List<PhoneContact> contactList;

                contactList = playerSIM != null ? PhoneContactRepository.GetAllContactsOnPhoneAndSim(p.PrimaryPhone.Id, playerSIM.Id) : PhoneContactRepository.GetAllContactsOnPhoneAndSim(p.PrimaryPhone.Id, -1);

                API.triggerClientEvent(sender, "populateContactList", string.Join(",", contactList.OrderBy(e => e.Name).Select(e => e.Name)), false);

            }
            else if(eventName == "PhoneFavouriteList")
            {
                Player p = Player.PlayerData[sender];

                SimCard playerSIM = SimCardRepository.GetSimCardById(p.PrimaryPhone.InstalledSim);
                List<PhoneContact> contactList;

                contactList = playerSIM != null ? PhoneContactRepository.GetAllContactsOnPhoneAndSim(p.PrimaryPhone.Id, playerSIM.Id) : PhoneContactRepository.GetAllContactsOnPhoneAndSim(p.PrimaryPhone.Id, -1);

                API.triggerClientEvent(sender, "populateContactList", string.Join(",", contactList.Where(e=>e.IsFavourite).OrderBy(e => e.Name).Select(e => e.Name)), true);
            }
            else if(eventName == "PhoneRecentCallSelect")
            {
                Player p = Player.PlayerData[sender];

                List<PhoneLog> recentCalls = PhoneLogRepository.GetPhoneLogsOfTypeByIMEI(PhoneLogType.Call, p.PrimaryPhone.IMEI);

                string recentCall = recentCalls[int.Parse(arguments[0].ToString())].IMEIFrom == p.PrimaryPhone.IMEI ?
                    recentCalls[int.Parse(arguments[0].ToString())].NumberTo : recentCalls[int.Parse(arguments[0].ToString())].NumberFrom;
                API.triggerClientEvent(sender, "redialRecentCall", recentCall);
            }
            else if(eventName == "PhoneAddNewContact")
            {
                sender.ToggleCursorLock(true);
            }
            else if(eventName == "PhoneCreateNewContact")
            {
                Player p = Player.PlayerData[sender];

                PhoneContact contact = new PhoneContact()
                {
                    Name = arguments[0].ToString(),
                    Number = arguments[1].ToString(),
                    Address1 = arguments[2].ToString(),
                    Address2 = arguments[3].ToString(),
                    Address3 = arguments[4].ToString(),
                    Notes = arguments[5].ToString(),
                    IsBlocked = false,
                    IsFavourite = false,
                    SavedTo = p.PrimaryPhone.Id,
                    IsSimContact = false
                };

                PhoneContactRepository.AddPhoneContact(contact);

                OnClientBasePhoneEventTrigger(sender, "PhoneContactList", new object { });
                sender.ToggleCursorLock(false);
            }
            else if(eventName == "PhoneContactSelected")
            {
                Player p = Player.PlayerData[sender];
                int ind = (int)arguments[0];
                bool isFavourite = (bool)arguments[1];
                SimCard playerSIM = SimCardRepository.GetSimCardById(p.PrimaryPhone.InstalledSim);
                List<PhoneContact> contactList;

                contactList = playerSIM != null ? PhoneContactRepository.GetAllContactsOnPhoneAndSim(p.PrimaryPhone.Id, playerSIM.Id) : PhoneContactRepository.GetAllContactsOnPhoneAndSim(p.PrimaryPhone.Id, -1);

                contactList = !isFavourite ? contactList.OrderBy(e => e.Name).ToList() : contactList.Where(e => e.IsFavourite).OrderBy(e => e.Name).ToList();

                API.triggerClientEvent(sender, "displayPhoneContact", contactList[ind].Id, contactList[ind].Name, contactList[ind].Number, contactList[ind].Address1, contactList[ind].Address2, 
                    contactList[ind].Address3, contactList[ind].Notes, contactList[ind].IsFavourite, contactList[ind].IsBlocked, contactList[ind].IsSimContact);
            }
            else if(eventName == "PhoneUpdateContact")
            {
                Player p = Player.PlayerData[sender];

                PhoneContact pc = PhoneContactRepository.GetPhoneContactById(int.Parse(arguments[0].ToString()));

                pc.Name = arguments[1].ToString();
                pc.Number = arguments[2].ToString();
                pc.Address1 = arguments[3].ToString();
                pc.Address2 = arguments[4].ToString();
                pc.Address3 = arguments[5].ToString();
                pc.Notes = arguments[6].ToString();

                PhoneContactRepository.UpdateAsync(pc);

                API.triggerClientEvent(sender, "displayPhoneContact", pc.Id, pc.Name, pc.Number, pc.Address1, pc.Address2, pc.Address3, pc.Notes, pc.IsFavourite, pc.IsBlocked, pc.IsSimContact);
            }
            else if(eventName == "PhoneSetFavouriteContact")
            {
                Player p = Player.PlayerData[sender];

                PhoneContact pc = PhoneContactRepository.GetPhoneContactById(int.Parse(arguments[0].ToString()));

                pc.IsFavourite = !pc.IsFavourite;

                PhoneContactRepository.UpdateAsync(pc);

                API.triggerClientEvent(sender, "displayPhoneContact", pc.Id, pc.Name, pc.Number, pc.Address1, pc.Address2, pc.Address3, pc.Notes, pc.IsFavourite, pc.IsBlocked, pc.IsSimContact);
            }
            else if(eventName == "PhoneBatteryChange")
            {
                Player p = Player.PlayerData[sender];
                int battLevel = Convert.ToInt32(float.Parse(arguments[0].ToString()));
                if (battLevel <= 100)
                    p.PrimaryPhone.BatteryLevel = battLevel;

                if (battLevel < 2)
                    p.PrimaryPhone.TurnOff(sender);

                PhoneRepository.UpdateAsync(p.PrimaryPhone);
            }
            else if(eventName == "PhoneDeleteContact")
            {
                Player p = Player.PlayerData[sender];
                p.DeletingContact = PhoneContactRepository.GetPhoneContactById(int.Parse(arguments[0].ToString()));

                API.ShowPopupPrompt(sender, "PhoneConfirmDeleteContact", "Delete " + p.DeletingContact.Name, "Are you sure you want to delete " + p.DeletingContact.Name + " from your contacts?");
            }

            else if(eventName == "PhoneConfirmDeleteContact")
            {
                Player p = Player.PlayerData[sender];
                if((int)arguments[0] == 1)
                {
                    PhoneContactRepository.Delete(p.DeletingContact);
                    p.DeletingContact = null;

                    OnClientBasePhoneEventTrigger(sender, "PhoneContactList", new object { });
                }
            }

            else if(eventName == "PhoneBlockContact")
            {
                PhoneContact pc = PhoneContactRepository.GetPhoneContactById(int.Parse(arguments[0].ToString()));

                if(pc != null)
                {
                    pc.IsBlocked = !pc.IsBlocked;
                    PhoneContactRepository.UpdateAsync(pc);

                    API.triggerClientEvent(sender, "displayPhoneContact", pc.Id, pc.Name, pc.Number, pc.Address1, pc.Address2, pc.Address3, pc.Notes, pc.IsFavourite, pc.IsBlocked, pc.IsSimContact);
                }
            }
            else if(eventName == "PhoneChangeContactStorage")
            {
                Player p = Player.PlayerData[sender];
                PhoneContact pc = PhoneContactRepository.GetPhoneContactById(int.Parse(arguments[0].ToString()));

                if(pc != null)
                {
                    if(pc.IsSimContact)
                    {
                        pc.SavedTo = p.PrimaryPhone.Id;
                    }
                    else
                    {
                        if(p.PrimaryPhone.InstalledSim > 0)
                        {
                            int simContacts = PhoneContactRepository.GetAllContactsOnSim(p.PrimaryPhone.InstalledSim).Count;
                            if (simContacts < SimCard.CONTACT_CAPACITY)
                                pc.SavedTo = p.PrimaryPhone.InstalledSim;
                            else
                                API.SendErrorNotification(sender, "You have reached the limit of contacts stored to this SIM Card");
                        }
                    }

                    pc.IsSimContact = !pc.IsSimContact;

                    PhoneContactRepository.UpdateAsync(pc);
                    API.triggerClientEvent(sender, "displayPhoneContact", pc.Id, pc.Name, pc.Number, pc.Address1, pc.Address2, pc.Address3, pc.Notes, pc.IsFavourite, pc.IsBlocked, pc.IsSimContact);
                }
            }
            else if(eventName == "PhoneEnteredPasscode")
            {
                Player p = Player.PlayerData[sender];

                if(p.PrimaryPhone.Passcode != null)
                {
                    API.triggerClientEvent(sender, "phonePasscodeResult",
                        Crypter.CheckPassword(arguments[0].ToString(), p.PrimaryPhone.Passcode) ? 4 : 0);
                }
            }
        }

        private void OnClientMessageEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "PhoneLoadMessagesApp")
            {
                Player p = Player.PlayerData[sender];

                List<PhoneLog> messages = PhoneLogRepository.GetPhoneLogsOfTypeByIMEI(PhoneLogType.SMS, p.PrimaryPhone.IMEI);

                messages = messages
                    .Where(m => (m.IMEIFrom == p.PrimaryPhone.IMEI) ? m.DeletedFrom != true : m.DeletedTo != true)
                    .GroupBy(m => (m.IMEIFrom == p.PrimaryPhone.IMEI) ? m.NumberTo : m.NumberFrom) 
                    .SelectMany(m => m.OrderByDescending(me => me.SentAt).Take(1))
                    .ToList();

                string convoMessages = "";
                string toFrom = "";
                string messDate = "";
                string viewed = "";
                int count = 0;

                foreach (PhoneLog pl in messages)
                {
                    PhoneContact pc;
                    pc = PhoneContactRepository.GetPhoneContactByNumber(pl.IMEITo == p.PrimaryPhone.IMEI ? pl.NumberFrom : pl.NumberTo, p.PrimaryPhone);
                    if (pc != null)
                    {
                        if (pc.Name.Length > 19) toFrom += string.Format("{0}|", pc.Name.Substring(0, 16) + "...");
                        else toFrom += string.Format("{0}|", pc.Name);
                    }
                    else toFrom += string.Format("{0}|", pl.NumberTo);

                    if (pl.SentAt > Server.Date.AddDays(-1)) messDate += pl.SentAt.ToString("HH:mm") + ",";
                    else if (pl.SentAt > Server.Date.AddDays(-7)) messDate += pl.SentAt.ToString("dddd") + ",";
                    else messDate += pl.SentAt.ToString("dd/MM/yyyy") + ",";

                    if (pl.Viewed) viewed += "yes,";
                    else viewed += "no,";

                    convoMessages += pl.Message + "|";
                    count++;
                }

                API.triggerClientEvent(sender, "populateMessagesList", toFrom, messDate, viewed, convoMessages, count);
            }
            else if(eventName == "PhoneShowMessageConversation")
            {
                Player p = Player.PlayerData[sender];

                List<PhoneLog> messages = PhoneLogRepository.GetPhoneLogsOfTypeByIMEI(PhoneLogType.SMS, p.PrimaryPhone.IMEI);
                messages = messages
                   .GroupBy(m => (m.IMEIFrom == p.PrimaryPhone.IMEI) ? m.NumberTo : m.NumberFrom)
                   .SelectMany(m => m.OrderByDescending(me => me.SentAt).Take(1))
                   .ToList();

                PhoneLog selectedMessages = messages[int.Parse(arguments[0].ToString())];
                string number = "";
                string name = "";

                PhoneContact pc;
                if (selectedMessages.IMEITo == p.PrimaryPhone.IMEI)
                {
                    pc = PhoneContactRepository.GetPhoneContactByNumber(selectedMessages.NumberFrom, p.PrimaryPhone);
                    number = name = selectedMessages.NumberFrom;
                }
                else
                {
                    pc = PhoneContactRepository.GetPhoneContactByNumber(selectedMessages.NumberTo, p.PrimaryPhone);
                    number = name = selectedMessages.NumberTo;
                }

                if(pc != null) {
                    name = pc.Name;
                }

                List<PhoneLog> conversation = PhoneLogRepository.GetPhoneMessagesNumberToIMEI(number, p.PrimaryPhone.IMEI);

                string convoMessages = "";
                string toFrom = "";
                string messDate = "";
                string ids = "";
                int count = 0;

                foreach(PhoneLog con in conversation)
                {
                    if ((p.PrimaryPhone.IMEI == con.IMEIFrom && con.DeletedFrom) || (p.PrimaryPhone.IMEI == con.IMEITo && con.DeletedTo)) continue;
                    ids += con.Id + ",";
                    convoMessages += con.Message + "|";
                    toFrom += (con.IMEIFrom == p.PrimaryPhone.IMEI) ? "outbound," : "inbound,";

                    if (con.SentAt > Server.Date.AddDays(-1)) messDate += con.SentAt.ToString("HH:mm") + ",";
                    else if (con.SentAt > Server.Date.AddDays(-7)) messDate += con.SentAt.ToString("ddd dd MMM") + ",";
                    else messDate += con.SentAt.ToString("dd/MM/yyyy HH:mm") + ",";
                    count++;
                }

                API.triggerClientEvent(sender, "populateMessageConvo", name, number, ids, toFrom, messDate, convoMessages, count);
            }
            else if(eventName == "PhoneSendTextMessage")
            {
                Player p = Player.PlayerData[sender];

                SimCard playersim = SimCardRepository.GetSimCardById(p.PrimaryPhone.InstalledSim);

                if (playersim.Credit >= 2)
                {
                    playersim.Credit -= 2;

                    Phone receiverPhone = PhoneRepository.GetPhoneBySim(SimCardRepository.GetSimCardByNumber(arguments[1].ToString()).Id);

                    if (receiverPhone == null) { API.triggerClientEvent(sender, "sendTextMessageResult", 0); return; }

                    PhoneLog newMessage = new PhoneLog()
                    {
                        IMEITo = receiverPhone.IMEI,
                        IMEIFrom = p.PrimaryPhone.IMEI,
                        NumberFrom = playersim.Number,
                        NumberTo = arguments[1].ToString(),
                        Message = arguments[0].ToString(),
                        SentAt = Server.Date,
                        Duration = 0,
                        Viewed = false,
                        Type = PhoneLogType.SMS
                    };

                    PhoneLogRepository.AddPhoneLog(newMessage);

                    string number = "";
                    string name = "";

                    var pc = PhoneContactRepository.GetPhoneContactByNumber(arguments[1].ToString(), p.PrimaryPhone);
                    name = number = arguments[1].ToString();

                    if (pc != null)
                    {
                        name = pc.Name;
                    }

                    List<PhoneLog> conversation = PhoneLogRepository.GetPhoneMessagesNumberToIMEI(number, p.PrimaryPhone.IMEI);

                    string convoMessages = "";
                    string toFrom = "";
                    string messDate = "";
                    string ids = "";
                    int count = 0;

                    foreach (PhoneLog con in conversation)
                    {
                        if ((p.PrimaryPhone.IMEI == con.IMEIFrom && con.DeletedFrom) || (p.PrimaryPhone.IMEI == con.IMEITo && con.DeletedTo)) continue;
                        ids += con.Id + ",";
                        convoMessages += con.Message + "|";
                        toFrom += (con.IMEIFrom == p.PrimaryPhone.IMEI) ? "outbound," : "inbound,";

                        if (con.SentAt > Server.Date.AddDays(-1)) messDate += con.SentAt.ToString("HH:mm") + ",";
                        else if (con.SentAt > Server.Date.AddDays(-7)) messDate += con.SentAt.ToString("ddd dd MMM") + ",";
                        else messDate += con.SentAt.ToString("dd/MM/yyyy HH:mm") + ",";
                        count++;
                    }

                    Player receiver = Player.PlayerData.Values.FirstOrDefault(pl => pl.PrimaryPhone.Id == receiverPhone.Id);
                    if(receiver != null)
                    {
                        API.triggerClientEvent(receiver.Client, "phoneSendNotification", PhoneApp.AppInfo[1][1], name, newMessage.Message, 5000);
                    }

                    PlayerRepository.UpdateAsync(p);

                    API.triggerClientEvent(sender, "phoneTextMessageResult", 1);
                    API.triggerClientEvent(sender, "populateMessageConvo", name, number, ids, toFrom, messDate, convoMessages, count);
                }
                API.triggerClientEvent(sender, "phoneTextMessageResult", 0);

            }
            else if(eventName == "PhoneMessageComposeContacts")
            {
                Player p = Player.PlayerData[sender];

                List<PhoneContact> phoneContacts = PhoneContactRepository.GetAllContactsOnPhoneAndSim(p.PrimaryPhone.Id,
                    SimCardRepository.GetSimCardById(p.PrimaryPhone.InstalledSim).Id);

                string contactNames = string.Join("|", phoneContacts.Select(pc => pc.Name));
                string contactNumbers = string.Join(",", phoneContacts.Select(pc => pc.Number));

                API.triggerClientEvent(sender, "populateMessageComposeContact", contactNames, contactNumbers);
            }
            else if(eventName == "PhoneMessageActiveConversation")
            {
                Player p = Player.PlayerData[sender];

                List<PhoneLog> conversation = PhoneLogRepository.GetPhoneMessagesNumberToIMEI(arguments[0].ToString(), p.PrimaryPhone.IMEI);

                if(conversation.Count > 0)
                {
                    string name = "";
                    PhoneContact pc = PhoneContactRepository.GetPhoneContactByNumber(arguments[0].ToString(), p.PrimaryPhone);

                    name = pc == null ? arguments[0].ToString() : pc.Name;

                    string convoMessages = "";
                    string toFrom = "";
                    string messDate = "";
                    string ids = "";
                    int count = 0;

                    foreach (PhoneLog con in conversation)
                    {
                        if ((p.PrimaryPhone.IMEI == con.IMEIFrom && con.DeletedFrom) || (p.PrimaryPhone.IMEI == con.IMEITo && con.DeletedTo)) continue;
                        ids += con.Id + ",";
                        convoMessages += con.Message + "|";
                        toFrom += (con.IMEIFrom == p.PrimaryPhone.IMEI) ? "outbound," : "inbound,";

                        if (con.SentAt > Server.Date.AddDays(-1)) messDate += con.SentAt.ToString("HH:mm") + ",";
                        else if (con.SentAt > Server.Date.AddDays(-7)) messDate += con.SentAt.ToString("ddd dd MMM") + ",";
                        else messDate += con.SentAt.ToString("dd/MM/yyyy HH:mm") + ",";
                        count++;
                    }

                    API.triggerClientEvent(sender, "populateMessageConvo", name, arguments[0].ToString(), ids, toFrom, messDate, convoMessages, count);
                }
                else
                {
                    try
                    {
                        if ((bool)arguments[1])
                        {
                            OnClientMessageEventTrigger(sender, "PhoneLoadMessagesApp");
                        }
                    }
                    catch { }
                }
            }
            else if(eventName == "PhoneMessageDelete")
            {
                Player p = Player.PlayerData[sender];

                PhoneLog deletedLog = PhoneLogRepository.GetPhoneLogById(Convert.ToInt32(arguments[0]));

                if (p.PrimaryPhone.IMEI == deletedLog.IMEIFrom) deletedLog.DeletedFrom = true;
                else deletedLog.DeletedTo = true;

                PhoneLogRepository.UpdateAsync(deletedLog);

                OnClientMessageEventTrigger(sender, "PhoneMessageActiveConversation", (p.PrimaryPhone.IMEI == deletedLog.IMEIFrom) ? deletedLog.NumberTo : deletedLog.NumberFrom, true);
            }
        }

        public static async Task<long> GenerateImeiAsync()
        {
            bool unique = false;
            return await Task.Run(() =>
            {
                long imei = 0;
                while (!unique)
                {
                    int imeia = API.shared.RandomNumber(111111111, 999999999);
                    Thread.Sleep(20);
                    int imeib = API.shared.RandomNumber(11111111, 99999999);

                    imei = long.Parse(imeia.ToString() + imeib.ToString());

                    if (PhoneRepository.GetPhoneByIMEI(imei) == null)
                        unique = true;

                }
                return imei;
            });
        }

        public static async Task<string> GeneratePhoneNumberAsync()
        {
            bool unique = false;
            return await Task.Run(() =>
            {
                string number = "";
                while (!unique)
                {
                    number = API.shared.RandomNumber(52100000, 58600000).ToString();

                    if (SimCardRepository.GetSimCardByNumber(number) == null)
                        unique = true;

                }
                return number;
            });
        }
    }
}
