using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using System;

namespace PBRP
{
    public class InjuryManager
    {
        public InjuryManager()
        {
            Player.OnPlayerSecond += OnPlayerSecond;
        }

        private void OnPlayerSecond(Player player)
        {
            if(player.BulletWounds > 0)
            {
                player.BloodLevel -= player.BulletWounds * 0.07f;
            }
            if(player.StabWounds > 0)
            {
                player.BloodLevel -= player.StabWounds * 0.12f;
            }

            if(player.Downed)
            {
                API.shared.sendNativeToAllPlayers(Hash.SET_PED_TO_RAGDOLL, player.Client, -1, -1, 0, true, true, true);
            }
            if(player.BloodLevel < 50)
            {
                player.Client.FadeOut(Convert.ToInt32(player.BloodLevel * 50));
            }
        }
    }
}
