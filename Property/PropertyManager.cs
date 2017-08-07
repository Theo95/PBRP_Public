using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PBRP
{
    public class PropertyManager : Script {

        public PropertyManager() {
            API.onResourceStart += OnResourceStart;
            API.onResourceStop += OnResourceStop;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "OnPropertyEnter":
                    new PropertyGeneralCommands().EnterCommand(sender);
                    break;
                case "OnPropertyExit":
                    new PropertyGeneralCommands().ExitCommand(sender);
                    break;
            }
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client client = API.getPlayerFromHandle(entity);
                try
                {
                    var player = Player.PlayerData[client];

                    if (player.PropertyEnterExit == null) return;
                    if (player.PropertyEnterExit.EntranceColShape != colshape &&
                        player.PropertyEnterExit.ExitColShape != colshape) return;
                    API.triggerClientEvent(client, "displayPropertyEnter", "");
                    player.PropertyEnterExit = null;
                }
                catch {

                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if(API.getEntityType(entity) == EntityType.Player)
            {
                Client client = API.getPlayerFromHandle(entity);
                foreach (Property p in Properties)
                {
                    if ((p.EntranceColShape != colshape && p.ExitColShape != colshape) || p.Type == PropertyType.Service) continue;
                    Player player = Player.PlayerData[client];
                    if (p.EntranceColShape == colshape)
                    {
                        if (!p.IsLocked && p.IsEnterable)
                            API.triggerClientEvent(client, "displayPropertyEnter",
                                $"Press ~r~{(char) player.MasterAccount.KeyInteract} ~w~to enter ~y~{p.Name}");
                        else
                            API.triggerClientEvent(client, "displayPropertyEnter",
                                $"~r~{p.Name}~w~'s door is ~y~locked.");
                    }
                    else
                        API.triggerClientEvent(client, "displayPropertyEnter",
                            $"Press ~r~{(char) player.MasterAccount.KeyInteract}~w~ to exit");

                    player.PropertyEnterExit = p;
                    return;
                }
            }
        }

        private void OnResourceStop() { }

        public static List<Property> Properties = new List<Property>();
        public static List<Business> Businesses = new List<Business>();
        public static List<House> Houses = new List<House>();
        private List<NetHandle> propertyPickups = new List<NetHandle>();
        private List<int> propertyDoors = new List<int>();

        private async void OnResourceStart()  {
            Properties = await PropertyRepository.GetAllProperties();

            Businesses = await PropertyRepository.GetAllBusinesses();

            foreach (Property prop in Properties)  {
                InitProperty(prop);
            }

            foreach(Business bus in Businesses) {
                BusinessManager.ConfigureBusiness(bus);
            }

            RegisterDoormanagerDoors();

            API.createMarker(20, new Vector3(-248.1604f, 6212.266f, 31.5), new Vector3(0, 0, 0), new Vector3(0, 0, 0),
                new Vector3(0.5, 0.5, 0.5), 255, 25, 159, 235, 0);

            API.createTextLabel("Helmuts Autos", new Vector3(-248.1604f, 6212.266f, 32), 8, 0.85f);

            LoadPropertyMaps();
        }

        // --- END EVENTS

        public void InitProperty(Property prop) {
            // --- Create the "Pickup" for the property
            Globals.Colour col = new Globals.Colour(255, 255, 255);

            switch (prop.Type) {
                case PropertyType.General: { col = Property.generalColour; break; }
                case PropertyType.Residential: { col = Property.residentialColour; break; }
                case PropertyType.Commericial: { col = Property.commercialColour; break; }
                case PropertyType.Industrial: { col = Property.industrialColour; break; }
                case PropertyType.Service: { col = Property.commercialColour; break; }
            }

            prop.pickupHandle = API.createMarker(
                prop.Type == PropertyType.Service ? 2 : 20,
                prop.EnterPosition,
                new Vector3(0, 0, 0),
                prop.Type == PropertyType.Service ? new Vector3(180, 0, 0) : new Vector3(0, 0, 0),
                new Vector3(0.5f, 0.5f, 0.5f),
                col.a, col.r, col.g, col.b
            );

            // --- Create the label for the property
            bool isEnterable = prop.IsEnterable;
            col = new Globals.Colour(
                isEnterable ? 100 : 223,
                isEnterable ? 223 : 58,
                0
            );

            if (prop.Type == PropertyType.Service) col = new Globals.Colour(255, 255, 255);

            prop.EnterPosition += new Vector3(0, 0, 0.5f);

            prop.labelHandle = API.createTextLabel(prop.Name, prop.EnterPosition, 7.0f, 0.85f);
            API.setTextLabelColor(prop.labelHandle, col.r, col.g, col.b, col.a);

            prop.EntranceColShape = API.createCylinderColShape(prop.EnterPosition, 1f, 1f);
            prop.ExitColShape = API.createCylinderColShape(prop.ExitPosition, 1f, 1f);
        }

        // ---- GETTER FUNCTIONS

        public List<Property> GetPropertiesByTypeId(int typeId) {
            return Properties.Where(p => (int)p.Type == typeId).ToList();
        }

        public Property GetClosestPropertyToLocation(Vector3 location, float minRange) {
            Property closestProperty = null;
            float maxDist = -1;
            foreach (Property p in Properties)  {
                float dist = location.DistanceTo(p.EnterPosition);
                if ((maxDist != -1 && !(dist < maxDist)) || !(dist < minRange)) continue;
                maxDist = dist;
                closestProperty = p;
            }
            return closestProperty;
        }

        public int GetClosestPropertyIDToLocation(Vector3 location) {
            int id = -1;

            Property closestProperty = null;
            float maxDist = -1;
            foreach (Property p in Properties) {
                float dist = location.DistanceTo(p.EnterPosition);
                if (maxDist != -1 && !(dist < maxDist)) continue;
                maxDist = dist;
                closestProperty = p;
            }

            if (closestProperty != null) {
                id = closestProperty.Id;
            }
            return id;
        }

        public Property GetClosestPropertyToLocationByDoors(Vector3 location, float maxRange) {
            Property prop = null;
            float closestDist = maxRange;

            // @TODO needs refactoring, potential CPU hog.
            List<Property> propertiesToCheck = Properties.Where(p => p.DoormanagerDoors.Count > 0).ToList();
            foreach (Property t_prop in propertiesToCheck)
            {
                for(int d = 0; d < t_prop.DoormanagerDoors.Count; ++d) { // Iterate through each door in the property;
                    float dist = t_prop.DoormanagerDoorLocations[d].DistanceTo(location);
                    if (dist <= closestDist) {
                        prop = t_prop;
                    }
                }
            }
            
            return prop;
        }

        public Property GetPropertyByDimension(int Dimension) {
            return Properties.FirstOrDefault(p => p.Dimension == Dimension);
        }

        public Property GetPropertyByName(string Name) {
            return Properties.FirstOrDefault(p => p.Name == Name);
        }

        public List<Property> GetPropertiesByPartialName(string Name) {
            return Properties.Where(p => p.Name.Contains(Name)).ToList();
        }

        // --- MISC FUNCTIONS

        public bool IsValidTypeID(PropertyType typeID)
        {
            return (typeID >= PropertyType.General && typeID <= PropertyType.Industrial);
        }

        public async void DeleteAllPropertyKeys(int id) {
            Property property = Properties[(id-1)];
            List<Inventory> items = await InventoryRepository.GetInventoryItemByName(property.Name + " key");

            // --- Go through all actively loaded properties and remove the key
            foreach (Inventory item in items) {
                foreach (KeyValuePair<Client, Player> player in Player.PlayerData) {
                    Player ply = Player.PlayerData[player.Key];
                    Inventory itm = ply.Inventory.FirstOrDefault(i => i.Id == item.Id);
                    if (itm == null) { continue; }

                    ply.Inventory.Remove(itm);
                }

                foreach (Property prop in Properties) {
                    Inventory itm = prop.Inventory?.FirstOrDefault(i => i.Id == item.Id);
                    if (itm == null) { continue; }

                    prop.Inventory.Remove(itm);
                }

                foreach (KeyValuePair<NetHandle, Vehicle> vehicle in Vehicle.VehicleData) {
                    Vehicle veh = Vehicle.VehicleData[vehicle.Key];
                    Inventory itm = veh.TrunkItems?.FirstOrDefault(i => i.Id == item.Id);
                    if (itm == null) { continue; }

                    veh.TrunkItems.Remove(itm);
                }

                InventoryRepository.RemoveInventoryItem(item);
            }
        }

        public void BindDoormanagerDoorsToProperty(List<int> doors, List<Vector3> locations, string propertyName) {
            Property property = null;
            property = GetPropertyByName(propertyName);

            if (property == null) { return; }
            if (doors.Count == 0 || locations.Count != doors.Count) { return; }
            
            for (int i = 0; i < doors.Count; ++i) {
                if (property.DoormanagerDoors.Contains(doors[i])) { continue; }

                property.DoormanagerDoors.Add(doors[i]);
                property.DoormanagerDoorLocations.Add(locations[i]);

                API.exported.doormanager.setDoorState(doors[i], property.IsLocked, 0);
            }
        }


        // --- Door manager Doors
        private void RegisterDoormanagerDoors() {

            // @TODO: Needs DoorManager re-write to incorporate our needs. Otherwise we end up with this shitty looking code
            List<int> td = new List<int>(); // Create a new array which holds the ID of each door
            List<Vector3> tdl = new List<Vector3>(); // Create a new array which holds the ID of each door

            // -- PB AMMUNATION
            td.Add(API.exported.doormanager.registerDoor(97297972, new Vector3(-326.1122, 6075.270, 31.6047))); // PB Ammunation Left Door
            td.Add(API.exported.doormanager.registerDoor(-8873588, new Vector3(-324.2730, 6077.109, 31.6047))); // PB Ammunation Right Door

            tdl.Add(new Vector3(-326.1122, 6075.270, 31.6047));
            tdl.Add(new Vector3(-324.2730, 6077.109, 31.6047));

            BindDoormanagerDoorsToProperty(td, tdl, "Paleto Bay Ammunation");

            // --- PB TATTOO PARLOUR
            tdl.Clear(); td.Clear();

            td.Add(API.exported.doormanager.registerDoor(-121951353, new Vector3(-289.1752, 6199.112, 31.63704))); // PB Tattoo Parlour Door
            tdl.Add(new Vector3(-289.1752, 6199.112, 31.63704));

            BindDoormanagerDoorsToProperty(td, tdl, "Paleto Bay Tattoo Parlour");

            // --- HERR KUTZ BARBERSHOP
            tdl.Clear(); td.Clear();

            td.Add(API.exported.doormanager.registerDoor(-184444717, new Vector3(-280.7851, 6232.782, 31.84548))); // HERR KUTZ BARBERSHOP
            tdl.Add(new Vector3(-280.7851, 6232.782, 31.84548));

            BindDoormanagerDoorsToProperty(td, tdl, "Herr Kutz Barbershop");
        }

        // --- Property Maps
        private void LoadPropertyMaps() {
            API.createObject(1737076325, new Vector3(434.732391, -983.835938, 29.7125797), new Vector3(-0, 0, -90.4699326));
            API.createObject(1737076325, new Vector3(434.740051, -980.376831, 29.7132339), new Vector3(-0, 0, -90.1611481));
        }
    }
}