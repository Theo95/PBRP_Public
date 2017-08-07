var phoneUI = null;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "initiatePhone")
    {
        loadMainPage("Player/Phone/UI/SmartPhone/smartPhone.html");
    }
    else if (eventName === "PhoneBootScreen")
    {
        phoneUI.call("LoadPhoneScreen", "smartBootScreen");
    }
    else if (eventName === "PhoneHomeScreen")
    {
        API.sleep(200);
        phoneUI.call("LoadHomeScreen", args[2]);
        API.sleep(100);
        phoneUI.call("InitialiseSmartphone", args[0], args[1], args[3]);
        API.setCefBrowserHeadless(phoneUI, false);
        API.triggerServerEvent("ToggleCursorLock", false);
        API.setCanOpenChat(true);
    }
    else if (eventName === "ReloadHomeScreen")
    {
        phoneUI.call("LoadHomeScreen", args[0]);
        API.setCefBrowserHeadless(phoneUI, false);
        API.triggerServerEvent("ToggleCursorLock", false);
        API.setCanOpenChat(true);
    }
    else if (eventName === "InitiatePhoneOff")
    {
        loadMainPage("Player/Phone/UI/SmartPhone/smartPhone.html");
        API.sleep(500);
        phoneUI.call("clearUpdateIntervals");
        API.triggerServerEvent("ToggleCursorLock", false);
        API.setCanOpenChat(true);
        phoneUI.call("SetPhoneOff");
    }
    else if (eventName === "hidePhoneUI")
    {
        phoneUI.call("clearUpdateIntervals");
        API.setCefBrowserHeadless(phoneUI, true);
        API.triggerServerEvent("ToggleCursorLock", false);
        API.setCanOpenChat(true);
    }
    else if (eventName === "LoadPhoneApps")
    {
        API.sleep(300);
        phoneUI.call("LoadPhoneApps", args[0], args[1], args[2], args[3]);
    }
    else if (eventName === "LoadSettingsInfoPage")
    {
        phoneUI.call("LoadPage", "smartSettingsInfo.html");
        API.sleep(200);
        phoneUI.call("LoadSettingsInfoPage", args[0], args[1], args[2]);
    }
    else if (eventName === "phoneSendNotification")
    {
        phoneUI.call("sendPhoneNotification", args[0], args[1], args[2], args[3]);
    }
    else if (eventName === "PhoneCallConnection")
    {
        phoneUI.call("phoneCallConnect", args[0]);
    }
    else if (eventName === "IncomingPhoneCall")
    {
        phoneUI.call("incomingCall", args[0]);
    }
    else if (eventName === "redialRecentCall")
    {
        phoneUI.call("RedialPhoneCall", args[0]);
    }
    else if (eventName === "PhoneCallAnswered")
    {
        phoneUI.call("phoneCallAnswered");
    }
    else if (eventName === "phoneTerminateCall")
    {
        phoneUI.call("terminateCall");
    }
    else if (eventName === "populateRecentCalls")
    {
        phoneUI.call("populateRecentCalls", args[0], args[1]);
        API.triggerServerEvent("ToggleCursorLock", false);
        API.setCanOpenChat(true);
    }
    else if (eventName === "populateContactList") {
        phoneUI.call("populateContactList", args[0], args[1]);
        API.triggerServerEvent("ToggleCursorLock", false);
        API.setCanOpenChat(true);
    }
    else if (eventName === "PhoneSimInstalled")
    {
        phoneUI.call("InstallNewSim");
    }
    else if (eventName === "displayPhoneContact")
    {
        phoneUI.call("ShowContact", args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
    }
    else if (eventName === "phoneUpdateClock")
    {
        phoneUI.call("updatePhoneClock", args[0], args[1]);
    }
    else if (eventName === "phonePasscodeResult")
    {
        phoneUI.call("passcodeResult", args[0]);
    }
    else if (eventName === "onPlayerDisconnect")
    {
        if (phoneUI !== null) API.destroyCefBrowser(phoneUI);
    }
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "populateMessagesList")
    {
        phoneUI.call("populateMessages", args[0], args[1], args[2], args[3], args[4]);
    }
    else if (eventName === "populateMessageConvo")
    {
        phoneUI.call("populateConversation", args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
    }
    else if (eventName === "phoneTextMessageResult")
    {
        phoneUI.call("textMessageResult", args[0]);
    }
    else if (eventName === "populateMessageComposeContact")
    {
        phoneUI.call(eventName, args[0], args[1]);
    }
});


//Phone base functionality

var insertSimCard = function () {
    API.triggerServerEvent("PhoneInsertSimCard");
}

var LoadHomeScreen = function () {
    API.triggerServerEvent("LoadPhoneHomeScreen");
};

var settingsInfoPage = function () {
    API.triggerServerEvent("ShowSettingsInfoPage");
}

var placePhoneCall = function (number)
{
    API.triggerServerEvent("PhoneMakeCall", number);
    API.triggerServerEvent("ToggleCursorLock", false);
    API.setCanOpenChat(true);
}

var endPhoneCall = function (duration) {
    API.triggerServerEvent("PhoneEndCall", duration);
}

var answerPhoneCall = function () {
    API.triggerServerEvent("PhoneAnswerCall");
}

var phoneSpeakerphone = function () {
    API.triggerServerEvent('PhoneToggleSpeaker');
}

var phoneMuteMicrophone = function () {
    API.triggerServerEvent("PhoneToggleMic");
}

var showRecentCalls = function () {
    API.triggerServerEvent('PhoneRecentCalls');
}

var showContactList = function () {
    API.triggerServerEvent('PhoneContactList');
}

var showFavouriteList = function () {
    API.triggerServerEvent('PhoneFavouriteList');
}

var recentCallSelect = function (id) 
{
    API.triggerServerEvent("PhoneRecentCallSelect", id);
}

var addNewContact = function () {
    API.setCanOpenChat(false);
    API.triggerServerEvent("PhoneAddNewContact");
}

var createNewContact = function(name, number, add1, add2, add3, notes)
{
    API.triggerServerEvent("PhoneCreateNewContact", name, number, add1, add2, add3, notes);
    API.setCanOpenChat(true);
}

var updateContact = function (id, name, number, add1, add2, add3, notes) {
    API.triggerServerEvent("PhoneUpdateContact", id, name, number, add1, add2, add3, notes);
    API.setCanOpenChat(true);
}

var phoneContactSelected = function (id, fav) 
{
    API.triggerServerEvent("PhoneContactSelected", id, fav);
}

var setFavouriteContact = function (id) {
    API.triggerServerEvent("PhoneSetFavouriteContact", id);
};

var phoneBatteryUpdate = function (battery) {
    API.triggerServerEvent("PhoneBatteryChange", battery);
}

var deletePhoneContact = function (id) {
    API.triggerServerEvent("PhoneDeleteContact", id);
}

var blockPhoneContact = function (id) {
    API.triggerServerEvent("PhoneBlockContact", id);
}

var changeContactStorage = function (id) {
    API.triggerServerEvent("PhoneChangeContactStorage", id);
}

var submitPasscode = function (pass) {
    API.triggerServerEvent("PhoneEnteredPasscode", pass);
}


//Message App functionality

var loadMessagesApp = function () {
    API.triggerServerEvent("PhoneLoadMessagesApp");
}

var messageSelect = function (id) {
    API.triggerServerEvent("PhoneShowMessageConversation", id);
}

var phoneSendTextMessage = function (message, number) {
    API.sendChatMessage(number);
    API.triggerServerEvent("PhoneSendTextMessage", message, number);
}

var phoneMessageComposeContacts = function () {
    API.triggerServerEvent("PhoneMessageComposeContacts");
}

var phoneMessageActiveConversation = function (number) {
    API.triggerServerEvent("PhoneMessageActiveConversation", number);
}

var phoneMessageDelete = function (id) {
    API.triggerServerEvent("PhoneMessageDelete", id);
}


var onMessageFocus = function (lock) {
    API.setCanOpenChat(!lock);
    API.triggerServerEvent("ToggleCursorLock", lock);
    resource.inventory.inventoryLock(lock);
}

function loadMainPage(url) {
    var res = API.getScreenResolution();
    if (phoneUI === null) {
        phoneUI = API.createCefBrowser(res.Width, res.Height);
        API.sleep(50);
    }
    API.loadPageCefBrowser(phoneUI, url);
    API.sleep(100);
    API.setCefBrowserHeadless(phoneUI, false);
}