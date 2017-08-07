using System;
using System.Linq;

namespace PBRP
{
    public static class InventoryExtensionMethods
    {

        public static bool AddToPlayer(this Inventory inv, Player player = null, bool addToInv = false)
        {
            Player p = player ?? Player.PlayerData.Values.FirstOrDefault(pl => pl.Id == inv.OwnerId);
            int hCount = 1;
            int vCount = 1;

            while(hCount <= 4 && vCount <= 4)
            {
                int oH = hCount, oV = vCount;
                for(int h = hCount; h <= hCount + inv.SlotSpan[0] - 1 || hCount > 4; h++)
                {
                    for(int v = vCount; v <= vCount + inv.SlotSpan[1] - 1 || vCount > 4; v++)
                    {
                        if(p.Inventory.Where(i => (h >= Convert.ToInt32(i.SlotPosStr[0].ToString()) && 
                            h <= (Convert.ToInt32(i.SlotPosStr[0].ToString()) + i.SlotSpan[0]) - 1) &&
                            (v >= Convert.ToInt32(i.SlotPosStr[1].ToString()) && 
                            v <= (Convert.ToInt32(i.SlotPosStr[1].ToString()) + i.SlotSpan[1]) - 1)).FirstOrDefault() == null)
                        {
                            continue;
                        }
                        else
                        {
                            if (++vCount > 4)
                            {
                                vCount = 1;
                                if (++hCount > 4) return false;
                            }
                        }
                    }
                }
                if (oH == hCount && oV == vCount)
                {
                    if(addToInv)
                        inv.SlotPosition = int.Parse(hCount.ToString() + vCount.ToString());
                    return true;
                }
            }
            return false;
        }

        public static bool IsCash(this Inventory inv)
        {
            switch (inv.Type)
            {
                case InventoryType.CashSmall:
                case InventoryType.CashMedium:
                case InventoryType.CashLarge:
                    return true;
                default: return false;
            }
        }

        public static bool IsBag(this Inventory item)
        {
            switch(item.Type)
            {
                case InventoryType.SmallBackpack:
                case InventoryType.MediumBackpack:
                case InventoryType.LargeBackpack:
                    return true;
                default: return false;
            }
        }

        public static async void GiveInventoryItmeOfType(this Player p, InventoryType type)
        {
            Inventory item = new Inventory()
            {
                Name = Inventory.GetNameByType[type],
                OwnerId = p.Id,
                OwnerType = InventoryOwnerType.Player,
                Type = type,
                Quantity = 1
            };
            switch(type)
            {
                case InventoryType.SmartPhone1:
                    Phone phone = await Phone.CreateNow();
                    item.Value = phone.IMEI.ToString();
                    break;
            }

            item.AddToPlayer(p, true);
            InventoryRepository.AddNewInventoryItem(item);
        }
    }
}
