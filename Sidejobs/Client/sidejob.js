
var farmjobLocationMarker = null;
var farmjobLocationBlip = null;
var FMarker = null;
var showFMarker = false;
var FBlip = null;
var FVBlip = [null, null];
var FDelmarker = null;
var FDelBlip = null;
var delivering = false;
var FDelPos = null;

var activeMarkers = [];

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "toggleFarmjobMarker")
    {
        toggleFarmjobMarker(args[0], args[1]);
    }
    if (eventName === "toggleFarmjobBlip") {
        toggleFarmjobBlip(args[0], args[1]);
    }
    if(eventName === "makeFVMarker")
    {
        makeFVMarker(args[0], args[1]);
    }
    if (eventName === "deleteFVMarker")
    {
        deleteFVMarker(args[0]);
    }
    if(eventName === "togFDelMarker")
    {
        togFDelMarker(args[0], args[1]);
    }
    if (eventName === "displayNearbyTrees")
    { 
        ShowNearbyTrees(JSON.parse(args[0]));
    }
});

function addMarker(marker, location, size)
{
    if(marker === null)
    {
        return API.createMarker(1, location, new Vector3(0, 0, 0), new Vector3(0, 0, 0), size, 200, 0, 0, 200);
    }
}

function addBlip(blip, location)
{
    if (blip === null) {
        return API.createBlip(location);
    }
}

function removeEntity(markerEntity)
{
    if(markerEntity !== null)
    {
        API.deleteEntity(markerEntity);
    }
}

function toggleFarmjobMarker(status, location)
{
    if (status) {
        
        farmjobLocationMarker = addMarker(farmjobLocationMarker, location, new Vector3(5, 5, 1));
    }
    else {
        removeEntity(farmjobLocationMarker);
        farmjobLocationMarker = null;
        //if (farmjobLocationBlip != null) {
        //    API.deleteEntity(FMarker);
        //    API.deleteEntity(FBlip);
        //    FBlip = null;
        //    FMarker = null;
        //}
    }
};

function toggleFarmjobBlip(status, location) {
    if (status) {
        farmjobLocationBlip = addBlip(farmjobLocationBlip, location);
    }
    else {
        removeEntity(farmjobLocationBlip);
        farmjobLocationBlip = null;
        //if (farmjobLocationBlip != null) {
        //    API.deleteEntity(FMarker);
        //    API.deleteEntity(FBlip);
        //    FBlip = null;
        //    FMarker = null;
        //}
    }
};





function togFDelMarker(status, position)
{
    if(status && FDelmarker === null)
    {
        FDelmarker = API.createMarker(1, position, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1), 200, 0, 0, 200);
        FDelBlip = API.createBlip(position);
        delivering = true;
        FDelPos = position;
    }
    if(!status && FDelmarker !== null)
    {
        API.deleteEntity(FDelmarker);
        API.deleteEntity(FDelBlip);
        delivering = false;
        FDelPos = null;
        FDelmarker = null;
    }
}

function makeFVMarker(vpos, id)
{
    if (FVBlip[id] === null)
    {
        FVBlip[id] = API.createBlip(vpos);
    }
    
};

function deleteFVMarker(id)
{
    if (FVBlip[id] !== null)
    {
        API.deleteEntity(FVBlip[id]);
        FVBlip[id] = null;
    }
}

function checkPlayerHatchetTree() {

    var pos = API.getEntityPosition(API.getLocalPlayer());

    var posInFront = resource.utils.getForwardPos(API.getLocalPlayer(), 0.5);

    API.triggerServerEvent("ValidateLumberjackSwing", posInFront);

    resource.skill_effects.canHatchet = false;

}

function ShowNearbyTrees(positions) {
    for (var i = 0; i < activeMarkers.length; i++)
    {
        API.deleteEntity(activeMarkers[i]);
    }

    var pos = API.getEntityPosition(API.getLocalPlayer());
    for (var i = 0; i < positions.length; i++)
    {
        var treePos = new Vector3(positions[i].X, positions[i].Y, positions[i].Z);
        if (pos.DistanceTo(treePos) < 40)
            activeMarkers.push(API.createMarker(1, treePos, new Vector3(), new Vector3(), new Vector3(2, 2, 2), 255, 0, 0, 255));
    }
}



