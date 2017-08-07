using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using PBRP.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP {
    public class PDArmouryManager : Script {
        public struct ArmouryItem {
            public string itemResponse;
            public string itemName;
            public InventoryType inventoryType;
            public string defaultValue;
            public bool doesNeedSigningOut;
            public bool needsItem;
            public int defaultQuantity;
            public Vector3 position;

            public ArmouryItem(string response, string name, bool signOut, Vector3 pos) {
                itemResponse = response;
                itemName = name;
                inventoryType = InventoryType.VehicleKey;
                defaultValue = "";
                doesNeedSigningOut = signOut;
                needsItem = false;
                defaultQuantity = 0;
                position = pos;
            }

            public ArmouryItem(string response, string name, bool signOut, Vector3 pos, InventoryType type, string value, bool requireItem, int quantity) {
                itemResponse = response;
                itemName = name;
                inventoryType = type;
                defaultValue = value;
                doesNeedSigningOut = signOut;
                needsItem = requireItem;
                defaultQuantity = quantity;
                position = pos;
            }
        }

        public PDArmouryManager() {
            API.onResourceStart += onResourceStart;
            API.onEntityEnterColShape += onEntityEnterColShape;
            API.onEntityExitColShape += onEntityExitColShape;
            API.onChatMessage += onPlayerChatMessage;
        }

        public static void onResourceStart() {
            ArmouryGuy = new NPC(PedHash.CaseyCutscene, "Matthew Harrison", IdlePosition, 90, 22);
            ArmouryInteractionColShape = API.shared.createCylinderColShape(InteractionPosition, 2.5f, 2.5f);

            PopulateLookupTables();
        }

        // --- Ped
        public static Dictionary<string, bool> SignInOutLookupTable = new Dictionary<string, bool>();
        public static Dictionary<string, ArmouryItem> RequestLookupTable = new Dictionary<string, ArmouryItem>();

        public static NPC ArmouryGuy = null;
        public static bool IsGettingItem = false;

        public static ColShape ArmouryInteractionColShape = null;

        public static Dictionary<long, Func<bool>> ArmourerSequenceSteps = new Dictionary<long, Func<bool>>();
        public static Inventory itemRetrieved = null;

        // --- Misc
        public static List<Player> PlayersInInteraction = new List<Player>();

        // --- Positions / Rotations
        public static Vector3 InteractionPosition = new Vector3(452.4297, -980.1032, 30.68958);
        public static Vector3 IdlePosition = new Vector3(454.0594, -980.0482, 30.68961);
        public static Vector3 PistolPosition = new Vector3(458.5096, -979.121, 30.68961);
        public static Vector3 ShotgunPosition = new Vector3(461.8917, -982.3657, 30.68961);
        public static Vector3 MiscPosition = new Vector3(461.873, -979.5708, 30.68961);
        public static Vector3 RiflePosition = new Vector3(461.4247, -983.0906, 30.68959);
        public static Vector3 AmmoPosition = new Vector3(459.9089, -979.1108, 30.68959);

        public static Vector3 IdleRotation = new Vector3(0, 0, 90);
        public static Vector3 PistolRotation = new Vector3();
        public static Vector3 ShotgunRotation = new Vector3(0, 0, -90);
        public static Vector3 MiscRotation = new Vector3(0, 0, -90);
        public static Vector3 RifleRotation = new Vector3(0, 0, 180);
        public static Vector3 AmmoRotation = new Vector3(0, 0, 0);

        public static string[] BusyStatements = {
            "Hang on a second, I'll be right with you",
            "Just let me finish what I'm doing",
            "Can't you see I'm busy? Give me a minute"
        };

        public static string[] UnauthorizedEnterStatements = {
            "Hey. You're not allowed back here, you're not a cop.",
            "If you're looking for the reception you've walked past it pal.",
            "This isn't an ammunation, get out'a here."
        };

        public static string[] AuthorizedEnterStatements = {
            "You signing in or signing out?",
            "What can I do for you pal? In or out?",
            "Heading out or are you done for the day?"
        };

        public static string[] UnderstoodOutStatements = {
            "Coming right up",
            "You got it",
            "You know you have to sign these out, right?",
            "Yeah... that's no problem. Hold on."
        };

        public static string[] UnderstoodInStatements = {
            "You got it",
            "Thanks for that, I'll take it off your hands",
            "Yeah... that's no problem. Hold on."
        };

        public static string[] MisunderstoodStatements = {
            "Uh? Say again? I didn't quite catch that",
            "No idea what you're saying",
            "S-PEAK, CL-EAR-ER",
            "I don't think we have any of those"
        };

        public static string[] RetrievedStatements = {
            "Here you are pal, take care of the equipment. Anything else?",
            "Need anything else mate?",
            "Here you are, need anything else?",
            "What else do you need?",
            "Is that all? My coffee is going cold"
        };

        public static string emotePrefix = "places the ";
        public static string emoteSuffix = " onto the counter, he picks up some forms and lays them down before signing them and filing them";


        // --- Events

        public static void onEntityEnterColShape(ColShape shape, NetHandle entity) {
            if (shape != ArmouryInteractionColShape) { return; }

            Client PlayerC = API.shared.getPlayerFromHandle(entity); if(PlayerC == null) { return; }
            Player Player = Player.PlayerData[PlayerC];
            PlayersInInteraction.Add(Player);

            if (IsGettingItem) { return; }

            if(PlayersInInteraction.Count == 1) {
                bool isAuthorised = FactionManager.IsPlayerAnOfficer(Player);
                ArmouryGuy.Speak(GetRandomMessageFromArray(isAuthorised ? AuthorizedEnterStatements : UnauthorizedEnterStatements));
            }
        }

        public static void onEntityExitColShape(ColShape shape, NetHandle entity) {
            if (shape != ArmouryInteractionColShape) { return; }

            Client PlayerC = API.shared.getPlayerFromHandle(entity); if (PlayerC == null) { return; }
            Player Player = Player.PlayerData[PlayerC];
            PlayersInInteraction.Remove(Player);
        }

        public static void onPlayerChatMessage(Client sender, string message, CancelEventArgs e) {
            e.Cancel = true;

            Player Player = Player.PlayerData[sender];
            if (!PlayersInInteraction.Contains(Player)) { return; }

            bool isAuthorised = FactionManager.IsPlayerAnOfficer(Player);
            if (IsGettingItem) { ArmouryGuy.Speak(GetRandomMessageFromArray(BusyStatements)); return; }
            if (!isAuthorised) { ArmouryGuy.Speak(GetRandomMessageFromArray(UnauthorizedEnterStatements)); return; }

            string ItemName = ParseMessageForItemName(message);
            string InOut = ParseMessageForSigning(message);


            Console.WriteLine("Name: " + ItemName + " | InOut: " + InOut);
            if(ItemName == "UNKNOWN" && InOut == "UNKNOWN") {
                ArmouryGuy.Speak(GetRandomMessageFromArray(MisunderstoodStatements));
                return;
            }

            if(ItemName == "UNKNOWN") {
                ArmouryGuy.Speak("You need to tell me what you want to sign for");
                return;
            }

            if (InOut == "UNKNOWN") {
                ArmouryGuy.Speak("Are you signing in or signing out? It's a vital detail.");
                return;
            }

            bool IsSigningIn = (InOut == "IN");

            if (IsSigningIn) {
                SignInItem(Player, ItemName);
            } else {
                SignOutItem(Player, ItemName);
            }
            e.Cancel = true;
            return;
        }

        // --- Methods
        public static void SignInItem(Player player, string ItemName) {
            if (DoesNeedInventoryItem(ItemName)){
                Inventory Item = GetArmouryItemFromPlayer(player, ItemName, true);
                if (Item == null) { ArmouryGuy.Speak("I don't think you have one of those to return."); return; }
            } else {
                if (ItemName != "KEVLAR") {
                    Weapon weapon = GetArmouryWeaponFromPlayer(player, ItemName, true);
                    if (weapon == null) { ArmouryGuy.Speak("I don't think you have one of those to return."); }
                } else {

                }
            }

            IsGettingItem = true;

            MoveArmouryGuardToItemLocation(ItemName);
            ArmouryGuy.Speak(GetGuardItemResponseFromItemName(ItemName) + "? " + GetRandomMessageFromArray(UnderstoodInStatements));
        }

        public async static void SignOutItem(Player player, string ItemName) {
            if(GetArmouryItemFromPlayer(player, ItemName) != null){
                ArmouryGuy.Speak("I've already given you one of those");
                return;
            } else if (GetArmouryWeaponFromPlayer(player, ItemName) != null) {
                ArmouryGuy.Speak("I've already given you one of those");
                return;
            }


            if (DoesNeedInventoryItem(ItemName)) {
                Inventory Item = CreateArmouryItem(player, ItemName);
                if (Item == null) { return; }

                itemRetrieved = Item;
            } else {
                bool hasStock = CheckArmouryForItem(ItemName);
                if (!hasStock){ ArmouryGuy.Speak("I think we're out of those."); return; }

                if (ItemName != "KEVLAR") {
                    Weapon weapon = await CreateArmouryWeapon(player, ItemName);
                    if (weapon == null) { return; }
                } else {

                }
            }
            IsGettingItem = true;

            MoveArmouryGuardToItemLocation(ItemName);
            ArmouryGuy.Speak(GetGuardItemResponseFromItemName(ItemName) + "? " + GetRandomMessageFromArray(UnderstoodOutStatements));
        }

        public static bool CheckArmouryForItem(string itemName){
            // TODO: Add stock
            return true;
        }

        public static Inventory CreateArmouryItem(Player player, string itemName) {
            Inventory item = null;

            item = new Inventory() {
                Name = GetGuardItemResponseFromItemName(itemName),
                Type = GetInventoryType(itemName),
                Value = GetInventoryDefaultValue(itemName) + "," + Globals.GetUniqueString(),
                Quantity = GetDefaultQuantity(itemName),
                OwnerId = player.Id,
            };

            player.Inventory.Add(item);
            InventoryRepository.AddNewInventoryItem(item);

            if (DoesItemNameNeedSigningOut(itemName) && item != null) { CreateSignOutSheet(item, player); }
            return item;
        }

        public async static Task<Weapon> CreateArmouryWeapon(Player player, string itemName) {
            Weapon wep = null;

            wep = new Weapon() {
                Model = (WeaponHash)GetWeaponModelFromItemName(itemName),
                Ammo = GetWeaponAmmoFromItemName(itemName),
                Type = WeaponType.Faction,
                CurrentOwnerId = player.Id,
                CreatedBy = player.Id
            };

            API.shared.givePlayerWeapon(player.Client, (WeaponHash)wep.Model, wep.Ammo, true, false);
            player.Weapons.Add(wep);


            WeaponLogRepository.AddNew(new WeaponLog(0, await WeaponRepository.AddNew(wep), wep.Type, player.Id));

            if (DoesItemNameNeedSigningOut(itemName) && wep != null) { CreateSignOutSheet(wep, player); }
            return wep;
        }

        public static Inventory GetArmouryItemFromPlayer(Player player, string itemName, bool remove = false) {
            Inventory item = null;

            var list = player.Inventory.Where(i => i.Name == GetGuardItemResponseFromItemName(itemName)).ToList();
            if (list.Any()) {
                item = list.First(); 
            }

            if (!remove) return item;
            if (DoesItemNameNeedSigningOut(itemName) && item != null) { CreateSignInSheet(item, player); }
            return item;
        }

        public static Weapon GetArmouryWeaponFromPlayer(Player player, string itemName, bool remove = false) {
            Weapon wep = null;

            var list = player.Weapons.Where(w => w.Model.GetHashCode() == (int)GetWeaponModelFromItemName(itemName))
                .ToList();
            if(!list.Any()) { return null; }

            wep = list.First();
            player.Weapons.Remove(wep);

            API.shared.removePlayerWeapon(player.Client, wep.Model);
            return wep;
        }

        public static void CreateSignOutSheet(Inventory item, Player player) {
            // TODO: Finish sign out / in
        }

        public static void CreateSignOutSheet(Weapon wep, Player player) {
            // TODO: Finish sign out / in
        }

        public static void CreateSignInSheet(Inventory item, Player player) {
            // TODO: Finish sign out / in
        }
        public static void CreateSignInSheet(Weapon wep, Player player) {
            // TODO: Finish sign out / in
        }

        public static string GetRandomMessageFromArray(string[] array){
            int maxNum = array.Count();
            int rng = new Random().Next(maxNum);

            return array.ElementAt(rng);
        }

        // --- Pathing
       
        public static void BuildActionSequence(Vector3 MoveToPos) {
            ArmouryGuy.AddAction(0, () => {
                ArmouryGuy.MoveTo(MoveToPos.X, MoveToPos.Y, MoveToPos.Z, 2, 6);
            });

            ArmouryGuy.AddAction(8, ReturnGuardToInitialPosition);
            ArmouryGuy.AddAction(8, SetArmourerIdle);

            ArmouryGuy.StartActionSequence();
        }

        public static void MoveArmouryGuardToItemLocation(string ItemName) {
            Vector3 MoveToPos = new Vector3();

            MoveToPos = RequestLookupTable.First(i => i.Value.itemName == ItemName).Value.position;
            BuildActionSequence(MoveToPos);
        }

        public static void ReturnGuardToInitialPosition() {
            ArmouryGuy.MoveTo(IdlePosition.X, IdlePosition.Y, IdlePosition.Z, 2, 6);
            
        }

        public static void SetArmourerIdle() {
            ArmouryGuy.pedHandle.TurnToPosition(InteractionPosition);
            ArmouryGuy.Speak(GetRandomMessageFromArray(RetrievedStatements));           

            IsGettingItem = false;
            itemRetrieved = null;

            ArmouryGuy.ClearActions();
        }

        // --- Armoury Items / Lookup Table

        public static WeaponHash? GetWeaponModelFromItemName(string itemName) {
            WeaponHash? modelHash = null;

            int model = 0;
            switch (itemName) {
                case "FLASHLIGHT":    { model = -1951375401; break; }
                case "NIGHTSTICK":    { model =  1737195953; break; }
                case "TASER":         { model =   911657153; break; }
                case "BEANBAG":       { model =   487013001; break; }
                case "SMOKEG":        { model =   -37975472; break; }
                case "FLARE":         { model =  1233104067; break; }
                case "PISTOL":        { model =   453432689; break; }
                case "COMBAT-PISTOL": { model =  1593441988; break; }
                case "SMG":           { model =   736523883; break; }
                case "SNIPER":        { model =  -952879014; break; }
                case "CARBINE":       { model = -2084633992; break; }
                case "SHOTGUN":       { model =   487013001; break; }
            }

            modelHash = (WeaponHash)model;
            return modelHash;
        }

        public static int GetWeaponAmmoFromItemName(string itemName) {
            int ammoCount = 0;

            switch (itemName) {
                case "FLASHLIGHT":    { ammoCount =  1; break; }
                case "NIGHTSTICK":    { ammoCount =  1; break; }
                case "TASER":         { ammoCount =  1; break; }
                case "BEANBAG":       { ammoCount = 24; break; }
                case "SMOKEG":        { ammoCount =  3; break; }
                case "FLARE":         { ammoCount =  3; break; }
                case "PISTOL":        { ammoCount = 36; break; }
                case "COMBAT-PISTOL": { ammoCount = 36; break; }
                case "SMG":           { ammoCount = 90; break; }
                case "SNIPER":        { ammoCount = 24; break; }
                case "CARBINE":       { ammoCount = 90; break; }
                case "SHOTGUN":       { ammoCount = 24; break; }
            }

            return ammoCount;
        }

        public static string GetGuardItemResponseFromItemName(string itemName) {
            return RequestLookupTable.First(i => (i.Value.itemName == itemName)).Value.itemResponse;
        }

        public static string ParseMessageForItemName(string message) {
            Console.WriteLine(message);

            var Item = RequestLookupTable.FirstOrDefault(i => (message.Contains(i.Key, StringComparison.OrdinalIgnoreCase)));
            if(string.IsNullOrEmpty(Item.Value.itemName)) { return "UNKNOWN"; }
            return Item.Value.itemName;
        }

        public static string ParseMessageForSigning(string message) {
            var signList = SignInOutLookupTable.Where(i => (message.Contains(i.Key, StringComparison.OrdinalIgnoreCase)));
            if(!signList.Any()) { return "UNKNOWN"; }

            var sign = signList.FirstOrDefault();

            return sign.Value ? "OUT" : "IN";
        }

        public static bool DoesItemNameNeedSigningOut(string itemName) {
            return RequestLookupTable.FirstOrDefault(i => (i.Value.itemName == itemName)).Value.doesNeedSigningOut;
        }

        public static InventoryType GetInventoryType(string itemName) {
            return RequestLookupTable.FirstOrDefault(i => (i.Value.itemName == itemName)).Value.inventoryType;
        }

        public static string GetInventoryDefaultValue(string itemName) {
            return RequestLookupTable.FirstOrDefault(i => (i.Value.itemName == itemName)).Value.defaultValue;
        }

        public static int GetDefaultQuantity(string itemName) {
            return RequestLookupTable.FirstOrDefault(i => (i.Value.itemName == itemName)).Value.defaultQuantity;
        }

        public static bool DoesNeedInventoryItem(string itemName) {
            return RequestLookupTable.FirstOrDefault(i => (i.Value.itemName == itemName)).Value.needsItem;
        }

        public static void PopulateLookupTables() {
            SignInOutLookupTable.Add("out", true);
            SignInOutLookupTable.Add("withdraw", true);

            SignInOutLookupTable.Add("in", false);
            SignInOutLookupTable.Add("deposit", false);

            // -- Handcuffs Nicknames
            ArmouryItem item = new ArmouryItem("Handcuffs", "CUFFS", false, MiscPosition, InventoryType.Cuffs, "PDCuffs", true, 1);
            RequestLookupTable.Add("cuffs", item);
            RequestLookupTable.Add("restraint", item);
            RequestLookupTable.Add("bracelet", item);
            RequestLookupTable.Add("braces", item);
            RequestLookupTable.Add("chains", item);
            RequestLookupTable.Add("shackles", item);
            RequestLookupTable.Add("wristlets", item);
            RequestLookupTable.Add("iron rings", item);

            // -- Radio Nicknames
            item = new ArmouryItem("Police Radio", "RADIO", false, MiscPosition, InventoryType.EMSRadio, "#9111|ON", true, 1);
            RequestLookupTable.Add("radio", item);
            RequestLookupTable.Add("handheld", item);
            RequestLookupTable.Add("motorola", item);
            RequestLookupTable.Add("buzzbox", item);
            RequestLookupTable.Add("microphone", item);

            // -- Pager Nicknames
            item = new ArmouryItem("Pager", "PAGER", false, MiscPosition, InventoryType.EMSPager, "#9111|ON", true, 1);
            RequestLookupTable.Add("pager", item);
            RequestLookupTable.Add("beeper", item);

            // -- Flashlight Nicknames
            item = new ArmouryItem("Flashlight", "FLASHLIGHT", true, MiscPosition);
            RequestLookupTable.Add("flashlight", item);
            RequestLookupTable.Add("maglight", item);
            RequestLookupTable.Add("torch", item);

            // -- Nightstick Nicknames
            item = new ArmouryItem("Nightstick", "NIGHTSTICK", true, MiscPosition);
            RequestLookupTable.Add("baton", item);
            RequestLookupTable.Add("asp", item);
            RequestLookupTable.Add("nightstick", item);

            // -- Taser Nicknames
            item = new ArmouryItem("Taser", "TASER", true, MiscPosition);
            RequestLookupTable.Add("taser", item);
            RequestLookupTable.Add("stun", item);
            RequestLookupTable.Add("x26", item);
            RequestLookupTable.Add("CEW", item);
            RequestLookupTable.Add("electrical weapon", item);

            // -- Beanbag Nicknames
            item = new ArmouryItem("Beanbag shotgun", "BEANBAG", true, ShotgunPosition);
            RequestLookupTable.Add("beanbag", item);
            RequestLookupTable.Add("baton round", item);
            RequestLookupTable.Add("rubber", item);
            RequestLookupTable.Add("less lethal", item);

            // -- Smoke grenade Nicknames
            item = new ArmouryItem("Smoke grenade", "SMOKEG", true, PistolPosition);
            RequestLookupTable.Add("smoke", item);

            // -- Road flare Nicknames
            item = new ArmouryItem("Road flare", "FLARE", true, PistolPosition);
            RequestLookupTable.Add("flare", item);

            // -- Kevlar Vest Nicknames
            item = new ArmouryItem("Kevlar vest", "VEST", true, RiflePosition);
            RequestLookupTable.Add("vest", item);
            RequestLookupTable.Add("kevlar", item);
            RequestLookupTable.Add("bulletproof", item);
            RequestLookupTable.Add("stabproof", item);

            // -- Pistol Nicknames
            item = new ArmouryItem("Pistol", "PISTOL", true, PistolPosition);
            RequestLookupTable.Add("pistol", item);
            RequestLookupTable.Add("beretta", item);
            RequestLookupTable.Add("colt", item);

            // -- Compact Pistol
            item = new ArmouryItem("Compact pistol", "COMBAT-PISTOL", true, PistolPosition);
            RequestLookupTable.Add("glock", item);
            RequestLookupTable.Add("compact", item);
            RequestLookupTable.Add("p2000", item);

            // -- Sub machinegun
            item = new ArmouryItem("Sub machinegun", "SMG", true, RiflePosition);
            RequestLookupTable.Add("smg", item);
            RequestLookupTable.Add("sub", item);
            RequestLookupTable.Add("machine pistol", item);
            RequestLookupTable.Add("mp5", item);

            // -- Marksman rifle
            item = new ArmouryItem("Marksman rifle", "SNIPER", true, ShotgunPosition);
            RequestLookupTable.Add("sniper", item);
            RequestLookupTable.Add("marksman", item);
            RequestLookupTable.Add("arctic", item);
            RequestLookupTable.Add("awp", item); // Cause some twat is gonna try it
            RequestLookupTable.Add("scout", item); // Cause some twat is gonna try it

            // -- Assault rifle
            item = new ArmouryItem("Assault rifle", "CARBINE", true, RiflePosition);
            RequestLookupTable.Add("assault", item);
            RequestLookupTable.Add("rifle", item);
            RequestLookupTable.Add("m4", item);
            RequestLookupTable.Add("ar15", item);
            RequestLookupTable.Add("hk416", item);

            // -- Shotgun
            item = new ArmouryItem("Shotgun", "SHOTGUN", true, ShotgunPosition);
            RequestLookupTable.Add("shotgun", item);
            RequestLookupTable.Add("pump", item);
            RequestLookupTable.Add("mossberg", item);
            RequestLookupTable.Add("remmington", item);

            // -- Ammunition
            item = new ArmouryItem("Ammunition", "AMMO", true, AmmoPosition);
            RequestLookupTable.Add("ammunition", item);
            RequestLookupTable.Add("bullets", item);
            RequestLookupTable.Add("hollows", item);
            RequestLookupTable.Add("hollowpoints", item);
            RequestLookupTable.Add("ammo", item);
            RequestLookupTable.Add("dumdums", item);
        }
    }
}
