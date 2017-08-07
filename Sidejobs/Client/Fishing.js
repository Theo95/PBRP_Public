var isFishing = false;
var justStartedFishing = false;
var amountTapped = 0;
var amountToTap = 0;
var nextKey = 0;
var pont1 = new Point(0, 500);
var pont2 = new Point(150, 500);
var siz = new Size(128, 128);
var fishName = "";
var fishWeight = 0;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "catchFish") {
        fishName = args[1];
        startFish(args[0]);
    }
    if (eventName === "stopFish") {
        endFishing(false);
    }
    else if (eventName === "startFish")
    {
        justStartedFishing = true;
    }
});

function startFish(weight)
{
    if (!isFishing) {
        isFishing = true;
        this.fishWeight = weight;
        amountToTap = weight * 3;
        amountTapped = 0;
    }
}

function endFishing(status)
{
    if(status)
    {
        API.triggerServerEvent("caughtFish", fishName, fishWeight);
    }
    isFishing = false;
    amountTapped = 0;
}

API.onKeyDown.connect(function (sender, e) {
    if (isFishing) {
        if (e.KeyCode === Keys.Left) {
            if (nextKey === 0) {
                nextKey = 1;
                amountTapped++;
                if (amountTapped >= amountToTap) {
                    endFishing(true);
                }
            }
        }
        if (e.KeyCode === Keys.Right) {
            if (nextKey === 1) {
                nextKey = 0;
                amountTapped++;
                if (amountTapped >= amountToTap) {
                    endFishing(true);
                }
            }
        }
    }
    else if (justStartedFishing)
    {
        if (e.KeyCode === Keys.Left || e.KeyCode === Keys.Right) {
            amountTapped++;

            if (amountTapped >= 6)
                API.triggerServerEvent("stopFish");
        }
    }
});

API.onUpdate.connect(function (sender, args) {    
    if (isFishing)
    {
        if(nextKey === 1)
        {
            API.dxDrawTexture("Sidejobs/Client/right.png", pont2, siz);
        }
        else
        {
            API.dxDrawTexture("Sidejobs/Client/left.png", pont1, siz);
        }
    }
});