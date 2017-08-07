using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Managers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace PBRP
{
    public class FuelManager : Script
    {
        static long secTick = 0;
        public FuelManager()
        {
            API.onResourceStart += OnResourceStart;
            API.onClientEventTrigger += OnClientEventTrigger;
            API.onEntityEnterColShape += OnEntityEnterColShape;
            API.onEntityExitColShape += OnEntityExitColShape;
            Vehicle.onVehicleSecond += OnSecond;
            API.onPlayerExitVehicle += OnPlayerExitVehicle;
        }

        private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if(eventName == "OnFuelRefill")
            {
                GrandTheftMultiplayer.Server.Elements.Vehicle closest = Globals.GetClosestVehicle(sender, 3);
                if (closest != null)
                {
                    Vehicle veh = Vehicle.VehicleData[closest];
                    if (veh.Fuel + veh.RefillDiesel + veh.RefillPetrol < 100)
                    {
                        Player p = Player.PlayerData[sender];
                        if (p.SelectedCash == null && p.SelectedCardAccount == null) return;
                        
                        int gs = GasStation.GasStations.IndexOf(GasStation.GasStations.Single(g => g.Id == p.HasEnteredGasStation));
                        if ((p.FuelTypeSelected == FuelType.Petrol && GasStation.GasStations[gs].CurrentPetrol == 0) || 
                            (p.FuelTypeSelected == FuelType.Diesel && GasStation.GasStations[gs].CurrentDiesel == 0)) return;
                        long availableMoney = 0;

                        if (p.SelectedCash != null) availableMoney = long.Parse(p.SelectedCash.Value);
                        else if (p.SelectedCardAccount != null) availableMoney = p.SelectedCardAccount.Balance;

                        if (availableMoney >= (p.FuelTypeSelected == FuelType.Petrol ? GasStation.GasStations[gs].PetrolPrice : GasStation.GasStations[gs].DieselPrice))
                            availableMoney -= p.FuelTypeSelected == FuelType.Petrol ? GasStation.GasStations[gs].PetrolPrice : GasStation.GasStations[gs].DieselPrice;
                        else
                        {
                            API.SendErrorNotification(sender, "Insufficient funds", 7);
                            p.SelectedCash = null;
                            p.SelectedCardAccount = null;
                        }

                        if (p.FuelTypeSelected == FuelType.Petrol)
                        {
                            GasStation.GasStations[gs].CurrentPetrol--;
                            veh.RefillPetrol++;
                        }
                        else
                        {
                            GasStation.GasStations[gs].CurrentDiesel--;
                            veh.RefillDiesel++;
                        }
  
                        if(p.SelectedCash != null)
                        {
                            p.SelectedCash.Value = availableMoney.ToString();
                        }
                        else if(p.SelectedCardAccount != null)
                        {
                            p.SelectedCardAccount.Balance = availableMoney;
                        }
                    }
                }
            }
            else if(eventName == "GasStationSelectFuelType")
            {
                Player p = Player.PlayerData[sender];
                p.InEvent = PlayerEvent.RefillingVehicle;

                p.AwaitingInventorySelection = InventoryType.Money;

                InventoryManager.UpdatePlayerInventory(p);
                API.SendInfoNotification(sender, "Please select your payment method.", 10);
                p.FuelTypeSelected = (int)arguments[0] == 0 ? FuelType.Petrol : FuelType.Diesel;
            }
            else if (eventName == "OnPaymentPinEntered")
            {
                Player p = Player.PlayerData[sender];
                if (p.InEvent != PlayerEvent.RefillingVehicle) return;
                if (p.SelectedCardAccount.Pin == arguments[0].ToString())
                {
                    API.triggerClientEvent(sender, "closePaymentUI");
                    API.triggerClientEvent(p.Client, "OnExitVehicleGasStation");
                }
                else
                {
                    p.SelectedCardAccount.FailedPinAttempts = 3 - (int)arguments[1];

                    if (p.SelectedCardAccount.FailedPinAttempts == 3)
                    {
                        p.SelectedCardAccount.Locked = true;
                        p.SelectedCardAccount.LockedType = BankAccountLockedType.FailedPin;
                    }
                    BankRepository.UpdateAsync(p.SelectedCardAccount);

                }
            }
        }

        private void OnPlayerExitVehicle(Client player, NetHandle vehicle)
        {
            if (!API.doesEntityExist(vehicle)) return;
            Player p = Player.PlayerData[player];

            if (p.HasEnteredGasStation == 0) return;
            Vehicle v = Vehicle.VehicleData[vehicle];
            if (!v.IsEngineOn)
            {
                if (v.FuelType() == FuelType.Petrol || v.FuelType() == FuelType.Diesel)
                    API.ShowPopupPrompt(player, "GasStationSelectFuelType", "Select Fuel Type", "Please select the type of fuel you wish to fill up with", "Petrol", "Diesel");
                else
                    API.ShowPopupMessage(player, "Incorrect Fuel",
                        String.Format("You can't {0} an {1} here, try again at an {2}", v.FuelType() == FuelType.Electric ? "charge" : "refuel",
                            v.FuelType() == FuelType.Electric ? "electric vehicle" : "aircraft", v.FuelType() == FuelType.Electric ? "electric charging station" : "airfield"));
            }
            else
            {
                API.triggerClientEvent(player, "OnExitVehEngineOnGasStation");
            }
        }

        private void OnEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) != EntityType.Player) return;
            foreach (var gs in GasStation.GasStations)
            {
                if (gs.RefillArea1 != colshape && gs.RefillArea2 != colshape) continue;
                if (API.getEntityType(entity) != EntityType.Player) continue;
                Client player = API.getPlayerFromHandle(entity);
                Player p = Player.PlayerData[player];

                API.triggerClientEvent(player, "OnLeaveGasStation");
                p.HasEnteredGasStation = 0;
                if (p.SelectedCash != null) InventoryRepository.UpdateAsync(p.SelectedCash);
                else if (p.SelectedCardAccount != null) BankRepository.UpdateAsync(p.SelectedCardAccount);
            }
        }

        private void OnEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) != EntityType.Player) return;
            foreach (GasStation gs in GasStation.GasStations)
            {
                if (gs.RefillArea1 != colshape && gs.RefillArea2 != colshape) continue;
                Client player = API.getPlayerFromHandle(entity);
                if (!player.isInVehicle) return;
                if (player.vehicleSeat != -1) return;
                API.triggerClientEvent(player, "OnEnterGasStation");
                Player.PlayerData[player].HasEnteredGasStation = gs.Id;
            }
        }

        private void OnResourceStart()
        {
            GasStation.GasStations = GasStationRepository.GetAllGasStations();

            foreach(GasStation gs in GasStation.GasStations)
            {
                gs.RefillArea1 = API.createCylinderColShape(new Vector3(gs.RefillAreaX1, gs.RefillAreaY1, gs.RefillAreaZ1), (float)gs.RefillAreaR1, 1f);
                if(gs.RefillAreaX2 != 0) gs.RefillArea2 = API.createCylinderColShape(new Vector3(gs.RefillAreaX2, gs.RefillAreaY2, gs.RefillAreaZ2), (float)gs.RefillAreaR2, 1f);
            }
        }

        private void OnSecond(Vehicle v)
        {
            if (v.IsEngineOn)
            {
                if (v.Fuel > 0)
                {
                    if (v.Entity.occupants.Count() > 0)
                    {
                        double speed = (v.Entity.occupants[0].GetSpeed() * 2.23) + 0.2f;
                        speed /= 46 + 0.3;

                        v.Fuel -= speed / 8;

                        if (v.Health - (v.EngineDamagePerSecond + (speed / 2)) >= 0)
                            API.setVehicleHealth(v.Entity, (float)(v.Health - (v.EngineDamagePerSecond + (speed / 2))));
                        else
                        {
                            v.Entity.engineStatus = false;
                            v.IsEngineOn = false;
                        }

                        if (v.Fuel < 0) v.Fuel = 0f;
                        if (v.Fuel < 8 || v.Health < 250)
                        {
                            float originalPower = API.getVehicleEnginePowerMultiplier(v.Entity);

                            Task.Run(() =>
                            {
                                v.Entity.enginePowerMultiplier = API.RandomNumber(-60, -40);
                                Thread.Sleep(1000);
                                v.Entity.enginePowerMultiplier = API.RandomNumber(-88, -122);
                                Thread.Sleep(2000);
                                v.Entity.enginePowerMultiplier = API.RandomNumber(-20, -9);
                                Thread.Sleep(2000);
                                v.Entity.enginePowerMultiplier = API.RandomNumber(-90, -65);
                                Thread.Sleep(1000);
                                v.Entity.enginePowerMultiplier = API.RandomNumber(-1, (int)originalPower);
                                Thread.Sleep(4000);
                            });
                        }
                    }
                    else
                    {
                        v.Fuel -= 0.30434782;
                        if (v.Fuel < 0) v.Fuel = 0f;
                        if (Math.Abs(v.EngineDamagePerSecond) > 0) API.setVehicleHealth(v.Entity, (float)(v.Health - v.EngineDamagePerSecond));
                    }
                }
                else
                {
                    v.IsEngineOn = false;
                    v.Entity.engineStatus = false;
                }
            }
        }

        public static void BeginRefuelWithCard(Player p, Inventory inv)
        {
            p.SelectedCardAccount = BankRepository.GetAccountByCardNumber(long.Parse(inv.Value));

            InventoryManager.HidePlayerInventory(p, true);
            API.shared.triggerClientEvent(p.Client, "PaymentEnterPin", 3 - p.SelectedCardAccount.FailedPinAttempts);

        }

        public static void BeginRefuelWithCash(Player p, Inventory inv)
        { 
            p.SelectedCash = inv;
            API.shared.triggerClientEvent(p.Client, "OnExitVehicleGasStation");

            InventoryManager.HidePlayerInventory(p, true);
        }
    }
}
