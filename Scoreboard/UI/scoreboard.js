var scoreboardUI = null;
var scoreboardKey = 91;
var scoreboardShown = false;

var settingsUI = null;

var res = null;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "loadScoreboard")
    {
        if (scoreboardUI === null)
        {
            res = API.getScreenResolution();
            scoreboardUI = API.createCefBrowser(res.Width, res.Height);
            API.sleep(50);
        }
        API.setCefBrowserHeadless(scoreboardUI, true);
        API.loadPageCefBrowser(scoreboardUI, "Scoreboard/UI/scoreboardMain.html");

        API.setCefBrowserPosition(scoreboardUI, -res.Width, -res.Height);
        
        API.triggerServerEvent("ScoreboardPopulate");
        scoreboardShown = true;
    }
    
    else if (eventName === "scoreboardUpdateData")
    {
        if (scoreboardUI !== null)
        {
            API.sleep(300);
            if (args[1] === 0)
                scoreboardUI.call("LoadScoreboard", args[0], false);
            else
                scoreboardUI.call("LoadScoreboard", args[0], true);
        }
    }
    else if (eventName === "onPlayerDisconnect")
    {
        if (scoreboardUI !== null) API.setCefBrowserHeadless(scoreboardUI, true);
        if (settingsUI !== null) API.setCefBrowserHeadless(settingsUI, true);

    }
});

API.onKeyDown.connect(function (sender, key) {
    if (key.KeyCode === Keys.F3)
    {
        scoreboardShown = !scoreboardShown;
        if (!scoreboardShown) {
            API.triggerServerEvent("ScoreboardPopulate");
            API.setCefBrowserPosition(scoreboardUI, 0, 0);
        }
        else {
            API.setCefBrowserPosition(scoreboardUI, -res.Width, -res.Height);
        }
        API.setCefBrowserHeadless(scoreboardUI, scoreboardShown);
    }
});

var showSettings = function () {
    if (settingsUI === null) {
        var res = API.getScreenResolution();
        settingsUI = API.createCefBrowser(res.Width, res.Height);
        API.sleep(50);
    }

    API.loadPageCefBrowser(settingsUI, "UI/Settings/settingsMain.html");
    API.setCefBrowserHeadless(settingsUI, false);
};

var settingsEditPerformance = function () {
    if (settingsUI !== null)
    {
        API.loadPageCefBrowser(settingsUI, "UI/Settings/settingsPerformance.html");
    }
};

var settingsEditHotkeys = function () {
    if (settingsUI !== null)
    {
        API.loadPageCefBrowser(settingsUI, "UI/Settings/settingsPerformance.html");
    }
};

var userLogout = function () {
    resource.bank.closeBank();
    resource.atm.closeATM();
    API.triggerServerEvent("ExecutePlayerLogout");
};

var closeSettings = function () {
    if (settingsUI !== null) API.setCefBrowserHeadless(settingsUI, true);
};

var settingsSetPerformance = function (val) {
    API.triggerServerEvent("SettingsSetPerformance", val);
};

var kickPlayer = function (id) {
    API.triggerServerEvent('KickPlayer', id);
}

var teleportToPlayer = function (id) {
    API.triggerServerEvent('TeleportToPlayer', id);
}

var specPlayer = function (id) {
    API.triggerServerEvent('SpecPlayer', id);
}