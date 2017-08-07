var loginBrowser = null;

var email = null;
var password = null;

var startPosition = new Vector3(-875.3318, 5970.708, 126.3448);
var startRotation = new Vector3(0.0, 0.0, 86.77776);
var endPosition = new Vector3(484.1794, 6532.885, 85.74612);
var endRotation = new Vector3(0.0, 0.0, 59.20713);
var currentTime = 0;
var loginSubmitted = false;
var charComplete = false;
var inCharCreation = false;
var rotateChange = 0;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "account_prompt_login") {
        var res = API.getScreenResolution();
        if (loginBrowser !== null) {
            API.destroyCefBrowser(loginBrowser);
        }
        loginBrowser = API.createCefBrowser(res.Width, res.Height);
        API.showCursor(true);
        API.setCanOpenChat(false);
        //API.setChatVisible(false);
        //API.startAudio("res/audio/cr.mp3", true);
        loginCam();
        loginSubmitted = false;
        email = null;
        password = null;
        API.setHudVisible(false);
        API.loadPageCefBrowser(loginBrowser, "Login/client/login.html");

        API.sleep(1000);
        API.loadPageCefBrowser(loginBrowser, "Login/client/login.html");
        API.sleep(500);
        API.setCefBrowserHeadless(loginBrowser, false);
    }
    else if (eventName === "showLoginCam")
    {
        loginCam();
    }
    else if (eventName === "playerLogResult") {
        switch (args[0]) {
            case "invalid-user":
                loginBrowser.call("IncorrectUsername");
                loginSubmitted = false;
                break;
            case "incorrect-pass":
                loginBrowser.call("IncorrectPassword");
                loginSubmitted = false;
                break;
            case "success":
                API.loadPageCefBrowser(loginBrowser, "Login/client/charSelect.html");
                API.sleep(700);
                email = null;
                password = null;
                loginBrowser.call("PopulateCharacters", args[2], args[3], args[1]);
                API.sleep(300);
                loginBrowser.call("PopulateCharacterImages", args[4], args[1]);
                break;
        }
    }
    else if (eventName === "skinCustomOptions")
    {
        loginBrowser.call("skinCustomUpdateOptions", args[0], args[1]);
    }
    else if (eventName === "skinSelectClassSkin")
    {
        API.loadPageCefBrowser(loginBrowser, "Login/client/createCharSkinVars.html");
        API.sleep(500);
        loginBrowser.call("skinSelectClassSkin", args[0], args[1]);
    }
    else if (eventName === "createCharResult")
    {
        if (args[0])
        {
            API.loadPageCefBrowser(loginBrowser, "Login/client/createCharSkinSelect.html");
        }
    }
    else if (eventName === "createCharUsernameResult")
    {
        loginBrowser.call("createCharUsernameResult", args[0]);
    }
    else if (eventName === "onCharCreated")
    {
        API.sleep(400);
        onCharacterCreated();
    }

    else if (eventName === "onPlayerDisconnect")
    {
        API.displaySubtitle("");
        resource.hudDisplay.isLogged = false;
    }
});

var createCharRotateModel = function(value) {
    API.setEntityRotation(API.getLocalPlayer(), new Vector3(0, 0, value));
};

var onLoginSubmitted = function (user, pass) {
    if (!loginSubmitted) {
        email = user;
        password = pass;
        API.triggerServerEvent("onLoginSubmitted", user, pass);
        loginSubmitted = true;
    }
};

var onCharacterSelected = function (id) {
    API.triggerServerEvent("player-selected", id, email);
    onCharacterCreated();
};

var onCharacterCreated = function() {
    API.setActiveCamera(null);
    API.destroyCefBrowser(loginBrowser);
    loginBrowser = null;
    API.showCursor(false);
    API.setCanOpenChat(true);
    API.setChatVisible(true);
    API.stopAudio();
    API.setHudVisible(true);
    resource.hudDisplay.isLogged = true;
};
var createNewChar = function () {
    API.triggerServerEvent("CreateNewCharacter");
    API.setHudVisible(false);
    API.destroyCefBrowser(loginBrowser);
    API.sleep(500);
    API.setActiveCamera(null);
    API.sleep(1000);
    var res = API.getScreenResolution();
    loginBrowser = API.createCefBrowser(res.Width, res.Height);
    API.loadPageCefBrowser(loginBrowser, "Login/client/createNewCharacter.html");
    var vec = new Vector3(0, 0, 0);
    var cam = API.createCamera(new Vector3(11.43243, 6514.132, 32.59785), new Vector3(0,0,125));
    API.pointCameraAtEntity(cam, API.getLocalPlayer(), vec);
    API.setActiveCamera(cam);
    inCharCreation = true;
    charComplete = false;
    API.sleep(2000);
    API.displaySubtitle("Use ~y~LEFT ARROW ~w~ and ~y~RIGHT ARROW~w~ to rotate your character.", 280000);
};

var cancelCharacterCreate = function() {
    API.triggerServerEvent("CancelCharCreate");
};

var createCharUsernameCheck = function(username) {
    API.triggerServerEvent("CreateCharUsernameCheck", username);
};

var showSkinClassSelect = function() {
    API.loadPageCefBrowser(loginBrowser, "Login/client/createCharSkinSelect.html");
};

var createCharChangeGender = function (gender) {
    API.triggerServerEvent("CreateCharacterChangeGender", gender);
};

var createCharProceed = function(name, gender, day, month, year) {
    API.triggerServerEvent("CreateNewCharacterProceed", name, gender, day, month, year);
};

var skinCategorySelect = function (index) {
    API.triggerServerEvent("SkinSelectCategory", index);
};

var skinCustomAvailableOptions = function (comId) {
    API.triggerServerEvent("SkinCustomisationOptions", comId);
};

var skinSelectChange = function(ind) {
    API.triggerServerEvent("SkinSelectChange", ind);
};
var skinCustomTypeChange = function(com, draw, tex) {
    API.triggerServerEvent("SkinCustomTypeChange", com, draw, tex);
};

var skinCustomTextureChange = function(com, draw, tex) {
    API.triggerServerEvent("SkinCustomTextureChange", com, draw, tex);
};

var completeCharacterCreation = function () {
    if (!charComplete) {
        API.triggerServerEvent("CreateCharacterComplete");
        charComplete = true;
        inCharCreation = false;
    }
};

API.onKeyDown.connect(function (sender, keys) {
    if (inCharCreation)
    {
        if (keys.KeyValue === 37) rotateChange = 4;
        else if (keys.KeyValue === 39) rotateChange = -4;
    }
});

API.onKeyUp.connect(function (sender, keys) {
    if (inCharCreation) {

        if (keys.KeyValue === 39 || keys.KeyValue === 37) {
            rotateChange = 0;
            API.playPlayerAnimation("amb@world_human_hang_out_street@male_b@base", "base", 0, -1);
        }
    }
});

API.onUpdate.connect(function () {
    if (inCharCreation)
    {
        if (rotateChange !== 0)
        {
            createCharRotateModel(API.getEntityRotation(API.getLocalPlayer()).Z + rotateChange);
        }
    }
});

var loginCam = function () {
    var main_camera = API.createCamera(startPosition, startRotation);
    var destination_camera = API.createCamera(endPosition, startRotation);

    API.setActiveCamera(main_camera);
    API.interpolateCameras(main_camera, destination_camera, 30000, true, true);
    API.setEntityPosition(API.getLocalPlayer(), new Vector3(-294.9511, 6133.021, -9.555144));
    API.setEntityPositionFrozen(API.getLocalPlayer(), true);
};
