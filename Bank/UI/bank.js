/*eslint-disable */
var keyInteract = null;
var inBankCol = false;

var usingBank = false;

var enteringPin = false;
var bankUI = null;

API.onServerEventTrigger.connect(function (eventName, args) {
    var player;
    var pos;
    var rot;
    var destCamera;
    if (eventName === "onEnterBankCol") {
        API.displaySubtitle("Press ~r~" + resource.hudDisplay.interactChar + " ~w~to speak to the bank teller", 10000);
        inBankCol = true;
    }
    else if (eventName === "onLeaveBankCol") {
        API.displaySubtitle("");
        inBankCol = false;
        usingBank = false;
    }
    else if (eventName === "onExecuteBank")
    {
        usingBank = true;
    }
    else if (eventName === "showBankOptions") {
        player = API.getLocalPlayer();
        pos = API.getEntityPosition(player).Add(new Vector3(0, 0, 0.5));
        rot = API.getEntityRotation(player);
        destCamera = API.createCamera(new Vector3(-113.463, 6469.748, 32.2671), rot);
        API.pointCameraAtPosition(destCamera, new Vector3(-112.1975, 6471.044, 32.0971));

        API.setHudVisible(false);
        //API.pointCameraAtEntity(bankCamera, args[0], new Vector3(0,0,0));
        var bankCamera = API.createCamera(pos, rot);
        API.interpolateCameras(bankCamera, destCamera, 1000, true, true);
        API.sleep(1000);
        loadMainPage("Bank/UI/bankOption.html");
    }
    else if (eventName === "playerEnterPinBank") {
        player = API.getLocalPlayer();
        rot = API.getEntityRotation(player);
        API.displaySubtitle("");

        API.setCefBrowserHeadless(bankUI, true);

        pin_start_camera = API.createCamera(new Vector3(-113.463, 6469.748, 32.2671), rot);
        pin_dest_camera = API.createCamera(new Vector3(-113.1962, 6470.69693, 32.33671), rot);
        API.pointCameraAtPosition(pin_dest_camera, new Vector3(-113.20147, 6470.89793, 31.9489994));

        API.interpolateCameras(pin_start_camera, pin_dest_camera, 2000, true, true);
        loadMainPage("Bank/UI/enterPin.html");
        API.sleep(1800);
 

        bankUI.call("initPinMachine", args[0]);
        API.sleep(500);

        API.setCefBrowserHeadless(bankUI, false);
        enteringPin = true;
    }
    else if (eventName === "correctPinEntered") {
        API.destroyCefBrowser(bankUI);
        bankUI = null;
        player = API.getLocalPlayer();
        pos = API.getEntityPosition(player).Add(new Vector3(0, 0, 0.5));
        rot = API.getEntityRotation(player);
        destCamera = API.createCamera(new Vector3(-113.463, 6469.748, 32.2671), rot);
        API.pointCameraAtPosition(destCamera, new Vector3(-112.1975, 6471.044, 32.0971));

        API.setHudVisible(false);
        //API.pointCameraAtEntity(bankCamera, args[0], new Vector3(0,0,0));
        var bankCamera = API.createCamera(pos, rot);
        API.interpolateCameras(bankCamera, destCamera, 1000, true, true);
        API.sleep(1000);
        loadMainPage("Bank/UI/transactionOptions.html");
        bankUI.call("LoadOptions");
        API.sleep(200);
        API.setCefBrowserHeadless(bankUI, false);
        enteringPin = false;
    }
    else if (eventName === "selectCardMessage")
    {
        API.displaySubtitle("~r~Right Click~w~ on a bank card and press ~r~USE");
    }
});


function loadMainPage(url)
{
    var res = API.getScreenResolution();
    if (bankUI === null) {
        bankUI = API.createCefBrowser(res.Width, res.Height);
        API.sleep(50);
    }
    API.loadPageCefBrowser(bankUI, url);
    API.sleep(50);
    API.showCursor(true);
}

API.onKeyDown.connect(function (sender, key) {
    if (resource.hudDisplay.isLogged) {
        if (inBankCol) {
            if (key.KeyValue === resource.hudDisplay.interactKey && !usingBank) {
                API.triggerServerEvent("ActivateBank");
                API.displaySubtitle("");
                usingBank = true;
            }
        }
        if (enteringPin) {
            var keyVal = key.KeyValue;
            if (keyVal >= 96 && keyVal <= 105) keyVal -= 48;
            if (keyVal >= 48 && keyVal <= 57) {
                bankUI.call("numberButtonPressed", keyVal);
            }
            if (keyVal === 8) {
                bankUI.call("numberButtonPressed", -1);
            }
            if (keyVal === 13) {
                bankUI.call("numberButtonPressed", -2);
            }
        }
    }
});


var noBankCardSelected = function () {
    API.triggerServerEvent("displayPlayerIDs");
};

var IDCardSelected = function (id) {
    API.triggerServerEvent("validateIDGiven", id);
};

var validatePin = function (pin) {
    API.triggerServerEvent("validatePin", pin);
};

var newPinConfirmed = function (pin) {
    API.triggerServerEvent("createAccountPinConfirmed", pin);
    closePinMachine();
    enteringPin = false;
};

var closePinMachine = function () {
    API.destroyCefBrowser(bankUI);
    bankUI = null;
    loadMainPage("Bank/UI/bankOption.html");

    API.setCefBrowserHeadless(bankUI, true);
    enteringPin = false;

    var player = API.getLocalPlayer();

    var pos = API.getEntityPosition(player).Add(new Vector3(0, 0, 0.5));
    var rot = API.getEntityRotation(player);

    var destCamera = API.createCamera(new Vector3(-113.463, 6469.748, 32.2671), rot);
    API.pointCameraAtPosition(destCamera, new Vector3(-112.1975, 6471.044, 32.0971));

    var bankCamera = API.createCamera(new Vector3(-113.1962, 6470.69693, 32.33671), rot);
    API.interpolateCameras(bankCamera, destCamera, 1500, true, true);

    API.sleep(1700);
    API.setCefBrowserHeadless(bankUI, false);

}

var noIDSelected = function () {
    closeBank();
};

var createNewBankAccount = function () {
    loadMainPage("Bank/UI/createNewAccount.html");
};

var newBankAccountChoice = function (type) {
    API.triggerServerEvent("newBankAccountChoice", type);
};

var accessAccount = function () {
    API.triggerServerEvent("accessAccount");
};

var transactionBank = function (type) {
    API.triggerServerEvent("bankInputAction", type);
};

var bankCardSelected = function (id) {
    API.triggerServerEvent("onBankCardSelected", id);
};

function closeBank() {
    if (bankUI !== null) {
        API.destroyCefBrowser(bankUI);
        bankUI = null;
    }
    API.displaySubtitle("");
    inBankCol = false;
    usingBank = false;

    API.triggerServerEvent("onBankLeave");
    API.setActiveCamera(null);
    API.showCursor(false);


}
