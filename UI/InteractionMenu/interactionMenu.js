var interactionMenu = null;
var res = null;
var targettedVehicle = null;
var previousTime = 0;
var interactKeyHeldTime = 0;
var interactKeyHeld = false;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "showVehicleInteractionMenu")
    {
        var distance = API.getEntityPosition(args[0]).DistanceTo(API.getEntityPosition(API.getLocalPlayer()));
        if (interactionMenu !== null) {
            API.destroyCefBrowser(interactionMenu);
        }
        res = API.getScreenResolution();
        var minDist = (distance <= 10 ? distance : 10) / 20;
        interactionMenu = API.createCefBrowser(res.Width * (1 - minDist), res.Height * (1 - minDist));
        API.sleep(50);
        API.setCefBrowserHeadless(interactionMenu, true);

        API.loadPageCefBrowser(interactionMenu, "UI/InteractionMenu/menu.html");
        API.sleep(300);

        interactionMenu.call("initMenuOptions", args[1]);
        API.setCefBrowserHeadless(interactionMenu, false);
        var pos = API.worldToScreen(API.getEntityPosition(args[0]));

        API.setCefBrowserPosition(interactionMenu,
            pos.X - (res.Width / 3 * (1 - (distance / 20))),
            pos.Y - (res.Height / 3 * (1 - (distance / 20))));

        targettedVehicle = args[0];       
    }
});


API.onKeyDown.connect(function (sender, keys)
{
    if (keys.KeyValue === 69)
    {
        interactKeyHeld = true;
    }
});

API.onKeyUp.connect(function (sender, keys) {
    if (keys.KeyValue === 69)
    {
        interactKeyHeld = false;
        interactKeyHeldTime = 0;
    }
});

//API.onUpdate.connect(function () {
//    var distance;
//    if (Math.ceil(API.getGlobalTime() / 100) * 100 % 100 === 0) {
//        if (previousTime === Math.ceil(API.getGlobalTime() / 100) * 100) return;

//        if (interactKeyHeld)
//            interactKeyHeldTime++;

//        if (interactKeyHeldTime === 8) {
//            var vehInRange = [];
//            var streamedVeh = API.getStreamedVehicles();
//            for (var v = 0; v < streamedVeh.Length; v++) {
//                distance = API.getEntityPosition(streamedVeh[v]).DistanceTo(API.getEntityPosition(API.getLocalPlayer()));
//                if (distance <= 20) {
//                    for (var i = 0; i < 6; i++) {
//                        if (vehInRange[i] === null) {
//                            vehInRange[i] = streamedVeh[v]; break;
//                        }
//                        else if (i === 5) {
//                            for (var j = 0; j < 6; j++) {
//                                if (API.getEntityPosition(streamedVeh[v]).DistanceTo(API.getEntityPosition(vehInRange[j]))) {
//                                    vehInRange[j] = streamedVeh[v]; break;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            API.triggerServerEvent("ShowInteractionForVehicle", vehInRange[0], vehInRange[1], vehInRange[2], vehInRange[3], vehInRange[4], vehInRange[5]);
//        }
//        previousTime = Math.ceil(API.getGlobalTime() / 100) * 100;
//    }


//    if (targettedVehicle !== null) {
//        if (interactionMenu === null || !API.doesEntityExist(targettedVehicle)) { targettedVehicle = null; return; }

//        var pos = API.worldToScreen(API.getEntityPosition(targettedVehicle));
//        distance = API.getEntityPosition(targettedVehicle).DistanceTo(API.getEntityPosition(API.getLocalPlayer()));
//        var minDist = (distance <= 10 ? distance : 10) / 20;

//        API.setCefBrowserSize(interactionMenu, res.Width * (1 - minDist), res.Height * (1 - minDist));

//        API.setCefBrowserPosition(interactionMenu,
//            pos.X - (res.Width / 3 * (1 - minDist)),
//            pos.Y - (res.Height / 3 * (1 - minDist)));

//        if (distance >= 20)
//        {
//            cancelInteractionMenu();
//        }
//    }
        
//});

function performInteraction(type)
{
    if (targettedVehicle !== null)
    {
        var modelDimensions = null;
        if (type === "OPEN" || type === "CLOSE")
        {
            modelDimensions = API.getModelDimensions(API.getEntityModel(targettedVehicle));
            API.triggerServerEvent("PerformVehicleInteraction", targettedVehicle, type, modelDimensions.Maximum.Y, modelDimensions.Minimum.Y);

        }
        else
            API.triggerServerEvent("PerformVehicleInteraction", targettedVehicle, type);
        cancelInteractionMenu();
    }
}

function cancelInteractionMenu() {
    if (interactionMenu !== null) {
        API.destroyCefBrowser(interactionMenu);
        interactionMenu = null;
    }
    targettedVehicle = null;
    API.showCursor(false);
}