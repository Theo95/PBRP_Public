using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PBRP
{
    public class BusinessManager : Script {

        public BusinessManager()
        {
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            API.onClientEventTrigger += BaseAndShopClientTriggerEvent;
            API.onClientEventTrigger += OnRepairShopClientEventTrigger;
        }

        private void OnRepairShopClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "PerformGarageRepair")
            {
                Vehicle veh = Vehicle.VehicleData[sender.vehicle];
                veh.RepairType = (VehicleRepairType)arguments[0];
                for (int i = 0; i < 7; i++)
                {
                    veh.TyresData[i] = API.isVehicleTyrePopped(veh.Entity, i) ? 1 : 0;
                }
                string typeName = "";
                switch (veh.RepairType)
                {
                    case VehicleRepairType.Engine:
                        veh.RepairCost = Math.Round((veh.Entity.Price() * 0.3) * ((1000 - veh.Entity.health) / 1000));
                        veh.RepairTime = Server.Date.AddMinutes(12);
                        typeName = "Engine";
                        break;
                    case VehicleRepairType.Body:
                        veh.RepairTime = Server.Date.AddMinutes(12);
                        typeName = "Body";
                        break;
                    case VehicleRepairType.Tyres:
                        int poppedTyres = veh.TyresData.Where(t => API.isVehicleTyrePopped(veh.Entity, t)).Count();
                        veh.RepairCost = 60 * poppedTyres;
                        typeName = "Tyres";
                        veh.RepairTime = Server.Date.AddMinutes(poppedTyres);
                        break;
                }

                API.ShowPopupPrompt(sender, "ConfirmGarageRepair", String.Format("Confirm {0} Repair?", typeName),
                    String.Format("You are about to repair the {0} of your {1}, this will cost {2}.<br />Do you want to continue?", typeName.ToLower(), veh.Name, veh.RepairCost.ToString("C")));
            }
            else if (eventName == "ConfirmGarageRepair")
            {                
                Player player = Player.PlayerData[sender];
                if ((int)arguments[0] == 1)
                { 
                    player.SelectedCardAccount = null;
                    player.SelectedCash = null;

                    player.AwaitingInventorySelection = InventoryType.Money;
                    API.SendInfoNotification(sender, "Please select your payment method.", 7);
                    InventoryManager.UpdatePlayerInventory(player);
                }
                else
                {
                    Vehicle veh = Vehicle.VehicleData[sender.vehicle];
                    veh.RepairType = 0;

                    VehicleRepository.UpdateAsync(veh);
                }
            }
            else if (eventName == "RetrieveVehicleFromRepairGarage")
            {
                Player p = Player.PlayerData[sender];
                try
                {
                    Vehicle veh = Vehicle.VehicleData.Values.FirstOrDefault(v => v.Id == (int)arguments[0]);

                    if (!veh.RepairComplete) return;
                    string repairType = "";
                    switch(veh.RepairType)
                    {
                        case VehicleRepairType.Engine: repairType = "engine"; break;
                        case VehicleRepairType.Body: repairType = "body"; break;
                        case VehicleRepairType.Tyres: repairType = "tyres"; break;
                    }

                    API.ShowPopupMessage(sender, "Repair complete!",
                        string.Format("Your {0}'s ", veh.Name) +
                        string.Format("{0} {1} successfully repaired<br/>&emsp;", repairType,
                            veh.RepairType == VehicleRepairType.Tyres ? "were" : "was") +
                        string.Format("Repair Cost: {0:C}<br/>&emsp;Time of collection: {1:dd/MM/yy HH:mm}<br/>&emsp;",
                            veh.RepairCost, Server.Date) +
                        string.Format("Collected by: {0}", sender.name));

                    API.triggerClientEvent(sender, "closeShopUI");

                    veh.RepairType = 0;
                    veh.Dimension = 0;
                    VehicleRepository.UpdateAsync(veh);

                    veh = p.ReloadPlayerVehicle(veh);

                    API.delay(500, true, () =>
                    {
                        SpawnOutsideOfGarage(sender, veh);
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                }
            }
            else if (eventName == "ExitRepairShop")
            {
                Player p = Player.PlayerData[sender];
                sender.FadeOut(200);

                Vehicle veh = Vehicle.VehicleData[sender.vehicle];

                veh.RepairType = 0;

                SpawnOutsideOfGarage(sender, veh);
            }
        }

        private void BaseAndShopClientTriggerEvent(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "OnBusinessInteraction")
            {
                Player player = Player.PlayerData[sender];
                if (player.BusinessInteractingWith != null)
                {
                    player.SetCurrentWeapon(WeaponHash.Unarmed);
                    player.InEvent = PlayerEvent.BusinessInteract;
                    switch (player.BusinessInteractingWith.SubType)
                    {
                        case BusinessType.Shop:
                            InteractWithShop(player);
                            break;
                        case BusinessType.Mechanic:
                            InteractWithGarage(player);
                            break;
                    }
                }
            }
            else if (eventName == "OnVerifyShopStock")
            {
                Player player = Player.PlayerData[sender];
                if (player.BusinessInteractingWith != null)
                {
                    int change = (int)arguments[1];
                    ShopItem item = player.BusinessInteractingWith.BusinessItems.FirstOrDefault(i => i.Id == (int)arguments[0]);
                    item.Quantity -= change;
                    item.ReservedStock += change;
                    if (item.Quantity >= 0)
                    {
                        API.triggerClientEvent(sender, "returnShopStockCount", JsonConvert.SerializeObject(new { item.Id, item.Price, item.Name }), true, change);
                    }
                    else
                    {
                        item.Quantity += change;
                        item.ReservedStock -= change;
                        API.triggerClientEvent(sender, "returnShopStockCount", JsonConvert.SerializeObject(new { item.Id, item.Price, item.Name }), false, change);
                    }
                }
                else API.triggerClientEvent(sender, "closeShopUI");
            }
            else if (eventName == "CancelShopTransaction")
            {
                Player player = Player.PlayerData[sender];
                if (player.BusinessInteractingWith != null)
                {
                    int[] ids = JsonConvert.DeserializeObject<int[]>(arguments[0].ToString());
                    int[] reserveStock = JsonConvert.DeserializeObject<int[]>(arguments[1].ToString());

                    for (int i = 0; i < ids.Length; i++)
                    {
                        ShopItem item = player.BusinessInteractingWith.BusinessItems.FirstOrDefault(it => it.Id == ids[i]);
                        item.Quantity += reserveStock[i];
                        item.ReservedStock -= reserveStock[i];
                    }
                }
                else API.triggerClientEvent(sender, "closeShopUI");
                player.InEvent = PlayerEvent.None;
            }
            else if (eventName == "CompleteShopTransaction")
            {
                Player player = Player.PlayerData[sender];
                if (player.BusinessInteractingWith != null)
                {
                    int[] ids = JsonConvert.DeserializeObject<int[]>(arguments[0].ToString());
                    int[] quantity = JsonConvert.DeserializeObject<int[]>(arguments[1].ToString());
                    if(ids.Length < 1) { API.triggerClientEvent(sender, "closeShopUI"); return; } 
                    List<Inventory> tempInv = new List<Inventory>();
                    List<ShopItem> shoppingCart = new List<ShopItem>();

                    for(int i = 0; i < ids.Length; i++)
                    {
                        ShopItem item = player.BusinessInteractingWith.BusinessItems.FirstOrDefault(it => it.Id == ids[i]);
                        for (int j = 0; j < quantity[i]; j++)
                        {
                            Inventory tempInvItem = new Inventory()
                            {
                                Type = item.Type,
                                Quantity = 1,
                                OwnerId = player.Id
                            };

                            tempInv.Add(tempInvItem);

                            if (!tempInvItem.AddToPlayer(player))
                            {
                                API.SendErrorNotification(sender, "Not enough space in your inventory for these items", 7);
                                foreach (Inventory inv in tempInv) player.Inventory.Remove(inv);
                                return;
                            }
                        }
                        item.Quantity = quantity[i];
                        shoppingCart.Add(item);
                    }

                    foreach (Inventory inv in tempInv) player.Inventory.Remove(inv);

                    player.ShoppingCart = shoppingCart;
                    player.SelectedCardAccount = null;
                    player.SelectedCash = null;

                    player.AwaitingInventorySelection = InventoryType.Money;
                    API.SendInfoNotification(sender, "Please select your payment method.", 7);
                    InventoryManager.UpdatePlayerInventory(player);
                }
            }
            
        }

        private static void PerformTyreRepair(Vehicle veh)
        {
            for (int i = 0; i < 7; i++)
            {
                API.shared.popVehicleTyre(veh.Entity, i, false);
            }
            veh.TyreData = "0,0,0,0,0,0,0";
        }

        private static void PerformBodyRepair(Vehicle veh)
        {
            veh.BodyHealth = 1000;
            veh.DirtLevel = 0;
            for (int i = 0; i < veh.DoorData.Length; i++)
                API.shared.breakVehicleDoor(veh.Entity, i, false);

            veh.windowData = "0,0,0,0,0,0";
            for (int i = 0; i < veh.WindowData.Length; i++)
                API.shared.breakVehicleWindow(veh.Entity, i, false);

            veh.doorData = "0,0,0,0,0,0,0,0";
        }

        private static void PerformEngineRepair(Vehicle veh)
        {
            veh.Health = 1000;
            veh.Entity.health = 1000;
        }

        private void SpawnOutsideOfGarage(Client sender, Vehicle veh)
        {
            Player p = Player.PlayerData[sender];
            Vector3 pos = p.BusinessInteractingWith.EnterPosition;
            Task.Run(() =>
            {
                do
                {
                    Thread.Sleep(300);
                    bool carTooNear = false;

                    foreach (Vehicle v in Vehicle.VehicleData.Values)
                    {
                        if (v.Entity.position.DistanceTo(pos) <= 1f)
                        {
                            carTooNear = true;
                            pos = pos.Add(new Vector3(3, 0, -3));
                            break;
                        }
                    }
                    if (!carTooNear)
                    {
                        veh.Entity.position = pos;
                        veh.Entity.dimension = 0;
                        sender.dimension = 0;
                        foreach (Client passenger in veh.Entity.occupants)
                        {
                            passenger.dimension = 0;
                        }
                        veh.Entity.engineStatus = true;
                        sender.FadeIn(200);
                        API.delay(300, true, () => sender.setIntoVehicle(veh.Entity, -1));
                    }
                }
                while (sender.dimension != 0);
            });
            InventoryManager.HidePlayerInventory(p);
        }
    

        private void InteractWithGarage(Player player)
        {
            if(player.BusinessInteractingWith != null && player.InEvent == PlayerEvent.BusinessInteract)
            {
                if (player.Client.isInVehicle)
                {
                    player.Client.FadeOutIn(200, 1200);

                    API.triggerClientEvent(player.Client, "initRepairShopUI", Vehicle.VehicleData[player.Client.vehicle].Name);

                    Task.Run(() =>
                    {
                        Thread.Sleep(300);
                        player.Client.dimension = player.BusinessInteractingWith.Id * 10 + player.RealId + 1;
                        player.Client.vehicle.dimension = player.Client.dimension;
                        API.sendChatMessageToAll(API.getVehicleClassName(player.Client.vehicle.Class));
                        foreach (Client p in player.Client.vehicle.occupants)
                        {
                            p.dimension = player.Client.dimension;
                        }

                        player.Client.vehicle.position = player.BusinessInteractingWith.ExitPosition;
                        player.Client.vehicle.moveRotation(new Vector3(0, 0, (float)player.BusinessInteractingWith.ExitRZ), 50);

                        player.Client.vehicle.engineStatus = false;
                    });
                }
                else
                {
                    API.triggerClientEvent(player.Client, "initGarageVehicleCollectionUI", 
                        JsonConvert.SerializeObject(Vehicle.VehicleData.Values.Where(v => (v.OwnerId == player.Id || v.FactionId == player.FactionId) && v.RepairType != 0)
                        .Select(v => new { v.RepairCost, v.LicensePlate, v.RepairTime, v.RepairComplete, v.Id, v.Name })));
                }
            }
        }

        public static void GaragePaymentCash(Player p, Inventory inv)
        {
            Vehicle veh = Vehicle.VehicleData[p.Client.vehicle];

            if (long.Parse(inv.Value) >= veh.RepairCost)
            {
                inv.Value = (long.Parse(inv.Value) - (long)veh.RepairCost).ToString();

                if (inv.Value == "0")
                {
                    p.Inventory.Remove(inv);
                    InventoryRepository.RemoveInventoryItem(inv);
                }
                else InventoryRepository.UpdateAsync(inv);

                switch (veh.RepairType)
                {
                    case VehicleRepairType.Engine: PerformEngineRepair(veh); break;
                    case VehicleRepairType.Body: PerformBodyRepair(veh); break;
                    case VehicleRepairType.Tyres: PerformTyreRepair(veh); break;
                }

                VehicleRepository.UpdateAsync(veh);

                p.Client.warpOutOfVehicle();
                veh.Entity.Delete(false);
                p.Client.position = p.BusinessInteractingWith.EnterPosition;
                p.Client.dimension = 0;
                API.shared.triggerClientEvent(p.Client, "closeShopUI");
                InventoryManager.HidePlayerInventory(p);

                API.shared.ShowPopupMessage(p.Client, "Repair In Progress...",
                    String.Format("Your {0} is now booked in for repair!<br />The vehicle should be available for collection anytime after {1}.", veh.Name, veh.RepairTime.ToString("dd/MM/yy HH:mm")));
                p.InEvent = PlayerEvent.None;
            }
            else
            {
                API.shared.SendErrorNotification(p.Client, "Insufficient funds.");
                API.shared.SendInfoNotification(p.Client, "Select new payment method:");
                InventoryManager.UpdatePlayerInventory(p);
            }
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client client = API.getPlayerFromHandle(entity);
                foreach (Business bus in PropertyManager.Businesses)
                {
                    if (bus.InteractionPoint == null || (bus.SubType == BusinessType.Mechanic && client.dimension != 0)) continue;
                    if (colshape == bus.InteractionPoint)
                    {
                        Player.PlayerData[client].BusinessInteractingWith = null;
                        API.triggerClientEvent(client, "displayBusinessInteract", "");
                        break;
                    }
                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client client = API.getPlayerFromHandle(entity);
                foreach (Business bus in PropertyManager.Businesses)
                {
                    if (bus.InteractionPoint == null) continue;
                    if (colshape == bus.InteractionPoint)
                    {
                        if (client.dimension != bus.Dimension) continue;
                        string message = "";
                        Player player = Player.PlayerData[client];
                        switch (bus.SubType)
                        {
                            case BusinessType.Shop:
                                message = "Press ~r~" + (char)player.MasterAccount.KeyInteract + " ~w~to purchase items";
                                break;
                            case BusinessType.Mechanic:
                                if (client.isInVehicle) { 
                                    if (!Vehicle.VehicleData[client.vehicle].IsAdminVehicle && !Vehicle.VehicleData[client.vehicle].IsDealerVehicle)
                                    {
                                        message = String.Format("Press ~r~{0} ~w~ to use ~y~{1} Repair Garage", (char)player.MasterAccount.KeyInteract, bus.Name);
                                    }
                                } else {
                                    message = String.Format("Press ~r~{0} ~w~ to retrieve your vehicle from ~y~{1} Repair Garage", (char)player.MasterAccount.KeyInteract, bus.Name);
                                }
                                break;
                        }

                        if (message != "")
                        {
                            player.BusinessInteractingWith = bus;
                            API.triggerClientEvent(client, "displayBusinessInteract", message);
                        }
                        break;
                    }
                }
            }
        }

        public static void ConfigureBusiness(Business bus)
        {
            switch (bus.SubType)
            {
                case BusinessType.Shop:
                    if (bus.Interior > 0)
                        bus.InteractionPoint = API.shared.createCylinderColShape(Business.InteriorInteractPos[bus.Interior], 1f, 1f);
                    break;
                case BusinessType.Mechanic:
                    bus.InteractionPoint = API.shared.createCylinderColShape(bus.EnterPosition, 1f, 1f);
                    break;
            }
            bus.BusinessItems = ShopItemRepository.GetShopItemsByBusinessId(bus.Id);
        }

        #region Shop functionality
        private void InteractWithShop(Player player)
        {
            if (player.BusinessInteractingWith != null && player.InEvent == PlayerEvent.BusinessInteract)
            {
                API.triggerClientEvent(player.Client, "initShopUI", JsonConvert.SerializeObject(player.BusinessInteractingWith.BusinessItems));
            }
        }

        public static void ShopPaymentCash(Player p, Inventory inv)
        {
            if (p.ShoppingCart != null)
            {
                int total = p.ShoppingCart.Sum(sc => sc.Price * sc.Quantity);
                if (long.Parse(inv.Value) >= total)
                {
                    inv.Value = (long.Parse(inv.Value) - total).ToString();

                    foreach (ShopItem item in p.ShoppingCart)
                    {
                        for (int i = 0; i < item.Quantity; i++)
                            p.GiveInventoryItmeOfType(item.Type);

                        ShopItem shopItem = p.BusinessInteractingWith.BusinessItems.Single(bi => bi.Id == item.Id);
                        shopItem.ReservedStock -= item.Quantity;
                        shopItem.Quantity -= item.Quantity;
                        ShopItemRepository.UpdateAsync(shopItem);
                    }
                    if (inv.Value == "0")
                    {
                        p.Inventory.Remove(inv);
                        InventoryRepository.RemoveInventoryItem(inv);
                    }
                    else InventoryRepository.UpdateAsync(inv);
                }
                else
                {
                    API.shared.SendErrorNotification(p.Client, "Insufficient funds.");
                    API.shared.SendInfoNotification(p.Client, "Select new payment method:");
                    InventoryManager.UpdatePlayerInventory(p);
                }
            }
        }

        public static void ShopPaymentCard(Player p, Inventory inv)
        {
            p.SelectedCardAccount = BankRepository.GetAccountByCardNumber(long.Parse(inv.Value));

            API.shared.triggerClientEvent(p.Client, "PaymentEnterPin", 3 - p.SelectedCardAccount.FailedPinAttempts);
        } 
        #endregion
    }
}
