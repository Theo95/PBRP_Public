using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using System.Collections.Generic;

namespace PBRP
{
    public class PDManager : Script  {

        public PDManager()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if(eventName == "OnTicketChargeReason")
            {
                if ((int)arguments[0] == 0) return;
                Player p = Player.PlayerData[sender];
                Player target = p.PlayerInteractingWith;

                if(target == null) { return; }

                API.ShowInputPrompt(sender, "OnTicketChargeFine", "Fine Amount", "Please enter the amount you wish to fine " + target.Username.Roleplay() + ":", "Give copy", "Cancel");
            }
            else if(eventName == "OnPlayerFriskResponse")
            {
                Player p = Player.PlayerData[sender];
                if ((int)arguments[0] == 1)
                {
                    InventoryManager.ShowFriskInventory(p.PlayerInteractingWith, p);
                }
                else
                {
                    API_onClientEventTrigger(sender, "CancelPlayerFrisk", "");
                }
            }
            else if(eventName == "CancelPlayerFrisk")
            {
                Player p = Player.PlayerData[sender];
                if(p.PlayerInteractingWith != null)
                {
                    API.SendInfoNotification(p.PlayerInteractingWith.Client, string.Format("{0} {1} has stopped frisking you.", p.Faction.Ranks[p.FactionRank].Title, p.Username.Roleplay()), 10);
                    p.PlayerInteractingWith.PlayerInteractingWith = null;
                    p.PlayerInteractingWith = null;
                }
            }
        }

        public class CellDoor {
            public int doorID;
            public bool isLocked;
            public Vector3 position;

            public CellDoor(int hash, Vector3 pos, bool locked) {
                int id = API.shared.exported.doormanager.registerDoor(hash, pos);

                doorID = id;
                isLocked = locked;
                position = pos;

                API.shared.exported.doormanager.setDoorState(id, locked, 0);
            }
        }

        

        // --- Cells

        public static List<CellDoor> Cells = new List<CellDoor>();
        public static CellDoor AddCell(int hash, Vector3 pos, bool locked) {
            CellDoor door = new CellDoor(hash, pos, locked);
            Cells.Add(door);
            return door;
        }

        public static int GetClosestCellIDToPosition(Vector3 position, float maxRange = 1.5f) {
            int cellIndex = -1;
            float dist = maxRange;

            for(int i = 0; i < Cells.Count; ++i) {
                CellDoor cell = Cells[i];

                float distance = position.DistanceTo(cell.position);
                if(distance < dist) { cellIndex = i; }
            }

            return cellIndex;
        }

        public static bool ToggleCellLock(ref CellDoor door) {
            door.isLocked = !door.isLocked; // For some reason doesnt save the isLocked outside of scope

            API.shared.exported.doormanager.setDoorState(door.doorID, door.isLocked, 0);
            return door.isLocked;
        }
    }
}