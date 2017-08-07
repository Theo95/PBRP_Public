var downed = false;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "injuryMotionBlurStart")
    {
        API.callNative("SET_CAM_MOTION_BLUR", API.getActiveCamera(), API.f(10.0));
    }
    else if (eventName === "playerHasBeenDowned") {
        downed = args[0];
        if (downed) {
            API.callNative("SET_PED_TO_RAGDOLL", API.getLocalPlayer(), -1, -1, 0, true, true, false);
        }
    }
});

function isVaulting(handle)
{
    return API.returnNative("IS_PED_VAULTING", 8, handle);
}

function isClimbing(handle)
{
    return API.returnNative("IS_PED_CLIMBING", 8, handle);
}
