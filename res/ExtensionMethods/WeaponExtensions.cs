using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.API;

namespace PBRP
{
    public static class WeaponExtensions
    {
        public static WeaponClass GetWeaponClass(this ServerAPI api, WeaponHash weapon)
        {
            switch (weapon)
            {
                case WeaponHash.APPistol:
                case WeaponHash.Pistol:
                case WeaponHash.SNSPistol:
                case WeaponHash.CombatPistol:
                    return WeaponClass.AutoHandgun;
                case WeaponHash.Revolver:
                case WeaponHash.MarksmanPistol:
                case WeaponHash.Pistol50:
                case WeaponHash.HeavyPistol:
                case WeaponHash.VintagePistol:
                    return WeaponClass.Handgun;
                case WeaponHash.MicroSMG:
                case WeaponHash.MachinePistol:
                case WeaponHash.SMG:
                case WeaponHash.AssaultSMG:
                case WeaponHash.CombatPDW:
                case WeaponHash.MiniSMG:
                case WeaponHash.Gusenberg:
                    return WeaponClass.MachineGun;
                case WeaponHash.AssaultRifle:
                case WeaponHash.CarbineRifle:
                case WeaponHash.SpecialCarbine:
                case WeaponHash.AdvancedRifle:
                case WeaponHash.BullpupRifle:
                case WeaponHash.CompactRifle:
                    return WeaponClass.AssaultRifle;
                case WeaponHash.SniperRifle:
                case WeaponHash.HeavySniper:
                    return WeaponClass.HighCalibreRifle;
                case WeaponHash.MarksmanRifle:
                    return WeaponClass.AutoHighCalibreRifle;
                case WeaponHash.PumpShotgun:
                case WeaponHash.SawnoffShotgun:
                case WeaponHash.BullpupShotgun:
                case WeaponHash.AssaultShotgun:
                case WeaponHash.HeavyShotgun:
                case WeaponHash.DoubleBarrelShotgun:
                case WeaponHash.Autoshotgun:
                    return WeaponClass.Shotgun;
                case WeaponHash.StunGun:
                    return WeaponClass.Tazer;
                case WeaponHash.Bat:
                case WeaponHash.Crowbar:
                case WeaponHash.Golfclub:
                case WeaponHash.Flashlight:
                case WeaponHash.Hammer:
                case WeaponHash.KnuckleDuster:
                case WeaponHash.Nightstick:
                case WeaponHash.Poolcue:
                case WeaponHash.Wrench:
                    return WeaponClass.Melee;
                case WeaponHash.Bottle:
                case WeaponHash.Battleaxe:
                case WeaponHash.Dagger:
                case WeaponHash.Hatchet:
                case WeaponHash.Knife:
                case WeaponHash.SwitchBlade:
                    return WeaponClass.Blade;
                case WeaponHash.Unarmed:
                    return WeaponClass.Fists;
                default:
                    return WeaponClass.Melee;
            }
        }

        public static bool IsWeaponClass(this ServerAPI api, WeaponHash weapon, WeaponClass weapclass)
        {
            if (GetWeaponClass(api, weapon) == weapclass) return true;
            else return false;
        }

        public static float BaseDamage(this WeaponHash weapon)
        {
            switch(weapon)
            {
                case WeaponHash.Unarmed: return 5.3f;
                case WeaponHash.StunGun:
                case WeaponHash.BZGas:
                case WeaponHash.FireExtinguisher:
                case WeaponHash.PetrolCan:
                    return 0f;
                default: return 10f;
            }
        }
    }
}
