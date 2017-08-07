var camera = null;
var weapskill = null;
var attackTimeout = 0;
var isFalling = 0;
var previousTime = 0;

var blurDownSight = 0;
var blurDownSightWait = 0;
var HatchetTimeout = 4;
var isAiming = false;

var canHatchet = true;

API.onServerEventTrigger.connect(function (eventName, args) {
    API.sendChatMessage("" + eventName);
    if (eventName === "updateWeaponSkill") {
        weapskill = args[0].split(",");
    }
    else if (eventName === "onPlayerStopAim") {
        API.setActiveCamera(null);
        //API.triggerServerEvent("PlayerAimingStopBlur", 1000.0);
        blurDownSight = 0;
    }
    else if (eventName === "validatePlayerAimingAt") {
        var aimPos = API.getPlayerAimingPoint(API.getLocalPlayer());
        var camPos = API.getGameplayCamPos();

        aimPos = new Vector3(
            ((aimPos.X - camPos.X) * 20) + camPos.X,
            ((aimPos.Y - camPos.Y) * 20) + camPos.Y,
            ((aimPos.Z - camPos.Z) * 20) + camPos.Z
        );

        var rayCast = API.createRaycast(camPos, aimPos, 1 | 2 | 10 | 12 | 16 | 32 | 64 | 128 | 512, API.getLocalPlayer());

        if (rayCast.didHitEntity) {
            var hitPlayer = rayCast.hitEntity;
            if (hitPlayer !== null) {
                try {
                    if (API.getEntityType(hitPlayer) === 1) {
                        API.triggerServerEvent("PlayerAttackedInVehicle", hitPlayer, rayCast.hitCoords);
                    }
                    else if (API.getEntityType(hitPlayer) === 6) {
                        API.triggerServerEvent("isPlayerAimingAtPlayer", hitPlayer);
                       
                    }
                }
                catch (err) {
                    API.sendChatMessage("" + API.getEntityModel(hitPlayer));
                }
            }
        }
        else API.triggerServerEvent("isPlayerAimingAtPlayer", -1);
    }
    else if (eventName === "playScreenEffect") {
        //API.playScreenEffect(args[0], args[1], args[2]);
    }
});


API.onUpdate.connect(function () {
    skillTimer();
    if (resource.hudDisplay.isLogged) {
        if (camera !== null)
        {
            API.setCameraRotation(camera, API.getGameplayCamRot());
            API.setCameraPosition(camera, API.getGameplayCamPos());
        }
        if (API.isControlJustPressed(24)) {
            var weap = API.getPlayerCurrentWeapon();

            if (weap === -102973651) if (canHatchet) resource.sidejob.checkPlayerHatchetTree();
        }

        if (weapskill === null) return;
        if (resource.utils.getWeaponClass(API.getPlayerCurrentWeapon()) >= 8)
            return;
        if (weapskill[resource.utils.getWeaponClass(API.getPlayerCurrentWeapon())] <= 60) {
            API.callNative("HIDE_HUD_COMPONENT_THIS_FRAME", 14);
        } 
        if ((API.returnNative("IS_AIM_CAM_ACTIVE", 8) || API.returnNative("IS_FIRST_PERSON_AIM_CAM_ACTIVE", 8) ||
            API.isControlJustPressed(24)) && !isAiming) {

            if (weapskill[resource.utils.getWeaponClass(API.getPlayerCurrentWeapon())] <= 60) {
                if (camera === null) camera = API.createCamera(API.getGameplayCamPos(), API.getGameplayCamRot());
                API.setActiveCamera(camera);

                API.setCameraShake(camera, "HAND_SHAKE", (60 - weapskill[resource.utils.getWeaponClass(API.getPlayerCurrentWeapon())]) / 9.6);
            }
            isAiming = true;
        }
        else if (!API.returnNative("IS_AIM_CAM_ACTIVE", 8) && !API.returnNative("IS_FIRST_PERSON_AIM_CAM_ACTIVE", 8) || (API.isControlJustReleased(24) || API.isControlJustReleased(25)) && isAiming) {
            isAiming = false;
            if (camera !== null)
                API.setActiveCamera(null);
        }
    }
});

API.onLocalPlayerMeleeHit.connect(function (attacker, weapon) {
    API.triggerServerEvent("PlayerJustAttacked", attacker);
});

API.onLocalPlayerShoot.connect(function (weapon, aimCoords) {
    API.triggerServerEvent("PlayerJustAttacked", API.getLocalPlayer());
});

function skillTimer() {
    if (Math.ceil(API.getGlobalTime() / 100) * 100 % 500 === 0) {
        if (previousTime === Math.ceil(API.getGlobalTime() / 100) * 100) return;
        if (!canHatchet) {
            if (HatchetTimeout > 0) HatchetTimeout--;
            else {
                canHatchet = true;
                HatchetTimeout = 4;
            }
        }

        if (API.returnNative("IS_PED_FALLING", 8, API.getLocalPlayer())
            && API.returnNative("GET_ENTITY_HEIGHT_ABOVE_GROUND", 7, API.getLocalPlayer()) > 2) {
            isFalling++;
        }
        else if (API.returnNative("GET_ENTITY_HEIGHT_ABOVE_GROUND", 7, API.getLocalPlayer()) < 2 && isFalling > 0) {
            isFalling--;
            API.triggerServerEvent("PlayerHasFallen", isFalling);
            isFalling = 0;
        }

        //if (blurDownSight > 0) {
        //    blurDownSight -= 1;
        //}
        //if (blurDownSight < 0) {
        //    if (blurDownSightWait === 0) {
        //        blurDownSightWait = (40 - weapskill) / 7.5;
        //        API.triggerServerEvent("PlayerAimingBlur", 2500.0);
        //    }
        //    else {
        //        API.triggerServerEvent("PlayerAimingBlur", 0.0);
        //    }
        //}

        //if (blurDownSightWait > 0) {
        //    blurDownSightWait--;
        //}
        //else if (blurDownSightWait < 0) {
        //    blurDownSightWait = 0;
        //}
        if (resource.injury.downed) {
            if (!API.returnNative("IS_PED_RAGDOLL", 8, API.getLocalPlayer())) {
                API.callNative("SET_PED_TO_RAGDOLL", API.getLocalPlayer(), -1, -1, 0, true, true, false);
            }
        }
        previousTime = Math.ceil(API.getGlobalTime() / 100) * 100;
    }
};


