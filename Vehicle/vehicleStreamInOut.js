var previousTime = 0;
var recentUpdate = false;
var waitUntilUpdate = 0;

var entityToSync = null;

API.onServerEventTrigger.connect(function (eventName, args) {

})

//API.onEntityStreamIn.connect(function (entity, entType) {
//    if (entType == 1) {
//       // API.triggerServerEvent("onVehicleStreamIn", entity);
//        //recentUpdate = true;
//        //if (waitUntilUpdate <= 0) {
//        //    waitUntilUpdate = 8;
//        //    entityToSync = entity;
//        //}
//        ////vehiclesToStream.push(entity);
//    }
//});

//API.onEntityStreamOut.connect(function (entity, entType) {

//});

API.onKeyUp.connect(function (sender, keys) {

});

//API.onUpdate.connect(function () {
//    if (Math.ceil(API.getGlobalTime() / 100) * 100 % 500 === 0) {
//        if (previousTime === Math.ceil(API.getGlobalTime() / 100) * 100) return;
//        if (recentUpdate) { recentUpdate = false; return; }

//        if (waitUntilUpdate == 6) {
//            API.triggerServerEvent("onVehicleStreamIn", entityToSync);
//        }
//        if (waitUntilUpdate > 0)
//        {
//            waitUntilUpdate--;
//        }
//        previousTime = Math.ceil(API.getGlobalTime() / 100) * 100;
//    }
//});