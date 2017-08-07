using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;

namespace PBRP
{
    public class PDArrestCommands : Script {
        [Command("lockcell")]
        public void LockCellCommand(Client player) {
            if (!FactionManager.IsPlayerAnOfficer(Player.PlayerData[player])) { Message.NotAuthorised(player); return; }

            int cellID = PDManager.GetClosestCellIDToPosition(player.position);
            if(cellID == -1) { Message.Syntax(player, "You are not close enough to any cell");  return; }

            PDManager.CellDoor cell = PDManager.Cells[cellID];
            bool isLocked = PDManager.ToggleCellLock(ref cell);

            API.SendCloseMessage(player, 15.0f, "~#C2A2DA~", API.shared.getPlayerName(player) + " places a key inside the lock of the cell door and " + (isLocked ? "locks" : "unlocks") + " it.");
            Utils.SetEntityFacingVector(player, cell.position);
        }
    }
}
