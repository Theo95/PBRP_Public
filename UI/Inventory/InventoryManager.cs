using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;

namespace PBRP
{
    public class InventoryManager : Script
    {
        public InventoryManager()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private async void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "getInventoryTypeActions":
                    {
                        Inventory inv = InventoryRepository.GetInventoryItemById((int)arguments[0]);
                        if (inv.IsPhone())
                        {
                            Phone p = PhoneRepository.GetPhoneByIMEI(long.Parse(inv.Value));
                            API.triggerClientEvent(sender, "displayActionsForItem", inv.Id,
                                p.IsPrimary
                                    ? JsonConvert.SerializeObject("GIVE,DROP,UNSET PRIMARY".Split(','))
                                    : JsonConvert.SerializeObject("GIVE,DROP,SET PRIMARY".Split(',')));
                            return;
                        }

                        API.triggerClientEvent(sender, "displayActionsForItem", inv.Id, JsonConvert.SerializeObject(Inventory.GetActionsByType[inv.Type]));
                    }
                    break;
                case "performActionOnItem":
                    {
                        Inventory inv = InventoryRepository.GetInventoryItemById((int)arguments[0]);
                        switch (arguments[1].ToString())
                        {
                            case "USE":
                                UsePlayerInventoryItem(sender, (int)arguments[0]);
                                break;
                            case "GIVE":
                                GetPlayerToGiveItem(sender, (int)arguments[0]);
                                break;
                            case "DROP":
                                DropInventoryItem(sender, (int)arguments[0]);
                                break;
                            case "SET PRIMARY":
                                SetPhonePrimary(sender, inv, true);
                                break;
                            case "UNSET PRIMARY":
                                SetPhonePrimary(sender, inv, false);
                                break;
                            case "RELEASE":
                                DestroyInventoryItem(sender, (int)arguments[0]);
                                break;
                        }
                    }
                    break;
                case "giveItemToPlayer":
                    if ((int)arguments[0] == 1)
                        GiveInventoryItem(sender, arguments[1].ToString());
                    break;
                case "DisplayPlayerInventory":
                    {
                        Player p = Player.PlayerData[sender];
                        UpdatePlayerInventory(p);
                    }
                    break;
                case "OnInventoryItemMoved":
                    {
                        Player p = Player.PlayerData[sender];
                        Inventory item;
                        try
                        {
                            item = p.Inventory.Single(i => i.Id == (int)arguments[0]);
                        }
                        catch
                        {
                            item = Inventory.DroppedItems.Single(i => i.Id == (int)arguments[0]);
                            Inventory.DroppedItems.Remove(item);
                            API.deleteEntity(item.DroppedObj);

                            if (!Inventory.IsStackable(item))
                            {
                                item.OwnerId = p.Id;
                                item.OwnerType = InventoryOwnerType.Player;
                                p.Inventory.Add(item);

                                API.SendInfoNotification(p.Client, String.Format("You have picked up a {0}", item.Name));
                                API.playPlayerAnimation(p.Client, 0, "pickup_object", "pickup_low");
                                RefreshAfterDropChange(item);
                            }
                        }
                        item.SlotPosition = int.Parse(arguments[1].ToString() + arguments[2].ToString());

                        InventoryRepository.UpdateAsync(item);
                    }
                    break;
                case "OnFriskInventoryItemTaken":
                    {
                        Player p = Player.PlayerData[sender];
                        Inventory item = null;
                        try
                        {
                            item = p.Inventory.Single(i => i.Id == (int)arguments[0]);
                        }
                        catch
                        {
                            item = p.PlayerInteractingWith.Inventory.Single(i => i.Id == (int)arguments[0]);
                        }

                        if (item != null)
                        {
                            int newOwnerId = (int)arguments[1];
                            Player oldOwner = Player.PlayerData.Values.FirstOrDefault(pl => pl.Id == item.OwnerId);
                            Player newOwner = Player.PlayerData.Values.FirstOrDefault(pl => p.Id == newOwnerId);

                            API.SendCloseMessage(sender, 10.0f, "~#C2A2DA~",
                                p != oldOwner
                                    ? string.Format("{0} {1} takes a {2} from {3}.", p.Faction.Ranks[p.FactionRank].Title,
                                        API.getPlayerName(sender), item.Name, oldOwner.Username.Roleplay())
                                    : string.Format("{0} {1} places a {2} on {3}.", p.Faction.Ranks[p.FactionRank].Title,
                                        API.getPlayerName(sender), item.Name, newOwner.Username.Roleplay()));

                            item.OwnerId = newOwner.Id;
                            item.OwnerType = InventoryOwnerType.Player;
                            item.SlotPosition = int.Parse(arguments[2].ToString() + arguments[3].ToString());

                            InventoryRepository.UpdateAsync(item);

                            RefreshPlayerInventory(p.PlayerInteractingWith);
                            ShowFriskInventory(p, p.PlayerInteractingWith);
                        }
                    }
                    break;
                case "OnTrunkInventoryItemTaken":
                    {
                        int oldOwnerId;
                        Player p = Player.PlayerData[sender];
                        string type = arguments[2].ToString();

                        Vehicle vTo = null;
                        Player from = null;
                        Vehicle vFrom = null;
                        Player to = null;
                        Inventory item = null;

                        item = InventoryRepository.GetInventoryItemById((int)arguments[0]);
                        Console.WriteLine(arguments[1].ToString());

                        if (type == "trunk")
                        {
                            vTo = Vehicle.VehicleData.Values.FirstOrDefault(
                                ve => ve.Id == int.Parse(arguments[1].ToString()));
                            from = p;
                        }
                        else
                        {
                            to = Player.PlayerData.Values.FirstOrDefault(
                                pl => pl.Id == int.Parse(arguments[1].ToString()));
                            if(item.OwnerType == InventoryOwnerType.Vehicle)
                                vFrom = Vehicle.VehicleData.Values.FirstOrDefault(ve => ve.Id == item.OwnerId);
                        }

                        oldOwnerId = item.OwnerId;

                        item.OwnerType = vTo != null
                            ? InventoryOwnerType.Vehicle
                            : to != null
                                ? InventoryOwnerType.Player
                                : InventoryOwnerType.Dropped;
                        item.OwnerId = vTo != null ? vTo.Id : to != null ? p.Id : -1;
                        item.SlotPosition = int.Parse(arguments[3].ToString() + arguments[4].ToString());

                        InventoryRepository.UpdateAsync(item);

                        if (oldOwnerId == -1)
                        {
                            Inventory.DroppedItems.Remove(Inventory.DroppedItems.Single(i => i.Id == (int)arguments[0]));
                            API.deleteEntity(item.DroppedObj);

                            API.SendInfoNotification(p.Client, String.Format("You have picked up a {0}", item.Name));
                            API.playPlayerAnimation(p.Client, 0, "pickup_object", "pickup_low");
                            RefreshAfterDropChange(item);
                        }

                        p.Inventory = await InventoryRepository.GetInventoryByOwnerIdAsync(p.Id);

                        if (vFrom != null)
                            vFrom.TrunkItems.Remove(vFrom.TrunkItems.FirstOrDefault(vl => vl.Id == item.Id));
                        if (item.OwnerType == InventoryOwnerType.Vehicle)
                        {
                            if (oldOwnerId != item.OwnerId)
                                vTo.TrunkItems.Add(item);
                            else
                                vTo.TrunkItems.FirstOrDefault(vl => vl.Id == item.Id).SlotPosition = item.SlotPosition;
                        }

                        ShowTrunkInventory(p, p.VehicleInteractingWith);

                        if (p == to) return;

                        string fromString = vTo != null ? $"{vTo.DisplayName}" : $"{vFrom.DisplayName}";
                        API.SendCloseMessage(sender, 10.0f, "~#C2A2DA~",
                            vTo == null
                                ? string.Format("{0} takes a {1} from the {2}.",
                                    API.getPlayerName(sender), item.Name, fromString)
                                : string.Format("{0} places a {1} on the {2}.",
                                    API.getPlayerName(sender), item.Name, fromString));
                        break;
                    }
                case "OnHeaderSlotItemSelected":
                    {
                        Player p = Player.PlayerData[sender];
                        Inventory item = null;
                        try
                        {
                            item = p.Inventory.FirstOrDefault(i => i.Id == (int)arguments[0]);
                        }
                        catch
                        {
                            Console.WriteLine("{0}: Error occurred when trying to open Header Slot with ID: {1}", Server.Date.ToString(), (int)arguments[0]);
                            return;
                        }

                        if (item != null)
                        {
                            if (item.IsBag())
                            {

                            }
                        }
                    }
                    break;
                case "OnInventoryClose":
                    {
                        Player p = Player.PlayerData[sender];
                        if (p.AccessingBank != -1)
                        {

                        }
                        p.InEvent = PlayerEvent.None;
                    }
                    break;
            }
        }

        private void UsePlayerInventoryItem(Client sender, int id)
        {
            Player p = Player.PlayerData[sender];

            Inventory inv = p.Inventory.Where(i => i.Id == id).FirstOrDefault();

            if (inv != null)
            {
                if (p.AwaitingInventorySelection != null)
                {
                    if (inv.Type == p.AwaitingInventorySelection)
                    {
                        switch (p.AwaitingInventorySelection)
                        {
                            case InventoryType.BankCard:
                                if (p.InEvent == PlayerEvent.UsingBank)
                                    BankManager.OnBankCardSelected(p, inv);
                                else if (p.InEvent == PlayerEvent.UsingATM)
                                    ATMManager.OnATMCardSelected(p, inv);
                                return;
                            case InventoryType.DriversLicense:
                                if (p.AccessingBank != -1)
                                    BankManager.OnLicenseSelected(p, inv);
                                return;
                        }
                    }
                    switch (p.AwaitingInventorySelection)
                    {
                        case InventoryType.Money:
                            switch (p.InEvent)
                            {
                                case PlayerEvent.UsingBank:

                                    if (inv.IsCash())
                                    {
                                        if (p.AccessingBank == -1)
                                            BankManager.OnSavingsDepositPlaced(p, inv);
                                        else
                                            BankManager.OnBankDeposit(p, inv);
                                        return;
                                    }
                                    break;

                                case PlayerEvent.RefillingVehicle:

                                    if (inv.IsCash())
                                        FuelManager.BeginRefuelWithCash(p, inv);
                                    else if (inv.Type == InventoryType.BankCard)
                                        FuelManager.BeginRefuelWithCard(p, inv);
                                    return;

                                case PlayerEvent.VehicleDealership:
                                case PlayerEvent.TestDrive:

                                    if (inv.IsCash())
                                        VehicleDealerManager.DealershipPaymentCash(p, inv);
                                    else if (inv.Type == InventoryType.BankCard)
                                        VehicleDealerManager.DealershipPaymentCard(p, inv);
                                    return;
                                case PlayerEvent.BusinessInteract:
                                    switch (p.BusinessInteractingWith.SubType)
                                    {
                                        case BusinessType.Shop:
                                            if (inv.IsCash())
                                                BusinessManager.ShopPaymentCash(p, inv);
                                            else
                                                BusinessManager.ShopPaymentCard(p, inv);
                                            break;
                                        case BusinessType.Mechanic:
                                            if (inv.IsCash())
                                                BusinessManager.GaragePaymentCash(p, inv);
                                            else if (inv.Type == InventoryType.BankCard)
                                                BusinessManager.ShopPaymentCard(p, inv);
                                            break;
                                    }
                                    return;
                            }
                            break;
                    }
                }
                API.SendWarningNotification(sender, String.Format("There's nothing to use this {0} with.", inv.Name), 7);
            }
        }

        public static void SetPhonePrimary(Client sender, Inventory inv, bool set)
        {
            Phone p = PhoneRepository.GetPhoneByIMEI(long.Parse(inv.Value));
            Player player = Player.PlayerData[sender];
            if (player.PrimaryPhone != null)
            {
                player.PrimaryPhone.IsPrimary = false;
                PhoneRepository.UpdateAsync(player.PrimaryPhone);
            }
            p.IsPrimary = set;

            if (p.IsPrimary)
            {
                player.PrimaryPhone = p;
                if (p.PoweredOn)
                {
                    p.Show(sender);
                }
                else { p.TurnOff(sender); }
            }
            else
            {
                p.Hide(sender);
                player.PrimaryPhone = null;
            }

            PhoneRepository.UpdateAsync(p);
        }

        private void GiveInventoryItem(Client sender, string partOfName)
        {
            Player player = Player.PlayerData[sender];
            if (player.ItemBeingGiven > 0)
            {
                Player target;
                target = Player.GetPlayerData(partOfName);
                if (target == null) { Message.PlayerNotConnected(sender); return; }

                Inventory item = player.Inventory.First(i => i.Id == player.ItemBeingGiven);

                item.OwnerId = target.Id;
                item.OwnerType = InventoryOwnerType.Player;

                player.Inventory.Remove(item);
                target.Inventory.Add(item);

                Message.Info(sender, $"You have given your {item.Name} to {target.Client.name}.");
                Message.Info(target.Client, $"You have been given a {item.Name} by {sender.name}.");

                InventoryRepository.UpdateAsync(item);

                UpdatePlayerInventory(player);
            }
        }

        private void GetPlayerToGiveItem(Client sender, int id)
        {
            Player p = Player.PlayerData[sender];
            Inventory inv = p.Inventory.First(i => i.Id == id);
            p.ItemBeingGiven = id;
            HidePlayerInventory(p, true);
            API.triggerClientEvent(sender, "confirmInput", "giveItemToPlayer", $"Give your {inv.Name} to who?",
                $"Please enter the name or ID of the player you wish to give your {inv.Name} to?");
        }

        private void DestroyInventoryItem(Client sender, int id)
        {
            Player player = Player.PlayerData[sender];
            Inventory item = player.Inventory.First(i => i.Id == id);

            player.Inventory.Remove(item);
            InventoryRepository.RemoveInventoryItem(item);

            UpdatePlayerInventory(player);

            API.SendInfoNotification(sender, "You've destroyed: " + item.Name);
        }

        private async void DropInventoryItem(Client sender,  int id)
        {
            if (!sender.isInVehicle)
            {
                if (!sender.inFreefall && !sender.isParachuting && !sender.isOnLadder)
                {
                    Player player = Player.PlayerData[sender];
                    Inventory item = InventoryRepository.GetInventoryItemById(id);

                    API.playPlayerAnimation(player.Client, 0, "pickup_object", "putdown_low");

                    await Task.Run(() =>
                    {
                        Thread.Sleep(350);

                        item.DroppedPos = Player.GetPositionInFrontOfPlayer(sender, 0.4f, -0.74);
                        item.DroppedRot = Inventory.GetRotationForItem[item.Type];
                        item.OwnerId = -1;
                        item.OwnerType = InventoryOwnerType.Dropped;
                        item.DroppedDimension = sender.dimension;

                        item.DroppedObj = API.createObject(Inventory.GetObjectForItem[item.Type], item.DroppedPos, item.DroppedRot, item.DroppedDimension);
                        item.ApplyPhysics();

                        if (item.IsPhone())
                        {
                            Phone p = PhoneRepository.GetPhoneByIMEI(long.Parse(item.Value));
                            if (p.IsPrimary) API.triggerClientEvent(sender, "hidePhoneUI");

                            p.IsPrimary = false;
                            PhoneRepository.UpdateAsync(p);
                        }
                        List<Inventory> inv = null;
                        if (player.InEvent == PlayerEvent.AccessingInventory)
                        {
                            if (player.VehicleInteractingWith != null)
                                inv = player.VehicleInteractingWith.TrunkItems;
                            else
                                inv = player.Inventory;
                        }
                        inv?.Remove(inv.FirstOrDefault(il => il.Id == item.Id));
                        InventoryRepository.UpdateAsync(item);
                        Inventory.DroppedItems.Add(item);

                        RefreshAfterDropChange(item);
                    });
                }
                else API.SendErrorNotification(sender, "You have to be on the ground to drop items.");
            }
            else API.SendErrorNotification(sender, "You drop items in a vehicle");
        }

        public static void RefreshAfterDropChange(Inventory item)
        {
            foreach (Client pl in API.shared.getAllPlayers())
            {
                if (pl.position.DistanceTo(item.DroppedPos) < 2.5f)
                {
                    Player inRangePlayer = Player.PlayerData[pl];
                    if (inRangePlayer.InEvent == PlayerEvent.AccessingInventory)
                    {
                        if (inRangePlayer.PlayerInteractingWith != null)
                            ShowFriskInventory(inRangePlayer, inRangePlayer.PlayerInteractingWith);
                        else if (inRangePlayer.VehicleInteractingWith != null)
                            ShowTrunkInventory(inRangePlayer, inRangePlayer.VehicleInteractingWith);
                        else
                            RefreshPlayerInventory(inRangePlayer);
                    }
                }
            }
        }

        public static void UpdatePlayerInventory(Player player)
        {
            Task.Run(async () =>
            {
                var json = JsonConvert.SerializeObject(player.Inventory.Select(i => new
                {
                    i.Id,
                    i.Name,
                    i.Type,
                    i.Quantity,
                    i.Value,
                    i.SlotPosStr,
                    i.SlotSpan,
                    i.Image,
                    i.OwnerName,
                    i.OwnerGender
                }));
                var droppedjson = JsonConvert.SerializeObject(Inventory.DroppedItems
                    .Where(i => i.DroppedPos.DistanceTo(player.Client.position) < 2.0f)
                    .Select(i => new { i.Id, i.Name, i.Image, i.SlotSpan }));
                API.shared.triggerClientEvent(player.Client, "showPlayerInventory");
                await Task.Delay(200 + player.Client.ping);
                API.shared.triggerClientEvent(player.Client, "populateInventoryItems",
                   json, droppedjson, JsonConvert.SerializeObject(Inventory.GetActionsByType));
            });
        }

        public static void RefreshPlayerInventory(Player player)
        {
            Task.Run(async () =>
            {
                var json = JsonConvert.SerializeObject(player.Inventory.Select(i => new
                {
                    i.Id,
                    i.Name,
                    i.Type,
                    i.Quantity,
                    i.Value,
                    i.SlotPosStr,
                    i.SlotSpan,
                    i.Image,
                    i.OwnerName,
                    i.OwnerGender
                }));
                var droppedjson = JsonConvert.SerializeObject(Inventory.DroppedItems
                    .Where(i => i.DroppedPos.DistanceTo(player.Client.position) < 2.0f)
                    .Select(i => new { i.Id, i.Name, i.Image, i.SlotSpan }));
                API.shared.triggerClientEvent(player.Client, "updatePlayerInventory");
                await Task.Delay(200 + player.Client.ping);
                API.shared.triggerClientEvent(player.Client, "populateInventoryItems",
                    json, droppedjson, JsonConvert.SerializeObject(Inventory.GetActionsByType));
            });
        }

        public static void ShowFriskInventory(Player player, Player target)
        {
            API.shared.triggerClientEvent(player.Client, "showFriskInventory",
                JsonConvert.SerializeObject(player.Inventory.Select(i => new { i.Id, i.OwnerId, i.Name, i.Type, i.Quantity, i.Value, i.SlotPosStr, i.SlotSpan, i.Image, i.OwnerName, i.OwnerGender })),
                JsonConvert.SerializeObject(Inventory.DroppedItems.Where(i => i.DroppedPos.DistanceTo(player.Client.position) < 2.0f).Select(i => new { i.Id, i.Name, i.Image, i.SlotSpan })),
                JsonConvert.SerializeObject(target.Inventory.Select(i => new { i.Id, i.OwnerId, i.Name, i.Type, i.Quantity, i.Value, i.SlotPosStr, i.SlotSpan, i.Image, i.OwnerName, i.OwnerGender })),
                JsonConvert.SerializeObject(Inventory.GetActionsByType));
        }

        public static void ShowTrunkInventory(Player player, Vehicle vehicle)
        {
            var yourinv = JsonConvert.SerializeObject(player.Inventory.Select(i => new
            {
                i.Id,
                i.Name,
                i.Type,
                i.Quantity,
                i.Value,
                i.SlotPosStr,
                i.SlotSpan,
                i.Image,
                i.OwnerId,
                i.OwnerType,
                i.OwnerName,
                i.OwnerGender
            }));
            var json = JsonConvert.SerializeObject(vehicle.TrunkItems.Select(i => new
            {
                i.Id,
                i.Name,
                i.Type,
                i.Quantity,
                i.Value,
                i.SlotPosStr,
                i.SlotSpan,
                i.Image,
                i.OwnerId,
                i.OwnerType,
                i.OwnerName,
                i.OwnerGender
            }));
            var dropped = JsonConvert.SerializeObject(Inventory.DroppedItems
                .Where(i => i.DroppedPos.DistanceTo(player.Client.position) < 2.0f)
                .Select(i => new { i.Id, i.Name, i.Image, i.SlotSpan }));

            API.shared.triggerClientEvent(player.Client, "ShowTrunkInventory", yourinv, dropped, json, player.Id, player.VehicleInteractingWith.Id);
        }

        public static void HidePlayerInventory(Player player, bool keepMouseEnabled = false)
        {
            API.shared.triggerClientEvent(player.Client, "hideInventoryMenus", keepMouseEnabled);
        }
    }
}
