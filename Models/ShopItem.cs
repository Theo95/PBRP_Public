using System.ComponentModel.DataAnnotations.Schema;

namespace PBRP
{
    public class ShopItem
    {
        public int Id { get; set; }
        [NotMapped]
        public string Name => Inventory.GetNameByType[Type];

        public int BusinessId { get; set; }
        public InventoryType Type { get; set; }
        public int Quantity { get; set; }
        [NotMapped]
        public int ReservedStock { get; set; }
        public int Price { get; set; }

    }
}
