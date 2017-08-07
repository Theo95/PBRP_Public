using Newtonsoft.Json;
using PBRP.Logs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;

namespace PBRP
{
    public enum WeaponType
    {
        AdminSpawned = 1,
        Faction = 2,
        GunShop = 3
    }

    public enum WeaponClass
    {
        Handgun = 0,
        AutoHandgun = 1,
        MachineGun = 2,
        AssaultRifle = 3,
        HighCalibreRifle = 4,
        AutoHighCalibreRifle = 5,
        Shotgun = 6,
        Tazer = 7,
        Melee = 8,
        Blade = 9,
        Fists = 10
    }

    public enum ShotType
    {
        Missed = 0,
        HitPlayer = 1,
        ShootingRangeFail = 2,
        ShootingRangeAverage = 3,
        ShootingRangeGood = 4,
        ShootingRangeGreat = 5,
        ShootingRangePerfect = 6
    }

    public enum WeaponOwnerType
    {
        Player = 0,
        Vehicle = 1,
        Property = 2,
        Armoury = 3,
        GunStore = 4
    }

    public class Weapon
    {
        public delegate void onAmmoChange(Player player, int weapon, int ammo);
        public static event onAmmoChange OnAmmoChange = delegate { };

        [Key]
        [Column("id")]
        public int Id { get; set; }
        public WeaponHash Model { get; set; }
        public int Ammo { get; set; }
        public WeaponType Type { get; set; }

        public int CurrentOwnerId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DroppedAt { get; set; }

        public double DroppedX { get; set; }
        public double DroppedY { get; set; }
        public double DroppedZ { get; set; }
        public double DroppedRX { get; set; }
        public double DroppedRY { get; set; }
        public double DroppedRZ { get; set; }
        public int DroppedDimension { get; set; }
        [NotMapped]
        public string Name { get { return NameToHash.Weapons.Keys.First(k => NameToHash.Weapons[k] == Model); } }

        [NotMapped]
        public NetHandle DroppedObj { get; set; }

        [NotMapped]
        public Vector3 DroppedPos
        {
            get => new Vector3(DroppedX, DroppedY, DroppedZ);
            set { DroppedX = value.X; DroppedY = value.Y; DroppedZ = value.Z; }
        }

        [NotMapped]
        public Vector3 DroppedRot
        {
            get => new Vector3(DroppedRX, DroppedRY, DroppedRZ);
            set { DroppedRX = value.X; DroppedRY = value.Y; DroppedRZ = value.Z; }
        }

        [NotMapped]
        public static List<Weapon> DroppedWeapons = new List<Weapon>();

        public void AmmoChange(Player player, int index, int ammo)
        {
            OnAmmoChange(player, index, ammo);
        }

        public static async void ApplyDamageToPlayer(Player player, Player attacker)
        {
            WeaponHash attackedWith = attacker.Client.currentWeapon;
            player.Health -= attackedWith.BaseDamage();

            if (player.ActiveDeathLog == null)
            {
                player.ActiveAttackInfo = new List<AttackData>();
                player.ActiveDeathLog = new AttackLog(player.Id);
                await AttackLogRepository.AddNewAsync(player.ActiveDeathLog);
            }
            player.ActiveAttackInfo.Add(new AttackData(attacker.Id, attackedWith, player.Client.position.DistanceTo(attacker.Client.position)));
            player.ActiveDeathLog.AttackData = JsonConvert.SerializeObject(player.ActiveAttackInfo);
           
            if (player.Health < 0)
            {
                player.Health = 1;
                API.shared.sendNativeToPlayer(player.Client, Hash.SET_PED_TO_RAGDOLL, player.Client, -1, -1, 0, true, true, true);
                player.ActiveDeathLog.DownedTime = Server.Date;
                player.Downed = true;
            }
            player.Client.health = Convert.ToInt32(player.Health);
            switch (API.shared.GetWeaponClass(attackedWith))
            {
                case WeaponClass.Fists:
                case WeaponClass.Melee:
                    player.MeleeHits++;
                    if(API.shared.RandomNumber(100) % 2 == 0)
                        player.BeginInjuryBlur(2500, 0, 200f, true);
                    break;
                case WeaponClass.Blade:
                    player.StabWounds++;
                    break;
                case WeaponClass.Tazer:
                    break;
                default:
                    player.BulletWounds++;
                    break;
            }

            if (attackedWith == WeaponHash.StunGun)
            {
                await Task.Run(async () =>
                {
                    int ragdollTime;

                    if (player.Client.isInVehicle)
                    {
                        API.shared.playPlayerAnimation(player.Client, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "weapon@w_pi_stungun", "damage");
                        await Task.Delay(new Random().Next(10000, 15000));
                        API.shared.stopPlayerAnimation(player.Client);
                        return;
                    }
                    if (!player.Client.isOnLadder && !player.Client.IsClimbing() && !player.Client.IsVaulting())
                    {
                        switch (new Random().Next(1, 3))
                        {
                            case 1:
                                API.shared.playPlayerAnimation(player.Client, (int)AnimationFlags.StopOnLastFrame, "ragdoll@human", "electrocute");
                                break;
                            case 2:
                                API.shared.playPlayerAnimation(player.Client, (int)AnimationFlags.StopOnLastFrame, "stungun@standing", "damage");
                                break;
                            default:
                                API.shared.playPlayerAnimation(player.Client, (int)AnimationFlags.StopOnLastFrame, "weapon@w_pi_stungun", "damage");
                                break;
                        }
                        await Task.Delay(new Random().Next(2000, 5000));
                        ragdollTime = 10000;
                    }
                    else
                    {
                        ragdollTime = new Random().Next(2000, 8000) + 10000;
                    }
                    API.shared.sendNativeToPlayer(player.Client, Hash.SET_PED_TO_RAGDOLL, player.Client, ragdollTime, ragdollTime, 0, true, true, true);
                    API.shared.stopPlayerAnimation(player.Client);
                });
               
            }
                       
            AttackLogRepository.UpdateAsync(player.ActiveDeathLog);
        }

        public static void IncreaseWeaponSkill(Player player, ShotType shotType)
        {
            float increase = 0;
            switch (shotType)
            {
                case ShotType.Missed:
                    increase = API.shared.GetWeaponClass(player.Client.currentWeapon) == WeaponClass.Tazer ? 0.4f : 0.025f;
                    break;
                case ShotType.HitPlayer:
                    increase = API.shared.GetWeaponClass(player.Client.currentWeapon) == WeaponClass.Tazer ? 0.5f : 0.04f;
                    break;
            }

            player.WeaponSkill[API.shared.GetWeaponClass(player.Client.currentWeapon)] += (increase / player.WeaponSkill[API.shared.GetWeaponClass(player.Client.currentWeapon)]);

            API.shared.triggerClientEvent(player.Client, "updateWeaponSkill", player.WeaponSkill.Values.ToArray());
        }
    }
}
