using GrandTheftMultiplayer.Shared;
using System;

namespace PBRP
{
    public class AttackData
    {
        public AttackData(int attackerId, WeaponHash weapUsed, float distanceBetween)
        {
            AttackerId = attackerId;
            WeaponUsed = weapUsed;
            DistanceBetween = distanceBetween;
            TimeOfAttack = Server.Date;
        }
        public int AttackerId { get; set; }
        public WeaponHash WeaponUsed { get; set; }
        public float DistanceBetween { get; set; }
        public DateTime TimeOfAttack { get; set; }
    }
}
