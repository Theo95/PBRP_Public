var inFlyMode = false;
var currentPos;
var forward = false;
var backward = false;
var left = false;
var right = false;
var down = false;
var up = false;
var shift = false;
var maxSpeed = false;

API.onUpdate.connect(function () {
    if (inFlyMode) {

        var multiplier = 0.1;
        if (shift)
            multiplier = 5;

        if (maxSpeed)
            multiplier = 14;

        var player = API.getLocalPlayer();
        var camRotation = API.getGameplayCamRot();
        var pi = 3.1415923;
        var xradian = ((camRotation.X * pi) / 180);
        var yradian = ((camRotation.Z * pi) / 180);
        var zradian = ((camRotation.Z * pi) / 180);
        currentPos = API.getEntityPosition(player);
        var newx;
        var newy;
        var newz;
        if (forward) {
            newx = -(Math.sin(yradian) * multiplier);
            newy = Math.cos(yradian) * multiplier;
            newz = Math.sin(xradian) * multiplier - 1;
            currentPos.X += newx;
            currentPos.Y += newy;
            currentPos.Z += newz;
            API.setEntityPosition(player, currentPos);
        }
        if (backward) {
            newx = Math.sin(yradian) * multiplier;
            newy = -(Math.cos(yradian) * multiplier);
            newz = -(Math.sin(xradian) * multiplier);
            currentPos.X += newx;
            currentPos.Y += newy;
            currentPos.Z += newz;
            API.setEntityPosition(player, currentPos);
        }
    }
});

API.onKeyUp.connect(function (sender, key) {
    if (inFlyMode) {
        if (key.KeyCode === Keys.W) {
            forward = false;
        } if (key.KeyCode === Keys.D) {
            right = false;
        } if (key.KeyCode === Keys.A) {
            left = false;
        } if (key.KeyCode === Keys.S) {
            backward = false;
        } if (key.KeyCode === Keys.ShiftKey) {
            shift = false;
        } if (key.KeyCode === Keys.E) {
            maxSpeed = false;
        }
    }
});

API.onKeyDown.connect(function (sender, key) {
    if (inFlyMode) {
        if (key.KeyCode === Keys.W) {
            forward = true;
        } if (key.KeyCode === Keys.D) {
            right = true;
        } if (key.KeyCode === Keys.A) {
            left = true;
        } if (key.KeyCode === Keys.S) {
            backward = true;
        } if (key.KeyCode === Keys.ShiftKey) {
            shift = true;
        } if (key.KeyCode === Keys.E) {
            maxSpeed = true;
        }
    }
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "activateFlyMode")
    {
        inFlyMode = inFlyMode ? false : true;
    }
});

