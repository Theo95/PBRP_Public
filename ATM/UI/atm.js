
var atmBox = null;
var enteringPin = false;
var inATMCol = false;
var interactKey = null;
var usingATM = false;

var accountCreated = false;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "onExecuteATM") {
        var player = API.getLocalPlayer();
        var pos = args[0];
        var rot = API.getEntityRotation(player);

        var start_camera = API.createCamera(API.getGameplayCamPos(), rot);
        var dest_camera = API.createCamera(pos, rot);

        API.setHudVisible(false);
        API.pointCameraAtPosition(dest_camera, args[1]);
        API.setActiveCamera(start_camera);
        API.interpolateCameras(start_camera, dest_camera, 1000, true, true);
        API.sleep(1000);
        loadATMScreen("ATM/UI/atmInsertCard.html");
    }
    else if (eventName === "enterATMPin")
    {
        loadATMScreen("ATM/UI/atmEnterPin.html");
        enteringPin = true;
    }
    else if (eventName === "atmCorrectPin")
    {
        loadATMScreen("ATM/UI/atmDashboard.html");
        API.sleep(300);
        atmBox.call("OnDashboardLoad", "atmDash.html");
        API.sleep(200);
        atmBox.call("InjectPlayerName", args[0], args[1]);
    }
    else if (eventName === "atmBalanceReturn")
    {
        atmBox.call("OnDashboardLoad", "atmBalance.html");
        API.sleep(300);
        atmBox.call("ShowBalance", args[0]);
    }
    else if (eventName === "atmWithdrawComplete")
    {
        atmBox.call("OnDashboardLoad", "atmDash.html");

    }
    else if (eventName === "atmWithdrawError")
    {
        atmBox.call("WithdrawError", args[1]);
    }
    else if (eventName === "closeATMMenu")
    {
        closeATM();
    }
    else if (eventName === "onEnterATMCol")
    {
        API.displaySubtitle("Press ~r~" + resource.hudDisplay.interactChar + " ~w~to use the ATM", 10000);
        inATMCol = true;
    }
    else if (eventName === "onLeaveATMCol")
    {
        API.displaySubtitle("");
        inATMCol = false;
        usingATM = false;
        enteringPin = false;
    }
});

function loadATMScreen(url)
{
    var res = API.getScreenResolution();
    if (atmBox === null) {
        atmBox = API.createCefBrowser(res.Width, res.Height);
        API.sleep(50);
    }
    API.loadPageCefBrowser(atmBox, url);
    API.sleep(50);
    API.setCefBrowserHeadless(atmBox, false);
    API.showCursor(true);
}

var closeATM = function () {
    API.destroyCefBrowser(atmBox);
    atmBox = null;
    API.setActiveCamera(null);
    API.callNative("16661672023969927234", API.getLocalPlayer(), -1);
    API.showCursor(false);
    API.setHudVisible(true);
    enteringPin = false;
    API.triggerServerEvent("closeATM");
};

API.onKeyDown.connect(function (sender, key) {
    if (resource.hudDisplay.isLogged) {
        if (enteringPin) {
            var keyVal = key.KeyValue;
            if (keyVal >= 96 && keyVal <= 105) keyVal -= 48;
            if (keyVal >= 48 && keyVal <= 57) {
                atmBox.call("numberButtonPressed", keyVal);
            }
            if (keyVal === 8) {
                atmBox.call("numberButtonPressed", -1);
            }
            if (keyVal === 13) {
                atmBox.call("numberButtonPressed", -2);
            }
        }
        if (inATMCol) {
            if (key.KeyValue === resource.hudDisplay.interactKey && !usingATM) {
                API.triggerServerEvent("ActivateATM");
                usingATM = true;
            }
        }
    }
});

var insertCard = function () {
    API.triggerServerEvent("chooseBankCard");
};

var requestBalance = function () {
    API.triggerServerEvent("atmRequestBalance");
};

var atmCardSelected = function (id) {
    API.triggerServerEvent("ATMCardSelected", id);
};

var atmChangePin = function (pin) {
    API.triggerServerEvent("atmChangePin", pin);
};

var enableATMTyping = function () {
    enteringPin = true;
};

var validatePin = function (pin) {
    API.triggerServerEvent("atmValidatePin", pin);
};

var withdrawMoney = function (amount) {
    API.triggerServerEvent("atmWithdrawMoney", amount);
};

