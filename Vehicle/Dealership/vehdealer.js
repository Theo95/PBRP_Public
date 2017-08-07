var vehicleDealerBook = null;
var inDealerCollider = false;
var interactKey = 0;
var cursorKey = 0;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "InitDealer")
    {
        LoadDealerPage("Vehicle/Dealership/vehdealer-book.html");
    }
    else if (eventName === "dealerFrontPage") {
        API.sleep(100);
        vehicleDealerBook.call("InitDealerBook", args[0], args[1]);
        cursorKey = args[2];
    }
    else if (eventName === "onPlayerEnterDealer")
    {
        API.displaySubtitle("Press ~r~" + args[0] + " ~w~to Enter Helmut's European Auto");
        inDealerCollider = true;
        interactKey = args[1];
    }
    else if (eventName === "onPlayerLeaveDealer")
    {
        API.displaySubtitle("");
        inDealerCollider = false;
    }
    else if (eventName === "addDealerPage")
    {
        vehicleDealerBook.call("SetPageTitle", args[0]);

        API.displaySubtitle("Press ~r~" + cursorKey + "~w~ to show/hide your cursor", 2000000);
    }
    else if (eventName === "showPreviewOptions")
    {
        LoadDealerPage("Vehicle/Dealership/dealerPreviewPanel.html");
        API.sleep(600);
        API.setCefBrowserHeadless(vehicleDealerBook, false);
    }
    else if (eventName === "LoadDealerImages")
    {
        API.setCefBrowserHeadless(vehicleDealerBook, false);
        API.sleep(100);

    }
    else if (eventName === "closeDealerBook")
    {
        closeDealerBook();
    }
});

var outputVehicleName = function (message) {
    API.sendChatMessage(message)
};

var closeDealerBook = function () {
    API.setCefBrowserHeadless(vehicleDealerBook, true);
    API.setActiveCamera(null);
    API.callNative("16661672023969927234", API.getLocalPlayer(), -1);
    API.showCursor(false);
    API.setHudVisible(true);
    API.displaySubtitle("");
};

var previewVehicleDealer = function (id) {
    API.triggerServerEvent("previewVehicleClicked", id);
};

var setDealerVehicleColour = function (num, id) {
    API.triggerServerEvent("SetDealerVehicleColor", num, id);
};

var openDealerVehicleDoors = function () {
    API.triggerServerEvent("OpenDealerVehicleDoors");
};

var openDealerVehicleTrunk = function () {
    API.triggerServerEvent("OpenDealerVehicleTrunk");
};

var openDealerVehicleHood = function () {
    API.triggerServerEvent("OpenDealerVehicleHood");
};

var pressDealerVehicleHorn = function () {
    API.triggerServerEvent("PressDealerVehicleHorn");
};

var purchaseVehicle = function () {
    API.triggerServerEvent("PurchaseDealerVehicle");
};

var takeATestDrive = function () {
    API.triggerServerEvent("DealerTestDrive");
}

var exitPreview = function () {
    API.triggerServerEvent("ExitPreview");
    closeDealerBook();
}

function LoadDealerPage(url)
{
    var res = API.getScreenResolution();
    if (vehicleDealerBook !== null) {
        API.destroyCefBrowser(vehicleDealerBook);
    }
    vehicleDealerBook = API.createCefBrowser(res.Width, res.Height);
    API.sleep(70);
    API.setCefBrowserHeadless(vehicleDealerBook, true);
    API.loadPageCefBrowser(vehicleDealerBook, url);
    API.sleep(30);
    API.showCursor(true);
}

API.onKeyDown.connect(function (sender, key) {
    if(inDealerCollider)
    {
        if(key.KeyValue === interactKey)
        {
            API.triggerServerEvent("InitiateDealer");
        }
    }
});