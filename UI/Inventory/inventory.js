
var inventoryBox = null;
var actionsBox = null;

var enteringPin = false;

var pin_cam_start = null;
var pin_cam_end = null;

var invLock = false;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "showPlayerInventory") {
        loadMainPage("UI/Inventory/inventoryHome.html");
    }
    else if (eventName === "populateInventoryItems") {
        API.sleep(300);
        inventoryBox.call("populateInventoryItems", args[0], args[1]);
        API.showCursor(true);
        API.setCefBrowserHeadless(inventoryBox, false);
    }
    else if (eventName === "updatePlayerInventory")
    {
        if (inventoryBox !== null)
        {
            loadMainPage("UI/Inventory/inventoryHome.html");
        }
    }
    else if (eventName === "showFriskInventory")
    {
        loadMainPage("UI/Inventory/inventoryFrisk.html");
        API.sleep(700 + API.getPlayerPing(API.getLocalPlayer()));
        inventoryBox.call("populateInventoryItems", args[0], "your", args[1]);
        API.sleep(400 + API.getPlayerPing(API.getLocalPlayer()));
        inventoryBox.call("populateInventoryItems", args[2], "their");
        API.showCursor(true);
        API.setCefBrowserHeadless(inventoryBox, false);
    }
    else if (eventName === "ShowTrunkInventory") {
        invLock = true;
        loadMainPage("UI/Inventory/inventoryTrunk.html");
        API.sendChatMessage("" + args[3] + " " + args[4]);
        API.sleep(700 + API.getPlayerPing(API.getLocalPlayer()));
        inventoryBox.call("populateInventoryItems", args[0], "your", args[3], args[1]);
        API.sleep(400 + API.getPlayerPing(API.getLocalPlayer()));
        inventoryBox.call("populateInventoryItems", args[2], "trunk", args[4]);
        API.showCursor(true);
        API.setCefBrowserHeadless(inventoryBox, false);
    }
    else if (eventName === "displayActionsForItem") {
        if (actionsBox === null) {
            res = API.getScreenResolution();
            actionsBox = API.createCefBrowser(160, 300);
        }
        else API.setCefBrowserHeadless(actionsBox, true);
       
        API.sleep(200);
        var cursorPos = API.getCursorPositionMaintainRatio();
        API.setCefBrowserPosition(actionsBox, cursorPos.X, cursorPos.Y );
        API.loadPageCefBrowser(actionsBox, "UI/Inventory/inventoryActions.html");
        API.sleep(300);

        API.sendChatMessage("" + args[0]);
        actionsBox.call("displayActionItems", args[0], args[1]);
        API.setCefBrowserHeadless(actionsBox, false);
    }
    else if (eventName === "hideInventoryMenus") { 
        closeInventory(!args[0]);
    }
    else if (eventName === "showInventoryMenus") {
        showInventory();
    }
    else if (eventName === "onPlayerDisconnect")
    {
        if (inventoryBox !== null) API.destroyCefBrowser(inventoryBox);
        if (actionsBox !== null) API.destroyCefBrowser(actionsBox);
    }
});

var inventoryItemSelected = function (id) {
    API.triggerServerEvent("getInventoryTypeActions", id);
};

var inventoryHeaderSelected = function (id) {
    API.triggerServerEvent("OnHeaderSlotDoubleClick", id);
};

var onInventoryItemMoved = function (id, row, col)
{
    API.triggerServerEvent("OnInventoryItemMoved", id, row, col);
} 

var onFriskInventoryItemTaken = function (id, ownerid, row, col) {
    API.triggerServerEvent("OnFriskInventoryItemTaken", id, ownerid, row, col);
}

var onTrunkInventoryItemTaken = function(id, ownerid, ownerType, row, col) {
    API.triggerServerEvent("OnTrunkInventoryItemTaken", id, ownerid, ownerType, row, col);
}


var actionSelected = function (id, action) {
    API.triggerServerEvent("performActionOnItem", id, action);
    closeActionsMenu();
};

var inventoryLock = function (locked) {
    invLock = locked;
}

var simCardSelected = function (id) {
    API.triggerServerEvent("onSimCardSelected", id);
}


API.onKeyDown.connect(function (sender, key) {
    if (resource.hudDisplay.isLogged) {
        if (!resource.hudDisplay.userPrompted) {
            if (enteringPin) {
                var keyVal = key.KeyValue;

                if (keyVal >= 96 && keyVal <= 105) keyVal -= 48;
                if (keyVal >= 48 && keyVal <= 57) {
                    inventoryBox.call("numberButtonPressed", keyVal);
                }
                if (keyVal === 8) {
                    inventoryBox.call("numberButtonPressed", -1);
                }
                if (keyVal === 13) {
                    inventoryBox.call("numberButtonPressed", -2);
                }
            }
        }
        if (resource.hudDisplay.inventoryKey !== 0) {
            if (key.KeyValue === resource.hudDisplay.inventoryKey) {
                if (!API.isChatOpen() && !invLock) {
                    if (inventoryBox === null)
                        API.triggerServerEvent("DisplayPlayerInventory");
                    else
                        closeInventory();
                }
            }
        }
    }
});

var InventoryAddMoneyToHeader = function (id, value) {
    API.triggerServerEvent("InventoryAddMoneyToHeader", id, value);
};

var closeActionsMenu = function () {
    if (actionsBox !== null) {
        API.destroyCefBrowser(actionsBox);
        actionsBox = null;
    }
};

var closeInventory = function (disableMouse = true) {
    if (inventoryBox !== null) {
        API.destroyCefBrowser(inventoryBox);
        inventoryBox = null;
    }
    closeActionsMenu();
    if (disableMouse)
        API.showCursor(false);
    API.setCanOpenChat(true);
    API.setChatVisible(true);
    API.triggerServerEvent("OnInventoryClose")
};

var closeSimMenu = function () {
    if (inventoryBox !== null) {
        API.destroyCefBrowser(inventoryBox);
        inventoryBox = null;
    }
};

var showInventory = function () {
    API.showCursor(true);
};

var cancelPlayerFrisk = function() {
    API.triggerServerEvent("CancelPlayerFrisk");
    closeInventory();
}

var exitTrunk = function()
{
    closeInventory();
    invLock = false;
}

function loadMainPage(url)
{
    var res = API.getScreenResolution();
    if (inventoryBox === null)
    {
        inventoryBox = API.createCefBrowser(res.Width, res.Height);
        API.sleep(50);
        API.loadPageCefBrowser(inventoryBox, url);
    }
    API.sleep(50);
    API.showCursor(true);
}
