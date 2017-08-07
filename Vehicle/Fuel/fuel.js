
var keyInteractPressed = false;
var canRefuel = false;

var previousTime = 0;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "OnEnterGasStation")
    {
        API.displaySubtitle("Please ~r~TURN OFF~w~ your vehicles engine and step out of the vehicle.", 600000);
    }
    else if (eventName === "OnLeaveGasStation")
    {
        API.displaySubtitle("");
        canRefuel = false;
    }
    else if (eventName === "OnExitVehicleGasStation")
    {
        API.displaySubtitle("Press ~r~" + resource.hudDisplay.interactChar + " ~w~whilst next to the vehicle to begin refuelling.", 600000);
        canRefuel = true;
    }
    else if (eventName === "OnExitVehEngineOnGasStation")
    {
        API.displaySubtitle("Please ~r~TURN OFF~w~ your vehicles engine before attempting to refuel.", 600000);
    }
    else if (eventName === "PaymentEnterPin")
    {
        resource.payments.loadMainPage("Globals/Payments/globalEnterPin.html");
        API.sleep(200);
        resource.payments.enteringPin = true;
        resource.payments.paymentsUI.call("InitiatePaymentPin", args[0]);
    }
});

API.onKeyDown.connect(function (sender, keys) {
    if (keys.KeyValue === resource.hudDisplay.interactKey)
    {
        keyInteractPressed = true;
    }
});

API.onKeyUp.connect(function (sender, keys) {
    if (keys.KeyValue === resource.hudDisplay.interactKey)
    {
        keyInteractPressed = false;
    }
});

API.onUpdate.connect(function () {
    if (Math.ceil(API.getGlobalTime() / 100) * 100 % 500 === 0) {
        if (previousTime === Math.ceil(API.getGlobalTime() / 100) * 100) return;
        if (canRefuel && keyInteractPressed) {
            previousTime = Math.ceil(API.getGlobalTime() / 100) * 100;
            API.triggerServerEvent("OnFuelRefill");
        }
    }
});
