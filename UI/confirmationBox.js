
var confirmBox = null;

var userPrompted = false;

API.onServerEventTrigger.connect(function (eventName, args) {
    
    if (eventName === "reenableCursor")
    {
        API.showCursor(true);
    }
    else if (eventName === "userPrompted")
    {
        userPrompted = args[0];
    }
});

var closeConfirm = function () {
    API.setCefBrowserHeadless(confirmBox, true);
    API.setCanOpenChat(true);
};

