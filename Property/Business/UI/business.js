
var inInteractPoint = false;
var inEnterPoint = false;
var inExitPoint = false;
var businessUI = null;

API.onServerEventTrigger.connect(function (eventName, args) {

    if (eventName === "displayBusinessInteract") {
        inInteractPoint = args[0] !== "";

        API.displaySubtitle(args[0], 284000);
    }
    else if (eventName === "displayPropertyEnter")
    {
        inEnterPoint = args[0].includes("enter");
        inExitPoint = args[0].includes("exit");
        API.displaySubtitle(args[0], 284000);
    }
    else if (eventName === "initShopUI")
    {
        loadMainPage("Property/Business/UI/Shop/shopMain.html");
        API.sleep(300);
        businessUI.call("LoadShopData", args[0]);
        API.showCursor(true);
    }
    else if (eventName === "initRepairShopUI")
    {
        API.stopControlOfPlayer(API.getLocalPlayer());
        loadMainPage("Property/Business/UI/Mechanic/repairMain.html");
        API.showCursor(true);
        var vehicle = API.getPlayerVehicle(API.getLocalPlayer());
        var health = API.getVehicleHealth(vehicle);
        var body = API.returnNative("GET_VEHICLE_BODY_HEALTH", 7, vehicle);
        var tyresPopped = false;

        for (var w = 0; w < 7; w++) {
            if (API.isVehicleWindowBroken(vehicle, w) || API.isVehicleDoorBroken(vehicle, w)) {
                body = 700;
                break;
            }
        }

        for (var i = 0; i < 7; i++) {
            API.sendChatMessage("" + API.isVehicleTyrePopped(vehicle, i));
            if (API.isVehicleTyrePopped(vehicle, i)) {
                tyresPopped = true;
                break;
            }
        }
        API.sleep(50);
        businessUI.call("setVehicleNameHeader", args[0]);
        API.sleep(50);
        businessUI.call("vehicleDamageData", health, body, tyresPopped);
    }
    else if (eventName === "initGarageVehicleCollectionUI")
    {
        API.stopControlOfPlayer(API.getLocalPlayer());
        loadMainPage("Property/Business/UI/Mechanic/vehicleListGarage.html");
        API.sleep(150);
        API.sendChatMessage("" + args[0]);
        businessUI.call("LoadVehicleList", args[0]);
    }

    else if (eventName === "returnShopStockCount")
    {
        businessUI.call("ChangeQuantityInCart", args[0], args[2], args[1]);
    }
    else if (eventName === "closeShopUI")
    {
        closeBusinessUI();
    }
});


API.onKeyUp.connect(function (sender, key) {
    if (resource.hudDisplay.isLogged)
    {
        var interactKey = resource.hudDisplay.interactKey;
        if (interactKey === 0) return;

        if (key.KeyValue === interactKey)
        {
            if (!inInteractPoint && !inEnterPoint && !inExitPoint) return;
            if (inInteractPoint) API.triggerServerEvent("OnBusinessInteraction");
            else if (inEnterPoint) API.triggerServerEvent("OnPropertyEnter");
            else if (inExitPoint) API.triggerServerEvent("OnPropertyExit");
        }
    }
});

function loadMainPage(url) {
    var res = API.getScreenResolution();
    if (businessUI === null) {
        businessUI = API.createCefBrowser(res.Width, res.Height);
        API.sleep(50);
    }
    API.loadPageCefBrowser(businessUI, url);
    API.sleep(50);
    API.showCursor(true);
}

function verifyStockCount(id, change)
{
    API.triggerServerEvent("OnVerifyShopStock", id, change);
}

function outOfStockNotification() {
    resource.hudDisplay.sendNotification("errorNotification", "Out of stock", 7);
}

function cancelShopTransaction(ids, stock)
{
    API.triggerServerEvent("CancelShopTransaction", ids, stock);
    API.destroyCefBrowser(businessUI);
    businessUI = null;
    API.showCursor(false);
}

function completeShopTransaction(ids, quantity)
{
    API.triggerServerEvent("CompleteShopTransaction", ids, quantity);
    API.destroyCefBrowser(businessUI);
    businessUI = null;
    API.showCursor(false);
}

function performGarageRepair(type) {
    API.triggerServerEvent("PerformGarageRepair", type);
}

function exitRepairShop()
{
    API.destroyCefBrowser(businessUI);
    businessUI = null;
    API.showCursor(false);
    API.triggerServerEvent("ExitRepairShop");
}

function retrieveVehicleGarage(id)
{
    API.triggerServerEvent("RetrieveVehicleFromRepairGarage", id);
}

function closeBusinessUI() {
    API.destroyCefBrowser(businessUI);
    businessUI = null;
    API.showCursor(false);
}