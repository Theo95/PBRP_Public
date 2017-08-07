using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.Managers;

namespace PBRP
{
    public class RadioManager : Script {
        public RadioManager() {}

        public static Inventory GetPlayerRadioItem(Player player)
        {
            var list = player.Inventory.Where(i => i.Name.Contains("Radio"));
            return list.Any() ? list.First() : null;
        }

        public static List<Inventory> GetAllActiveRadiosOnFrequency(string frequency)
        {
            // Go through all players and add their radios to the list

            return Player.PlayerData.Select((t, i) => Player.PlayerData.ElementAt(i).Value).
                Select(GetPlayerRadioItem)
                .Where(radio => GetRadioFrequency(radio) == frequency && IsRadioOn(radio)).ToList();
        }

        public static List<Vehicle> GetAllActiveVehiclesWithCompanyId(int cID)
        {
            // Go through all players and add their radios to the list

            return Vehicle.VehicleData.Select((t, i) => Vehicle.VehicleData.ElementAt(i).Value).Where(veh => veh.FactionId == cID).ToList();
        }

        public static bool DoesPlayerHaveRadio(Player player) {
            return (player.Inventory.Any(i => i.Name.Contains("Radio")));
        }

        public static string GetRadioFrequency(Inventory radioItem) {
            string value = radioItem.Value;
            int indexToSeperate = value.IndexOf("|", StringComparison.Ordinal);
            return indexToSeperate == 0 ? "ERROR" : value.Substring(1, (indexToSeperate - 1));
        }

        public static bool IsRadioOn(Inventory radioItem) {
            Console.WriteLine(radioItem.Value);
            return radioItem.Value.Contains("|ON");
        }

        public static bool  ToggleRadioPower(Player player, Inventory radioItem) {
            bool newPower = true;

            if (radioItem.Value.Contains("|ON")) {
                radioItem.Value.Replace("|ON", "|OFF");
            } else if (radioItem.Value.Contains("|OFF")) {
                radioItem.Value.Replace("|OFF", "|ON");
            } else {
                Console.WriteLine("Someone fucked up a radio that doesn't have |ON or |OFF");
            }

            return newPower;
        }

        public static void BroadcastRadioMessage(Player player, string frequency, string message) {
            bool checkVehicles = (frequency == "9111" || frequency == "9112");
            if (checkVehicles) {
                switch (frequency) {
                    case "9111":
                        break;
                    case "9112":
                        break;
                }
            }

            List<Inventory> radios = GetAllActiveRadiosOnFrequency(frequency);
            List<Vehicle> vehicles = new List<Vehicle>();
            //if (checkVehicles) {
            //    vehicles = GetAllActiveVehiclesWithCompanyID(companyIDToCheck);
            //}

            // --- 
            List<int> sentTo = new List<int>();
            foreach (Inventory radio in radios)
            {
                if(radio.OwnerId == player.Id) { continue; }

                int owner = radio.OwnerId; sentTo.Add(owner);
                Message.Radio(Player.PlayerData.ElementAt(owner).Key, "[Radio (#" + frequency + ")] " + player.Username +" says: " + message);
            }

            API.shared.SendCloseMessage(player.Client, 15f, player.Username.Roleplay() + " (Radio) says: " + message);

            //for(int i = 0; i < vehicles.Count; ++i) {
            //    Vehicle veh = vehicles[i];
            //    Client[] occupants = API.shared.getVehicleOccupants(veh.Entity);

            //    for(int j = 0; j < occupants.Length; ++j) {
            //        Player ply = Player.PlayerData[occupants[j]];
            //        if (sentTo.Contains(ply.Id)) { continue; }
            //        Message.Radio(ply.Client, "[Radio (#VehicleRadio)] " + player.Username + " says: " + message);
            //    }
            //}
            return;
        }

        [Command("radio", Alias = "r", GreedyArg = true)]
        public void RadioCommand(Client sender, string message){
            Player player = Player.PlayerData[sender];
            Inventory radio = GetPlayerRadioItem(player);

            if(radio == null) { Message.Syntax(sender, "You don't have a radio."); return; }
            if(!IsRadioOn(radio)){ Message.Syntax(sender, "Your radio is turned off."); return; }
            BroadcastRadioMessage(player, GetRadioFrequency(radio), message);
        }
    }
}
