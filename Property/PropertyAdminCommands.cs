using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Server;

namespace PBRP
{
    public class PropertyAdminCommands : Script
    {
        [Command("gotoproperty", Alias = "gotoprop", GreedyArg = true)]
        public void GotoPropertyCommand(Client sender, string option = "") {
            if (option == "") { Message.Syntax(sender, "/gotoproperty [PartOfName / ID]"); return; }
            int val; Int32.TryParse(option, out val);

            if (val != 0) {
                Property prop = PropertyManager.Properties[val - 1];
                API.setEntityPosition(sender.handle, prop.EnterPosition);

                Message.Info(sender, "You have teleported yourself to " + prop.Name);
            } else {
                PropertyManager mng = new PropertyManager();
                Property property = mng.GetPropertiesByPartialName(option).First();

                API.setEntityPosition(sender.handle, property.EnterPosition);
                Message.Info(sender, "You have teleported yourself to " + property.Name);
            }
        }

        [Command("propertylookup", GreedyArg = true)]
        public void LookupPropertyCommand(Client sender, string option = "") {
            if(option == "") { Message.Syntax(sender, "/propertylookup [PartOfName / ID]"); return; }
            int val; Int32.TryParse(option, out val);

            if(val != 0) {
                Message.Info(sender, "Property lookup (ID: " + val + ")");

                Property prop = PropertyManager.Properties[val-1];
                Message.Info(sender, prop.Name + " | Owner: " + (prop.OwnerId != -1 ? PlayerRepository.GetPlayerDataById(prop.OwnerId).Username : "None") + " (SQL ID: " + prop.Id + ")");
            } else {
                PropertyManager mng = new PropertyManager();
                List<Property> properties = mng.GetPropertiesByPartialName(option);

                Message.Info(sender, "Property lookup (Name: " + option + ")");
                foreach (Property prop in properties) {
                    Message.Info(sender, prop.Name + " | Owner: " + (prop.OwnerId != -1 ? PlayerRepository.GetPlayerDataById(prop.OwnerId).Username : "None") + " (SQL ID: " + prop.Id + ")");
                }
            }
        }

        [Command("aproperty", GreedyArg = true)]
        public void PropertyCommand(Client sender, string option = "", int num = -1, string name = "") {
            Player user = Player.PlayerData[sender];
            if (!(user.MasterAccount.AdminLevel >= 4)) { Message.NotAuthorised(sender); return; }

            if (option == "") { Message.Syntax(sender, "/aproperty [create/delete] (To edit a property the command is /editproperty)"); return; }
            if (option == "create") { CreateProperty(sender, num, name); return; }
            if (option == "delete") { DeleteProperty(sender, num); return; }

            return;
        }

        [Command("editproperty", GreedyArg = true)]
        public void EditPropertyCommand(Client sender, int num = -1, string name = "", string value = "", string val2 = "")
        {
            Player user = Player.PlayerData[sender];
            if (!(user.MasterAccount.AdminLevel >= 4)) { Message.NotAuthorised(sender); return; }
            EditProperty(sender, num, name, value, val2);
            return;
        }

        [Command("setinterior", GreedyArg = true)]
        public async void SetInteriorCommand(Client sender, int num = -1, string name = "") {
            Player user = Player.PlayerData[sender];
            if (!(user.MasterAccount.AdminLevel >= 4)) { Message.NotAuthorised(sender); return; }

            if(name == "") { Message.Syntax(sender, "/setinterior [ID (-1 for closest)] [Name]"); return; }

            PropertyManager mng = new PropertyManager();
            Property prop;
            prop = num == -1 ? mng.GetClosestPropertyToLocation(sender.position, 2.0f) : PropertyManager.Properties[num];

            int Dimension = prop.Id;
            Vector3 interiorPos = new Vector3();

            interiorPos = GetInteriorCoordinatesFromName(name);
            if(interiorPos.Equals(new Vector3())) { Message.Syntax(sender, "Cannot find interior: " + name); return; }

            prop.IsEnterable = true;
            prop.ExitPosition = interiorPos;
            prop.Dimension = Dimension;

            API.setTextLabelColor(prop.labelHandle, 100, 223, 0, 255);
            API.SendWarningNotification(sender, ("You have edited " + prop.Name + "'s ExitPos to: " + interiorPos.ToString() + " (Dimension: " + prop.Id + ")"));

            await PropertyRepository.UpdateAsync(prop);
        }

        [Command("gotointerior", GreedyArg = true)]
        public void GotoInteriorCommand(Client sender, string name = "") {
            if(name == "") { Message.Syntax(sender, "/gotointerior [Interior Name]"); return; }

            Vector3 pos = GetInteriorCoordinatesFromName(name);

            if(pos.Equals(new Vector3())) { Message.Syntax(sender, "Incorrect interior name"); return; }
            API.setEntityPosition(sender, pos);
        }

        [Command("getpropertyid")]
        public void GetPropertyCommand(Client sender) {
            Player user = Player.PlayerData[sender];
            if (!(user.MasterAccount.AdminLevel >= 4)) { Message.NotAuthorised(sender); return; }

            PropertyManager mng = new PropertyManager();
            Property prop = mng.GetClosestPropertyToLocation(sender.position, 5.0f);

            if(prop == null) {
                Message.Syntax(sender, "You are not close enough to any properties");
            }

            API.SendWarningNotification(sender, "Property ID for " + prop.Name + " is " + prop.Id);
            return;
        }

        // ------- Non CMDs

        public void CreateProperty(Client sender, int type = -1, string name = "") {
            Player user = Player.PlayerData[sender];
            if (!(user.MasterAccount.AdminLevel >= 4)) { Message.NotAuthorised(sender); return; }

            // --- Handle Command Parameter checking
            if ((type < 0 || type > 2) || name == "") {
                Message.Syntax(sender, "/aproperty create [Type] [Name] (Types: 0 - General | 1 - Residential | 2 - Commercial | 3 - Industrial)");
                return;
            }

            if (PropertyRepository.GetPropertyByPropertyName(name) != null)  {
                API.SendErrorNotification(sender, "That property name already exists");
                return;
            }

            // --- Create the property
            //Property newProperty = new Property();
            //newProperty.Type = (PropertyType)type; // Cast int to PropertyType
            //newProperty.Name = name;
            //newProperty.EnterPosition = sender.position;

            //PropertyManager mng = new PropertyManager();
            //mng.InitProperty(newProperty);

            //// --- Add to Entities
            //PropertyManager.Properties.Add(newProperty);
            //await PropertyRepository.AddNewProperty(newProperty);

        }

        public async void EditProperty(Client sender, int id = -1, string field = "", string value = "", string val2 = "") {
            Player user = Player.PlayerData[sender];
            if (!(user.MasterAccount.AdminLevel >= 4)) { Message.NotAuthorised(sender); return; }
            if (field == "" && value == "") {
                PropertyCommandUsage(sender);
                return;
            }

            Property propertyToEdit = null;
            PropertyManager mng = new PropertyManager();
            if (id == -1) {
                float minRange = 2.5f;
                propertyToEdit = mng.GetClosestPropertyToLocation(sender.position, minRange);
                if (propertyToEdit == null) { API.SendErrorNotification(sender, "Error: Could not find property"); return; }
                id = propertyToEdit.Id;
            } else {
                if(id > PropertyManager.Properties.Count) { API.SendErrorNotification(sender, "Error: Could not find property"); return; }
                propertyToEdit = PropertyManager.Properties[id-1];
            }

            // --- Property is valid, found and ready to edit
        
            if(field == "Name") {
                value += (" " + val2);

                propertyToEdit.Name = value;
                API.setTextLabelText(propertyToEdit.labelHandle, value);
            }

            if(field == "Type") {
                PropertyType typeID = ((PropertyType)Int32.Parse(value));
                if (!mng.IsValidTypeID(typeID)) { PropertyCommandUsage(sender); return; }

                propertyToEdit.Type = typeID;

                Globals.Colour col = new Globals.Colour(255, 255, 255);
                switch (typeID) {
                    case PropertyType.General:    { col = Property.generalColour; break; }
                    case PropertyType.Residential:{ col = Property.residentialColour; break; }
                    case PropertyType.Commericial: { col = Property.commercialColour; break; }
                    case PropertyType.Industrial: { col = Property.industrialColour; break; }
                }

                API.setMarkerColor(propertyToEdit.pickupHandle, col.a, col.r, col.g, col.b);
                API.SendWarningNotification(sender, ("You have edited " + propertyToEdit.Name + "'s type ID to: " + propertyToEdit.Type));
            }

            if(field == "Owner") {
                if (val2 == "") { PropertyCommandUsage(sender); return; }

                int val = Int32.Parse(val2);
                int TYPE_PLAYER = 0; int TYPE_COMPANY = 1;
                if (val != TYPE_PLAYER && val != TYPE_COMPANY) { PropertyCommandUsage(sender); return; }

                // -- Delete all property keys
                mng.DeleteAllPropertyKeys(id);

                // -- Create a new key and give it to X
                if(val == TYPE_PLAYER) {

                    Player recipient = Player.GetPlayerData(value);
                    Inventory key = null;

                    propertyToEdit.OwnerId = recipient.Id;

                    // --- If the player is online, we should handle it
                    if (API.isPlayerConnected(recipient.Client)) { // -- Check may be redundant. Added as extra security
                        key = new Inventory() {
                            Name = propertyToEdit.Name + " key",
                            Type = InventoryType.PropertyKey,
                            Value = propertyToEdit.Id + "," + Globals.GetUniqueString(),
                            Quantity = 1,
                            OwnerId = recipient.Id,
                        };
                        recipient.Inventory.Add(key);
                    } else { API.SendErrorNotification(sender, "Player could not be found, use their full name with underscores"); }
                    InventoryRepository.AddNewInventoryItem(key);
                    key.Id = InventoryRepository.GetInventoryItemOfTypeByValue(InventoryType.PropertyKey, key.Value).Id;
                    API.SendWarningNotification(sender, "You have set the Owner of " + propertyToEdit.Name + " to " + recipient.Username);

                } else if (val == TYPE_COMPANY) {
                    // If Faction give to all high ranks? or just highest? or give to admin to give to faction member
                    API.SendErrorNotification(sender, "Not implemented yet");
                }
                return;
            }

            if (field == "EnterPos") {
                Vector3 pos = sender.position;
                Globals.Colour col = new Globals.Colour(255, 255, 255);

                switch (propertyToEdit.Type) {
                    case PropertyType.General:     { col = Property.generalColour;     break; }
                    case PropertyType.Residential: { col = Property.residentialColour; break; }
                    case PropertyType.Commericial:  { col = Property.commercialColour;  break; }
                    case PropertyType.Industrial:  { col = Property.industrialColour;  break; }
                }

                API.deleteEntity(propertyToEdit.pickupHandle);
                propertyToEdit.pickupHandle = API.createMarker(20, pos, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.5, 0.5, 0.5),
                    col.a, col.r, col.g, col.b);

                bool isEnterable = propertyToEdit.IsEnterable;
                col = new Globals.Colour(
                    isEnterable ? 100 : 223,
                    isEnterable ? 223 : 58,
                    isEnterable ? 0 : 0
                );

                API.deleteEntity(propertyToEdit.labelHandle);
                Vector3 lablePos = pos.Copy(); lablePos.Z += 0.5f;
                propertyToEdit.labelHandle = API.createTextLabel(propertyToEdit.Name, lablePos, 25.0f, 0.75f);
                API.setTextLabelColor(propertyToEdit.labelHandle, col.r, col.g, col.b, col.a);

                propertyToEdit.EnterPosition = pos;
                API.SendWarningNotification(sender, ("You have edited " + propertyToEdit.Name + "'s EnterPos to: " + pos.ToString()));
            }

            if (field == "ExitPos") {
                if (value == "-1") {
                    propertyToEdit.IsEnterable = false;
                    propertyToEdit.ExitPosition = new Vector3(0, 0, 0);
                    propertyToEdit.Dimension = 0;

                    API.setTextLabelColor(propertyToEdit.labelHandle, 223, 58, 0, 255);
                    API.SendWarningNotification(sender, ("You have removed " + propertyToEdit.Name + "'s ExitPos"));

                } else {
                    propertyToEdit.IsEnterable = true;
                    propertyToEdit.ExitPosition = sender.position;
                    propertyToEdit.Dimension = propertyToEdit.Id;

                    API.setTextLabelColor(propertyToEdit.labelHandle, 100, 223, 0, 255);
                    API.SendWarningNotification(sender, ("You have edited " + propertyToEdit.Name + "'s ExitPos to: " + sender.position.ToString() + " (Dimension: " + propertyToEdit.Id + ")"));
                }

                
            }

            if (field == "Dimension") {
                int dimension = Int32.Parse(value);

                if ((int)dimension < 0) { PropertyCommandUsage(sender); return; }

                propertyToEdit.Dimension = dimension;
                API.SendWarningNotification(sender, ("You have edited " + propertyToEdit.Name + "'s type Dimension: " + dimension));
            }

            await PropertyRepository.UpdateAsync(propertyToEdit);
            return;
        }

        public void DeleteProperty(Client sender, int id = -1) {
            Player user = Player.PlayerData[sender];
            if (!(user.MasterAccount.AdminLevel >= 4)) { Message.NotAuthorised(sender); return; }
            if (id == -1) { Message.Syntax(sender, "/aproperty delete [ID] (ID must be valid, /getpropertyid or /propertylookup)"); return; }
            if (id > PropertyManager.Properties.Count) { API.SendErrorNotification(sender, "Could not find property at ID: " + id); return; }

            Property propertyToDelete = PropertyManager.Properties[(id-1)];
            if(propertyToDelete == null) { API.SendErrorNotification(sender, "Could not find property at ID: " + id); return; }

            PropertyManager mng = new PropertyManager();
            mng.DeleteAllPropertyKeys(id);

            PropertyRepository.DeleteProperty(propertyToDelete);
            return;
        }

        private void PropertyCommandUsage(Client sender) {
            Message.Syntax(sender, "/editproperty [ID] [Field] [Value] [Val2] | (ID should be -1 to edit the business you're on)");
            Message.Syntax(sender, "Fields: Name, Type*, Owner**, EnterPos***, ExitPos***, Dimension*");
            Message.Syntax(sender, "* - Requires a number Value (Types: 0 - General | 1 - Residential | 2 - Commercial)");
            Message.Syntax(sender, "** - Val1 = Player name / Company ID | Requires Val2 (Types: 0 - Player | 1 - Company)");
            Message.Syntax(sender, "*** - No value required, gets your current position or -1 to remove");
            return;
        }

        private Vector3 GetInteriorCoordinatesFromName(string name) {
            Vector3 coords = new Vector3();

            switch (name.ToLower()) {
                // --- Houses
                case "motel apartment": { coords = new Vector3(151.2715, -1007.802, -99); break; }
                case "beach house": { coords = new Vector3(-1150.938, -1520.66, 10.63273); break; }
                case "low end apartment": { coords = new Vector3(266.1723, -1006.963, -100.8682); break; }
                case "mid range apartment": { coords = new Vector3(346.4366, -1012.726, -99.196188); break; }

                // --- Garages
                case "small garage": { coords = new Vector3(173.360535, -1002.68848, -98.9999008); break; }
                case "medium garage": { coords = new Vector3(198.04454, -1002.16327, -99.0000076); break; }
                case "large garage": { coords = new Vector3(229.356995, -992.910583, -98.9999008); break; }

                // --- Businesses / Buildings

                // --- Misc
                case "lspd": { coords = new Vector3(435.559235, -981.960022, 30.6980133); break; }
                default: break;
            }

            return coords;
        }
    }
}