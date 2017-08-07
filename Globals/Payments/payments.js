
var paymentsUI = null;
var enteringPin = false;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "initiateCardPaymentUI") {
        loadMainPage("Globals/Payments/globalEnterPin.html");

        enteringPin = true;
    }
    else if (eventName === "onPlayerDisconnect") {
        if (paymentsUI !== null) {
            API.destroyCefBrowser(paymentsUI);
        }
    }
    else if (eventName === "closePaymentUI")
    {
        if (paymentsUI)
            API.destroyCefBrowser(paymentsUI);
        paymentsUI = null;
    }
});

var validatePaymentPin = function (pin, attemptsRemain) {
    API.triggerServerEvent("OnPaymentPinEntered", pin, attemptsRemain);
}


API.onKeyDown.connect(function (sender, key) {
    if (resource.hudDisplay.isLogged) {
        if (enteringPin) {
            var keyVal = key.KeyValue;
            if (keyVal >= 96 && keyVal <= 105) keyVal -= 48;
            if (keyVal >= 48 && keyVal <= 57) {
                paymentsUI.call("numberButtonPressed", keyVal);
            }
            if (keyVal === 8) {
                paymentsUI.call("numberButtonPressed", -1);
            }
            if (keyVal === 13) {
                paymentsUI.call("numberButtonPressed", -2);
            }
        }
    }
});

function loadMainPage(url)
{
    if (paymentsUI === null)
    {
        var res = API.getScreenResolution();
        paymentsUI = API.createCefBrowser(res.Width, res.Height);
        API.sleep(50);
    }
    API.loadPageCefBrowser(paymentsUI, url);
    API.sleep(50);
}

function closeGlobalPinMachine() {
    enteringPin = false;
    API.destroyCefBrowser(paymentsUI);
    paymentsUI = null;
    API.showCursor(false);
    API.triggerServerEvent("DisplayPlayerInventory");
}