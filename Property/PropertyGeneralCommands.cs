using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Managers;
using System.Linq;

namespace PBRP
{
    public class PropertyGeneralCommands : Script {
        [Command("enter")]
        public void EnterCommand(Client sender) {
            PropertyManager mng = new PropertyManager();
            Property prop = mng.GetClosestPropertyToLocation(sender.position, 1.0f);

            if(prop == null || !prop.IsEnterable) { return; }
            if(prop.IsLocked == true) { Message.Syntax(sender, "The property is locked."); return; }

            Player user = Player.PlayerData[sender];

            user.IsInInterior = true;
            user.PropertyIn = prop;
            sender.position = prop.ExitPosition;
            user.Dimension = prop.Dimension;

            API.setEntityDimension(sender, prop.Dimension);
        }

        [Command("exit")]
        public void ExitCommand(Client sender) {
            Player user = Player.PlayerData[sender];
            PropertyManager mng = new PropertyManager();
            Property prop = user.PropertyIn;
            
            if (prop == null || !prop.IsEnterable) { return; }
            if (prop.IsLocked == true) { Message.Syntax(sender, "The property is locked."); return; }
            if (sender.position.DistanceTo(prop.ExitPosition) > 2.5f) { return; }

            user.IsInInterior = false;
            user.PropertyIn = null;
            sender.position = prop.EnterPosition;

            user.Dimension = 0;
            API.setEntityDimension(sender, prop.Dimension);
        }

        [Command("property")]
        public async static void PropertyCommand(Client sender, string option = "") {
            if(option == "") { Message.Syntax(sender, "/property [Option] (lock, inventory)"); return; }

            Player player = Player.PlayerData[sender];
            PropertyManager mng = new PropertyManager();
            bool isLockingDoorManagerDoors = false;
            Property assumedProp = ((player.Dimension == 0) ? mng.GetClosestPropertyToLocation(sender.position, 2.0f) : null);
            if (assumedProp == null && player.Dimension != 0) { assumedProp = mng.GetPropertyByDimension(player.Dimension); }
            if (assumedProp == null) {
                if ((assumedProp = mng.GetClosestPropertyToLocationByDoors(sender.position, 2.0f)) != null) {
                    isLockingDoorManagerDoors = true;
                }
            }

            if (assumedProp == null) { Message.Syntax(sender, "You are not close enough to a property"); return; }

            if (option.ToLower() == "lock") {
                if (player.Dimension != 0 && (sender.position.DistanceTo(assumedProp.ExitPosition) > 2.0f)) {
                    Message.Syntax(sender, "You are not close enough to the door"); return; // Range check
                }

                string nameMatched = assumedProp.Name + " key";
                Inventory keyItem = player.Inventory.FirstOrDefault(i => i.Name == nameMatched);


                if (keyItem == null) { Message.Syntax(sender, "You do not have the correct key for this property"); return; }

                assumedProp.IsLocked = !assumedProp.IsLocked;
                API.shared.SendCloseMessage(sender, 15.0f, "~#C2A2DA~", API.shared.getPlayerName(sender) + " " + "places a key inside the lock of the door for " + assumedProp.Name + " and " + (assumedProp.IsLocked ? "locks" : "unlocks") + " it.");

                if (isLockingDoorManagerDoors)
                {
                    foreach (int t in assumedProp.DoormanagerDoors)
                    {
                        API.shared.exported.doormanager.setDoorState(t, assumedProp.IsLocked, 0);
                    }
                }

                await PropertyRepository.UpdateAsync(assumedProp);
                return;
            }

            if(option.ToLower() == "inventory") {
                Property p = assumedProp;
                if(p.Inventory == null) {
                    Message.Syntax(sender, "This property does not have an inventory. Please consult an administrator"); return;
                }

                API.shared.triggerClientEvent(sender, "showPlayerInventory", string.Join(",", p.Inventory.Select(e => e.Id).ToList()),
                    string.Join(",", p.Inventory.Select(e => e.Name).ToList()), string.Join(".", p.Inventory.Select(e => Inventory.GetInventoryImage[e.Type]).ToList()),
                    string.Join(",", p.Inventory.Select(e => e.Quantity).ToList())
                );
                return;
            }
        }
    }
}