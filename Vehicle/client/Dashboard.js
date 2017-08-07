///**
// * Created by Joe on 11/01/2017.
// */
//var mainBrowser = null;
//var showing = true;
//var activateCruise = false;
//var cruiseSpeed = 0;
//var lastVeh = null;

//API.onServerEventTrigger.connect(function(eventName, args){

//    if(eventName == "toggle_speedo_display"){
//        if (showing) {
     
//            API.setCefBrowserHeadless(mainBrowser, true);
//        } else{
//            API.setCefBrowserHeadless(mainBrowser, false);
//        }
//        showing  = !showing;
//    }
//    else if (eventName === "dashboard_update_fuel")
//    {
//        mainBrowser.call("updateFuel", args[0]);
//    }
//    else if (eventName === "onPlayerDisconnect")
//    {
//        if (activateCruise)
//        {
//            cruiseSpeed = 0;
//            activateCruise = false;
//            API.triggerServerEvent("DeactivateVehicleCruiseControl");
//        }
//    }
//});

//API.onResourceStop.connect(function() {
//    if (mainBrowser != null) {
//        API.destroyCefBrowser(mainBrowser);
//    }
//});

//API.onPlayerEnterVehicle.connect(function(handle) {
//    if (mainBrowser == null) {
//        var res = API.getScreenResolution();
//        mainBrowser = API.createCefBrowser(1000, 500);
//        API.setCefBrowserPosition(mainBrowser, 320, res.Height - 220);
//        API.waitUntilCefBrowserInit(mainBrowser);
//        API.loadPageCefBrowser(mainBrowser, "Vehicle/client/dashboard.html");
//    }
//    lastVeh = handle;
//    API.setCefBrowserHeadless(mainBrowser, false);
//});

//API.onPlayerExitVehicle.connect(function (handle) {
//    API.setCefBrowserHeadless(mainBrowser, true);
//});

//API.onKeyDown.connect(function (sender, key) {
//    var player = API.getLocalPlayer();
//    var inVeh = API.isPlayerInAnyVehicle(player);

//    //if (key.KeyCode == Keys.C) {
//    //    if (inVeh) {
//    //        if (!activateCruise) {
//    //            var car = API.getPlayerVehicle(player);
//    //            var velocity = API.getEntityVelocity(car);
//    //            var speed = Math.sqrt(
//    //                velocity.X * velocity.X +
//    //                velocity.Y * velocity.Y +
//    //                velocity.Z * velocity.Z
//    //            );
//    //            cruiseSpeed = speed;
//    //            activateCruise = true;
//    //        }
//    //        else {
//    //            cruiseSpeed = 0;
//    //            activateCruise = false;
//    //            API.triggerServerEvent("DeactivateVehicleCruiseControl");
//    //        }
//    //    }   
//    //}
//});


//API.onUpdate.connect(function() {
//    var player = API.getLocalPlayer();

//    if (lastVeh != null) {
//        var car = lastVeh;
//        var rpm = API.getVehicleRPM(car);
//        var velocity = API.getEntityVelocity(car);
//        var speed = Math.sqrt(
//            velocity.X * velocity.X +
//            velocity.Y * velocity.Y +
//            velocity.Z * velocity.Z
//        );

//        if (cruiseSpeed > 0) {
//            API.triggerServerEvent("ActivateVehicleCruiseControl", car, cruiseSpeed);
//            cruiseSpeed = 0;
//        }

//        if ((API.isControlJustPressed(71) || API.isControlJustPressed(72) || API.returnNative("HAS_ENTITY_COLLIDED_WITH_ANYTHING", 8, car) ||
//            !API.returnNative("IS_VEHICLE_ON_ALL_WHEELS", 8, car)) && activateCruise) {
//            API.triggerServerEvent("DeactivateVehicleCruiseControl", car);
//            activateCruise = false;
//            cruiseSpeed = 0;
//        }
//    }

//    if (mainBrowser != null)
//    {
//        mainBrowser.call("updateSpeed", speed * 2.23); // from m/s to km/h
//        mainBrowser.call("updateRpm", rpm * 10);
//    }
//});

