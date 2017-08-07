using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PBRP
{

    public enum LicenseType
    {
        Car = 1,
        Bike = 2,
        Truck = 3,
        Heli = 4,
        Plane = 5,
        Boat = 6
    }
    [Table("licenses")]
    public class License
    {
        [Key]
        [Column("id")]
        public int id { get; set; }
        [Column("PlayerId")]
        public int PlayerId { get; set; }
        [Column("Type")]
        public LicenseType Type { get; set; }

    }
}
