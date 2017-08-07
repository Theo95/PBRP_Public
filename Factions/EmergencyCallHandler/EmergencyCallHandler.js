var callTypeSelector;

API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName === "load_calltype_menu") {
        loadGenericCallTypeMenu()
    } else if (eventName === "add_call_types") {
        addCallTypes(args[0]);
    }
})

function loadGenericCallTypeMenu() {
    res = API.getScreenResolution();
    callTypeSelector = API.createCefBrowser(400,700);
    API.setCefBrowserHeadless(callTypeSelector, false);
    API.setCefBrowserPosition(callTypeSelector, res.Width - res.Width / 2, 100);
    API.loadPageCefBrowser(callTypeSelector, "Factions/EmergencyCallHandler/callTypeSelector.html");
    API.triggerServerEvent("load_call_types");
}

function addCallTypes(type) {
    callTypeSelector.call("addCallTypes", type);
}

function callTypeSelected(type) {
    API.destroyCefBrowser(callTypeSelector);
    const pl = API.getLocalPlayer();
    const loc = API.getEntityPosition(pl)
    API.triggerServerEvent("call_type_selected", type, API.getStreetName(loc));
}