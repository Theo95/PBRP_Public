using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBRP
{
    public enum BusinessType
    {
        Undefined = -1,
        Shop = 0,
        Mechanic = 1,
        Bar = 2,
        Gas = 3,
        Clothes = 4,
        Tattoo = 5,
        Hair = 6,
        Motel = 7
    }

    public enum VehicleRepairType
    {
        Engine = 1,
        Body = 2,
        Tyres = 3
    }

    [Table("properties")]
    public class Business : Property {

        public BusinessType SubType { get; set; }

        public List<ShopItem> BusinessItems { get; set; }

        [NotMapped]
        public ColShape InteractionPoint { get; set; }

        public string GenerateSafeCode() {
            string code = "";

            int NUM_OF_COMBINATIONS = 5;
            Random random = new Random();

            for(int i = 0; i < NUM_OF_COMBINATIONS; ++i) {
                int comb = random.Next(1, 40);
                code += comb;

                if(i+i < NUM_OF_COMBINATIONS) {
                    code += "-";
                }
            }

            return code;
        }

        public static Dictionary<int, Vector3> InteriorInteractPos = new Dictionary<int, Vector3>()
        {
            { 1, new Vector3(-48.64783, -1757.045, 29.42101) }, //Limited Gas 24/7, Los Santos
            { 2, new Vector3(25.78219, -1346.585, 29.49703) }, //South Los Santos 24/7
        };

        public Property GetBusinessProperty()
        {
            return PropertyManager.Properties.Find(p => p.Id == Id);
        }
    }
}
