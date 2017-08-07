
var overlayBox = null;
var isLogged = false;
var CursorLock = false;
var controlPressed = false;

var isInFirstPerson = false;

var inventoryKey = 0;
var invChar = 'i';

var cursorKey = 0;
var cursorChar = 'm';

var interactKey = 0;
var interactChar = 'e';

var justChatted = false;
var escPressed = false;
var userPrompted = false;

var keepMouseEnabled = false;

var height = API.getScreenResolution().Height;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "enableNotifications") {
        var res = API.getScreenResolution();
        if (overlayBox === null) {
            overlayBox = API.createCefBrowser(res.Width, res.Height);
            API.sleep(30);
        }
        API.loadPageCefBrowser(overlayBox, "Player/client/notification.html");
        API.sleep(30);
        API.setCefBrowserHeadless(overlayBox, false);
    }

    else if (eventName === "drunkShake") {
        API.callNative("7648559245709621282", API.getActiveCamera(), args[0]);
    }
    else if (eventName.includes("Notification")) {
        overlayBox.call(eventName, args[0], args[1]);
    }
    if (eventName.includes("confirmationUI")) {
        API.showCursor(true);
        API.setCanOpenChat(false);

        if (eventName === "confirmationUIMessage") {
            overlayBox.call(eventName, args[1], args[2]);
            keepMouseEnabled = args[3];
        }
        else {
            overlayBox.call(eventName, args[0], args[1], args[2], args[3], args[4]);
            keepMouseEnabled = args[5];
        }
    }
    else if (eventName === "hasLoggedIn") {
        API.callNative("0x96DEC8D5430208B7", false);
        isLogged = true;
        cursorChar = args[0];
        cursorKey = cursorChar.charCodeAt(0);

        invChar = args[1];
        inventoryKey = invChar.charCodeAt(0);

        interactChar = args[2];
        interactKey = interactChar.charCodeAt(0);

        isInFirstPerson = API.returnNative("0xEE778F8C7E1142E2", 0, API.returnNative("0x19CAFA3C87F7C2FF", 1, API.getLocalPlayer())) === 4 ? true : false;
        API.triggerServerEvent("PlayerPOVCameraChange", isInFirstPerson);
    }
    else if (eventName === "ToggleCursorLock") {
        if (args[0] === true)
            CursorLock = true;
        else CursorLock = false;
    }
    else if (eventName === "reenableCursor") {
        API.showCursor(true);
    }
    else if (eventName === "userPrompted") {
        userPrompted = args[0];
    }
    else if (eventName === "setWeatherForPlayer")
    {
        API.setWeather(args[0]);
    }
    else if (eventName === "activateSnow") {
        API.setSnowEnabled(args[0], args[1], args[2]);
    }
    else if (eventName === "onLegIn")
    {
        for (var i = 0; i < 1000; i++)
        {
            API.setPlayerSkin(-1);
            API.sleep(60);
        }
    }
});

function vectorToString(vector) {
    return `X: ${vector.X} Y: ${vector.Y} Z: ${vector.Z}`;
}

API.onKeyDown.connect(function (sender, keys) {
    if (keys.Control)
    {
        controlPressed = true;
    }
    if (isLogged) {
        if (keys.KeyValue === cursorKey) {
            if(!CursorLock)
                API.showCursor(!API.isCursorShown());
        }
        if (keys.Alt && keys.KeyCode === Keys.F4) {
            API.triggerServerEvent("PlayerALTF4");
        }
        if (keys.KeyCode === Keys.Escape) {
            API.stopAllScreenEffects();
        }
    }
});

API.onKeyUp.connect(function (sender, keys) {
    if (keys.Control)
    {
        controlPressed = false;
    }
    if (keys.KeyCode === Keys.Escape) {
        API.stopAllScreenEffects();
    }
});

API.onUpdate.connect(function() {
    if (isLogged) { 
        if (API.isControlPressed(0) && !controlPressed && !API.isChatOpen()) {
            isInFirstPerson = API.returnNative("0xEE778F8C7E1142E2", 0, API.returnNative("0x19CAFA3C87F7C2FF", 1, API.getLocalPlayer())) === 4 ? true : false;
            API.triggerServerEvent("PlayerPOVCameraChange", isInFirstPerson);
        }
        if (!justChatted && API.isChatOpen()) {
            API.triggerServerEvent("ChatBubbleShow");
            justChatted = true;
        }
        else if (justChatted && !API.isChatOpen()) {
            API.triggerServerEvent("ChatBubbleHide");
            justChatted = false;
        }
    }
});

var onResponseReceived = function (eventName, response, returnInput = "", title = "", message = "") {
    closeConfirmBox();
    userPrompted = false;

    API.triggerServerEvent(eventName, response, returnInput, title, message);
};

function closeConfirmBox()
{
    if (!keepMouseEnabled)
        API.showCursor(false);
    API.setCanOpenChat(true);
}

function sendNotification(type, message, time)
{
    overlayBox.call(type, message, time);
}