var previousTime = 0;

API.onServerEventTrigger.connect(function (eventName, args) {

    if (eventName === "updateWildLifePositions")
    {
        var allPed = API.getAllPeds();
        var handles = new Array();
        var positions = new Array();

        for (var i = 0; i < allPed.Length; i++) {
            var pos = API.returnNative("GET_ENTITY_COORDS", 5, allPed[i], true);
            if (pos.DistanceTo(API.getEntityPosition(API.getLocalPlayer())) < 1400) {
                if (API.getEntityModel(allPed[i]) === -664053099 || API.getEntityModel(allPed[i]) === -541762431) {
                    handles.push({ Value: allPed[i].Raw });
                    positions.push({ X: pos.X, Y: pos.Y, Z: pos.Z });
                }
            }

            if (handles.length)
                API.triggerServerEvent("updateWildlifePositions", JSON.stringify(handles), JSON.stringify(positions));
        }
    }

});

API.onUpdate.connect(function () {
    //if (Math.ceil(API.getGlobalTime() / 100) * 100 % 2400 === 0) {
    //    if (previousTime === Math.ceil(API.getGlobalTime() / 100) * 100) return;
    //    previousTime = Math.ceil(API.getGlobalTime() / 100) * 100;
    //}
});