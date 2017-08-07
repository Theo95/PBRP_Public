// --- Variable Declarations

var uiPath = "UI/Admin/Property";
var propertyEditBox = null;

// -- Events

API.onServerEventTrigger.connect(function (eventName, args) {
    if (event === "showPropertyEdit") {
        loadMainPage(uiPath + "propertyHome.html");

        propertyEditBox.call("updateTitle", args[0]);
        API.sleep(200);
    }
});

// -- Functionality

function loadMainPage(url) {
    var res = API.getScreenResolution();
    if (propertyEditBox === null) {
        propertyEditBox = API.createCefBrowser(res.Width, res.Height);
        API.sleep(50);
    }
    API.loadPageCefBrowser(propertyEditBox, url);
    API.sleep(30);
    API.setCefBrowserHeadless(propertyEditBox, false);
    API.showCursor(true);
}