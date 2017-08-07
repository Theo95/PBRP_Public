/**
 * Created by Joe on 02/01/2017.
 */

var currMarker = null;
var currMaxSpeed = null;
var currBlip = null;
var currMarkerLoc = null;
var testStartMarker = null;
var speedingCooldown = 0;
var res = null;
var testReport = null;
var testMenu = null;
var testInterface = null;
var pauseChecking = false;
var currTestType = null;

API.onServerEventTrigger.connect(function(eventName, args){
    if(eventName === "place_client_blip"){
        placeCheckpoint(new Vector3(args[0], args[1], args[2]-0.5), args[3]);
    } else if(eventName === "load_test_interface"){
        loadTestInterface();
    } else if(eventName === "end_curr_test"){
        endCurrTest(args[0]);
    } else if(eventName === "show_test_result") {
        generateTestReport(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
    } else if(eventName === "show_test_menu") {
        showTestMenu();
    } else if(eventName === "show_boattest_marker"){
        initBoatTest();
    } else if(eventName === "show_planetest_marker"){
        initPlaneTest();
    } else if(eventName === "show_helitest_marker"){
        initHeliTest();
    }
});

API.onUpdate.connect(function(){
    if(isPlayerInCurrMarker()){
        if(!pauseChecking){
            API.triggerServerEvent("player_entered_blip");
            pauseChecking = true;
        }

    }
    if(currMarker !== null){
        if(speedingCooldown === 0){

            if(isPlayerSpeeding()){
                speedingCooldown = 100;
            }
        } else{
            if(speedingCooldown > 0){
                speedingCooldown--;
            }
        }
    }

    if(testStartMarker !== null){

        var playerPos = API.getEntityPosition(API.getLocalPlayer());
        if(playerPos.DistanceTo(new Vector3(-1607.67,5264.037,3.974102)) < 3 || playerPos.DistanceTo(new Vector3(1716.27,3294.987,41.30125)) < 3){
            API.deleteEntity(testStartMarker);
            API.deleteEntity(currBlip);
            currBlip = null;
            testStartMarker = null;
            if(currTestType === 6){
                API.triggerServerEvent("start_boat_test");
            } else if(currTestType === 5){
                API.triggerServerEvent("start_plane_test");
            }else if(currTestType === 4){
                API.triggerServerEvent("start_heli_test");
            }

        }
    }

});

function launchTest(type){
    currTestType = type;
    API.triggerServerEvent("launch_test_type", type);
    API.destroyCefBrowser(testMenu);
    testMenu = null;
    API.showCursor(false);

}

function replaceLicense(){
    API.triggerServerEvent("replace_driving_license");
}

function initBoatTest(){
    testStartMarker = API.createMarker(1, new Vector3(-1607.67,5264.037,3.974102), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(2.5,2.5,2.5),200, 0,0,255);
    currBlip = API.createBlip(new Vector3(-1607.67,5264.037,3.974102));
    currTestType = 6;
    API.displaySubtitle("Go to the blip on your map to begin the test.", 10000);
}

function initHeliTest(){
    testStartMarker = API.createMarker(1, new Vector3(1716.27,3294.987,41.30125), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(2.5,2.5,2.5), 200, 0,0,255);
    currBlip = API.createBlip(new Vector3(1716.27,3294.987,41.30125));
    currTestType = 4;
    API.displaySubtitle("Go to the blip on your map to begin the test.", 10000);
}

function initPlaneTest(){
    testStartMarker = API.createMarker(1, new Vector3(1716.27,3294.987,41.30125), new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(2.5,2.5,2.5), 200, 0,0,255);
    currBlip = API.createBlip(new Vector3(1716.27,3294.987,41.30125));
    currTestType = 5;
    API.displaySubtitle("Go to the blip on your map to begin the test.", 10000);
}

function showTestMenu(){
    res = API.getScreenResolution();
    testMenu = API.createCefBrowser(res.Width, res.Height);
    API.sleep(50);
    API.loadPageCefBrowser(testMenu, "Vehicle/DrivingTest/Client/testmenu.html");
    API.showCursor(true);
}

function placeCheckpoint(loc, maxSpeed){
    if(currMarker !== null) { API.deleteEntity(currMarker); }
    if(currBlip !== null) { API.deleteEntity(currBlip); }
    currMarkerLoc = loc;

    currMarker = API.createMarker((currTestType === 5 || currTestType === 4) ? 2 : 1, loc, new Vector3(0,0,0), new Vector3(0,0,0), (currTestType === 5 || currTestType === 4) ? new Vector3(8,8,8) : new Vector3(2.5,2.5,2.5), 200, 0,0,255);
    if(currTestType === 1 || currTestType === 2 || currTestType === 3){
        currMaxSpeed = maxSpeed;
        testInterface.call("updateMax", currMaxSpeed);
        //testInterface.call("updateUpcoming", upcomingMax);
    }

    currBlip = API.createBlip(currMarkerLoc);
    API.setBlipSprite(currBlip, 1);
    API.setBlipColor(currBlip, 1);
    pauseChecking = false;
}

function loadTestInterface(){
    res = API.getScreenResolution();
    testInterface = API.createCefBrowser(1000,500);
    API.setCefBrowserPosition(testInterface, res.Width - 150 , 0);
    API.setCefBrowserHeadless(testInterface, false);
    API.sleep(50);
    API.loadPageCefBrowser(testInterface, "Vehicle/DrivingTest/Client/testinterface.html");
}

function isPlayerInCurrMarker() {
    if (currMarkerLoc !== null) {
        var checkRad;
        if(currTestType === 6){checkRad = 6;}
        else if(currTestType === 5 || currTestType === 4){checkRad = 15;}
        else {checkRad = 3;}
        var playerPos = API.getEntityPosition(API.getLocalPlayer());
        if (playerPos.DistanceTo(currMarkerLoc) < checkRad){//Bigger radius for boat test
                return true;
        }
    }
}

function isPlayerSpeeding(){
    if(currTestType === 1 || currTestType === 2 || currTestType === 3){
        if(testInterface !== null){
            var car = API.getPlayerVehicle(API.getLocalPlayer());
            var velocity = API.getEntityVelocity(car);
            var speed = Math.sqrt(
                velocity.X*velocity.X +
                velocity.Y*velocity.Y +
                velocity.Z*velocity.Z
            );
            if((speed*2.23).toFixed(0) > currMaxSpeed + (currMaxSpeed/10)){
                API.triggerServerEvent("player_found_speeding", ((speed*2.23).toFixed(0)) - currMaxSpeed);
                return true;
            } else {
                return false;
            }
        }
    }


}

function endCurrTest(complete) {

    if(testInterface !== null || currTestType === 6 || currTestType === 5 || currTestType === 4){
        API.deleteEntity(currMarker);
        API.deleteEntity(currBlip);
        var vHealth = (API.getVehicleHealth(API.getPlayerVehicle(API.getLocalPlayer())));
        if(complete){
            API.triggerServerEvent("request_test_results", vHealth.toFixed(0));
        } else {
            resetClientsideTest();
        }
        API.setCefBrowserHeadless(testInterface, true);
    } else {
        API.sendChatMessage("You're not taking a test!");
    }
    
}

function generateTestReport(datetime, surname, forename, vehClass, speeding, damage, fail){
    testReport = API.createCefBrowser(res.Width, res.Height);
    API.setCefBrowserHeadless(testReport, false);
    API.sleep(50);
    API.loadPageCefBrowser(testReport, "Vehicle/DrivingTest/Client/testreport.html");
    API.sleep(500); //Wont show otherwise

    testReport.call("populateReport", datetime, surname, forename, vehClass, speeding, damage, fail);
    API.showCursor(true);
    resetClientsideTest();

}

function resetClientsideTest(){
    currMarker = null;
    currBlip = null;
    currMaxSpeed = null;
    currMarkerLoc = null;
    currTestType = null;
}

function closeTestReport(){
    API.destroyCefBrowser(testReport);
    testReport = null;
    API.showCursor(false);
}

function closeTestMenu(){
    API.destroyCefBrowser(testMenu);
    testMenu = null;
    API.showCursor(false);
}