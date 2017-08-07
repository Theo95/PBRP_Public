using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;


namespace PBRP
{
    public class BankCommands : Script
    {
        [Command("bank")]
        public void AccessBank(Client player)
        {
            if(player.position.DistanceTo(new Vector3(-113.4233, 6469.712, 31.62671)) < 2)
            {
                Player p = Player.PlayerData[player];
                player.position = new Vector3(-113.4233, 6469.712, 31.62671);
                player.rotation = new Vector3(0, 0, -20);

                API.sendNativeToPlayer(player, Hash.TASK_PAUSE, player, 240000000);
                
                API.triggerClientEvent(player, "onExecuteBank", BankManager.BankClerk.handle);

                API.triggerClientEvent(player, "showBankOptions");

            }
        }
    }
}
