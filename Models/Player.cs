using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using PBRP.Logs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace PBRP
{
    public enum PlayerEvent
    {
        None = 0,
        DrivingTest = 1,
        VehicleDealership = 2,
        TestDrive = 3,
        UsingBank = 4,
        UsingATM = 5,
        RefillingVehicle = 6,
        BusinessInteract = 7,
        AccessingInventory = 8,
    }

    [Flags]
    public enum AnimationFlags
    {
        Loop = 1 << 0,
        StopOnLastFrame = 1 << 1,
        OnlyAnimateUpperBody = 1 << 4,
        AllowPlayerControl = 1 << 5,
        Cancellable = 1 << 7
    }

    public enum DamageType
    {
        Fall = 0,
        VehicleHit = 1,
        Fire = 2,
        Explosion = 3,
    }

    [Table("players")]
    public class Player : Script
    {
        [NotMapped]
        public static Dictionary<Client, Player> PlayerData = new Dictionary<Client, Player>();

        public delegate void onPlayerSecond(Player player);
        public static event onPlayerSecond OnPlayerSecond = delegate { };
        public delegate void onPlayerMilli(Player player);
        public static event onPlayerMilli OnPlayerMilli = delegate { };

        [NotMapped]
        public static Client[] IDs;

        public Player() {
            IsInFlyMode = false;
        }
        public Player(Client client) : base()
        {
            Client = client;
            WeaponSkill = new Dictionary<WeaponClass, float>();
        }

        [Key]
        public int Id { get; set; }

        [NotMapped]
        public int RealId => Globals.GetPlayerID(Client);

        public string Username { get; set; }

        public long Money { get; set; }
        [NotMapped]
        public Inventory SelectedCash { get; set; }
        [NotMapped]
        public BankAccount SelectedCardAccount { get; set; }

        public int Skin { get; set; }
        public double Health { get; set; }
        public double Armour { get; set; }
        public bool Downed { get; set; }
        public int StabWounds { get; set; }
        public int BulletWounds { get; set; }
        public int MeleeHits { get; set; }
        [NotMapped]
        public int Ping { get; set; }

        public double BloodLevel { get; set; }

        public int Level { get; set; }

        public int Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string WeaponSkillData { get; set; }

        public int FactionId { get; set; }
        public int FactionRank { get; set; }

        [NotMapped]
        public PlayerEvent InEvent { get; set; }

        public double LastPosX { get; set; }
        public double LastPosY { get; set; }
        public double LastPosZ { get; set; }

        [NotMapped]
        public Vector3 LastPosition {
            get => new Vector3(LastPosX, LastPosY, LastPosZ);
            set { LastPosX = value.X; LastPosY = value.Y; LastPosZ = value.Z; }
        }

        public double LastRotX { get; set; }
        public double LastRotY { get; set; }
        public double LastRotZ { get; set; }

        [NotMapped]
        public Vector3 LastRotation
        {
            get => new Vector3(LastRotX, LastRotY, LastRotZ);
            set { LastRotX = value.X; LastRotY = value.Y; LastRotZ = value.Z; }
        }

        public int Dimension { get; set; }

        public int MasterId { get; set; }

        public bool TestDriveBan { get; set; }

        [NotMapped]
        public bool CanAttack { get; set; }
        [NotMapped]
        public bool IsClimbing { get; set; }
        [NotMapped]
        public bool IsVaulting { get; set; }
        [NotMapped]
        public int HasEnteredGasStation { get; set; }
        [NotMapped]
        public FuelType FuelTypeSelected { get; set; }
        [NotMapped]
        public bool IsAiming { get; set; }
        [NotMapped]
        public bool IsInFirstPerson { get; set; }
        [NotMapped]
        public int SkinClass { get; set; }
        [NotMapped]
        public int CurrentSkinIndex { get; set; }
        [NotMapped]
        public bool IsCuffed { get; set; }
        [NotMapped]
        public Player AwaitingFactionInvite { get; set; }
        [NotMapped]
        public bool IsShooting { get; set; }
        [NotMapped]
        public bool IsInFlyMode { get; set; }
        [NotMapped]
        public bool IncognitoMode { get; set; }
        [NotMapped]
        public bool IsSpectating { get; set; }
        [NotMapped]
        public bool IsBlurred { get; set; }
        [NotMapped]
        public Ban CurrentBanData { get; set; }
        [NotMapped]
        public List<AttackData> ActiveAttackInfo { get; set; }
        [NotMapped]
        public AttackLog ActiveDeathLog { get; set; } 
        [NotMapped]
        public Business BusinessInteractingWith { get; set; }
        [NotMapped]
        public Property PropertyEnterExit { get; set; }

        [NotMapped]
        public int RepairType { get; set; }

        private bool _logged = false;
        [NotMapped]
        public bool IsLogged { get => _logged;
            set { _logged = value; API.triggerClientEvent(Client, "onLogIn"); } }
        [NotMapped]
        public int IsFalling { get; set; }
        [NotMapped]
        public Phone PrimaryPhone { get; set; }
        [NotMapped]
        public int ItemBeingGiven { get; set; }
        [NotMapped]
        public Client Client { get; set; }
        [NotMapped]
        public string VehicleDealerClass { get; set; }
        [NotMapped]
        public Faction Faction { get; set; }
        [NotMapped]
        public Master MasterAccount { get; set; }
        [NotMapped]
        public List<Vehicle> OwnedVehicles { get; set; }
        [NotMapped]
        public int AccessingBank { get; set; }
        [NotMapped]
        public int TransactionType { get; set; }
        [NotMapped]
        public BankAccount CreatingAccount { get; set; }
        [NotMapped]
        public Inventory CreatingAccountCard { get; set; }
        [NotMapped]
        public PhoneContact DeletingContact { get; set; }
        [NotMapped]
        public List<Weapon> Weapons { get; set; }
        [NotMapped]
        public List<Inventory> Inventory { get; set; }

        [NotMapped]
        public Player PlayerInteractingWith { get; set; }

        [NotMapped]
        public Vehicle VehicleInteractingWith { get; set; }

        [NotMapped]
        public Dictionary<WeaponClass, float> WeaponSkill { get; set; }
        [NotMapped]
        public NetHandle FishingRod { get; set; }
        [NotMapped]
        public bool IsFishing { get; set; }
        [NotMapped]
        public bool CaughtFish { get; set; }
        [NotMapped]
        public Fishing.Fishtype NewFish { get; set; }
        [NotMapped]
        public int NewFishWeight { get; set; }
        [NotMapped]
        public InventoryType? AwaitingInventorySelection { get; set; }
        [NotMapped]
        public Player ScoreboardActionPlayer { get; set; }
        [NotMapped]
        public TextLabel ChatIndicatorLabel { get; set; }
        [NotMapped]
        public int EmsCallStatus = 0; //1=Waiting for service name, 2=Waiting for desc, 3=Awaiting name
        [NotMapped]
        public EmsCall PlayerEmsCallInProgress { get; set; }


        // -- Property Additions

        [NotMapped]
        public bool IsInInterior = false;
        [NotMapped]
        public Property PropertyIn = null;
        [NotMapped]
        public List<ShopItem> ShoppingCart { get; set; }

        // -- End Property Additions

        [NotMapped]
        public float ATMEditDistance { get; set; }
        [NotMapped]
        public float ATMEditHeight { get; set; }
        [NotMapped]
        public Vector3 ATMCamPos { get; set; }
        [NotMapped]
        public Vector3 ATMCamOffset { get; set; }
        [NotMapped]
        public bool TogSpeedo = true;

        public static Player GetPlayerData(string partOfNameOrId)
        {
            try
            {
                int id = int.Parse(partOfNameOrId);
                return IDs[id] != null ? PlayerData[IDs[id]] : null;
            }
            catch
            {
                try
                {
                    Player target = PlayerData.Single(p => p.Value.Username.Contains(partOfNameOrId, StringComparison.OrdinalIgnoreCase)).Value;
                    return target;
                }
                catch { return null; }
            }
        }

        public void PopulateWeaponSkills()
        {
            string[] skills = WeaponSkillData.Split(',');

            WeaponSkill = new Dictionary<WeaponClass, float>()
            {
                {WeaponClass.AutoHandgun, float.Parse(skills[0]) },
                {WeaponClass.Handgun, float.Parse(skills[1]) },
                {WeaponClass.MachineGun, float.Parse(skills[2]) },
                {WeaponClass.AssaultRifle, float.Parse(skills[3]) },
                {WeaponClass.HighCalibreRifle, float.Parse(skills[4]) },
                {WeaponClass.AutoHighCalibreRifle, float.Parse(skills[5]) },
                {WeaponClass.Shotgun, float.Parse(skills[6]) },
                {WeaponClass.Tazer, float.Parse(skills[7]) }
            };
            API.triggerClientEvent(Client, "updateWeaponSkill", WeaponSkillData);
        }

        public void SetWeaponSkill(WeaponClass weapclass, float value)
        {
            WeaponSkill[weapclass] = value;
            WeaponSkillData = string.Join(",", WeaponSkill.Values.Select(e => e));
        }

        public void SetCurrentWeapon(WeaponHash weapModel)
        {
            if(weapModel == WeaponHash.Unarmed) { API.givePlayerWeapon(Client, weapModel, 1, true, true); return; }
            if (Weapons.FirstOrDefault(w => w.Model == weapModel) == null) return;

            Client.removeAllWeapons();

            foreach(Weapon weap in Weapons)
            {
                API.givePlayerWeapon(Client, weap.Model, weap.Ammo, weap.Model == weapModel, true);
            }
        }

        public Vehicle ReloadPlayerVehicle(Vehicle veh)
        {
            OwnedVehicles.Remove(veh);
            Vehicle.VehicleData.Remove(veh.Entity);

            API.deleteEntity(veh.Entity);

            Vehicle v = VehicleRepository.GetVehicleById(veh.Id);
            LoadPlayerVehicle(v);

            OwnedVehicles.Add(v);
            return v;
        }

        public async void LoadPlayerVehicle(Vehicle v)
        {
            v.Entity = API.createVehicle((VehicleHash)v.Model, v.SavePosition, v.SaveRotation, 0, 0, v.Dimension);
            if (v.Color1.Length > 3)
                v.Entity.customPrimaryColor = v.CustomColor(v.Color1);
            else
                v.Entity.primaryColor = int.Parse(v.Color1);
            if (v.Color2.Length > 3)
                v.Entity.customSecondaryColor = v.CustomColor(v.Color2);
            else
                v.Entity.secondaryColor = int.Parse(v.Color2);

            v.TrunkItems = await InventoryRepository.GetInventoryByOwnerIdAsync(v.Id, InventoryOwnerType.Vehicle);
            v.Entity.CreateID();
            if (v.TrunkItems.Count(t => t.Value == v.Key) != 0)
            {
                v.KeyInIgnition = true;
                v.IsEngineOn = true;
            }
            v.Entity.engineStatus = v.IsEngineOn;
            v.Entity.health = (float)v.Health;
            v.Entity.invincible = false;
            v.Entity.numberPlate = v.LicensePlate;
            v.IsAdminVehicle = false;
            v.IsDealerVehicle = false;
            v.Entity.numberPlateStyle = v.LicensePlateStyle;
            API.sendNativeToAllPlayers(Hash.SET_VEHICLE_BODY_HEALTH, v.Entity, v.BodyHealth);
            API.sendNativeToAllPlayers(Hash.SET_VEHICLE_DIRT_LEVEL, v.Entity, v.DirtLevel);
            v.UnoccupiedPosition = v.SavePosition;
            v.UnoccupiedRotation = v.SaveRotation;
            if (v.RepairType != 0)
            {
                API.setEntityCollisionless(v.Entity, true);
                API.setEntityTransparency(v.Entity, 25);
                v.Entity.dimension = 2000 + RealId;
                v.Entity.engineStatus = false;
            }

            for (int w = 0; w < v.DoorData.Length; w++)
                API.breakVehicleDoor(v.Entity, w, v.DoorData[w] == 1);

            for (int w = 0; w < v.WindowData.Length; w++)
                API.breakVehicleWindow(v.Entity, w, v.WindowData[w] == 1);

            for (int i = 0; i < 7; i++)
                API.popVehicleTyre(v.Entity, i, v.TyresData[i] == 1);

            API.sendNativeToAllPlayers(Hash._SET_VEHICLE_PAINT_FADE, v.Entity, v.PaintFade);

            API.sendNativeToAllPlayers(Hash.CLEAR_AREA_OF_OBJECTS, v.SavePosition.X, v.SavePosition.Y, v.SavePosition.Z, 2f, 2);

            API.setVehicleLocked(v.Entity, v.Locked);
            Vehicle.VehicleData.Add(v.Entity, v);
        }

        public async Task LoadPlayerVehicles()
        {
            OwnedVehicles = await VehicleRepository.GetAllVehiclesByOwnerId(Id);
            foreach (Vehicle v in OwnedVehicles)
            {
                LoadPlayerVehicle(v);            
            }
        }

        public void SavePlayerVehicles()
        {
            foreach (Vehicle v in OwnedVehicles)
            {
                Vehicle veh = Vehicle.VehicleData[v.Entity];

                veh.IsEngineOn = v.Entity.engineStatus;
                veh.Health = v.Entity.health;
                veh.SavePosition = v.Entity.position;
                veh.SaveRotation = v.Entity.rotation;
                veh.CalculateTaintedFuel();
                veh.Dimension = v.Entity.dimension;
                
                for(int i = 0; i < 7; i++)
                    v.TyresData[i] = API.isVehicleTyrePopped(v.Entity, i) ? 1 : 0;

                for(int w = 0; w < v.WindowData.Length; w++)
                    v.WindowData[w] = API.isVehicleWindowBroken(v.Entity, w) ? 1 : 0;

                for (int w = 0; w < v.DoorData.Length; w++)
                    v.DoorData[w] = API.isVehicleDoorBroken(v.Entity, w) ? 1 : 0;

                VehicleRepository.UpdateAsync(veh);
                v.Entity.Delete();
            }
        }

        public static Vector3 GetPositionInFrontOfPlayer(Client player, float distance, double height)
        {
            
            double xOff = -(Math.Sin((player.rotation.Z * Math.PI) / 180) * distance);
            double yOff = Math.Cos((player.rotation.Z * Math.PI) / 180) * distance;

            return player.position.Add(new Vector3(xOff, yOff, height));
        }

        public static Vector3 GetPositionInFrontOfPoint(Vector3 point, float rot, float dist)
        {
            double xOff = -(Math.Sin((rot * Math.PI) / 180) * dist);
            double yOff = Math.Cos((rot * Math.PI) / 180) * dist;

            return point.Add(new Vector3(xOff, yOff, 0));
        }

        public static Vector3 GetPositionInBehindOfPoint(Vector3 point, float rot, float dist)
        {
            double xOff = (Math.Cos((rot * Math.PI) / 180) * dist);
            double yOff = (Math.Sin((rot * Math.PI) / 180) * dist);

            return point.Add(new Vector3(xOff, yOff, 0));
        }

        public static Vector3 GetPositionLeftOfPlayer(Vector3 left, float rot, float dist)
        {
            double xOff = -(Math.Sin((rot * Math.PI) / 180) * dist);
            double yOff = -(Math.Cos((rot * Math.PI) / 180) * dist);

            return left.Add(new Vector3(xOff, yOff, 0));
        }

        public static Vector3 GetPositionRightOfPlayer(Vector3 right, float rot, float dist)
        {
            double xOff = Math.Sin((rot * Math.PI) / 180) * dist;
            double yOff = Math.Cos((rot * Math.PI) / 180) * dist;

            return right.Add(new Vector3(xOff, yOff, 0));
        }

        public static void OnSecond(Player player)
        {
            OnPlayerSecond(player);
        }

        public void OnMilli()
        {

        }

        public void Pause()
        {
            API.sendNativeToPlayer(Client, Hash.TASK_PAUSE, Client, 24000000000);
        }

        public void ApplyDamage(DamageType type, int multiplier)
        {
            int damageValue = 0;
            if (type == DamageType.Fall)
                damageValue = 10;

            Health -= damageValue * multiplier;
            if(Health <= 0)
            {
                Health = 1;
                if (!Downed) EnterInjuredState();
            }
            API.setPlayerHealth(Client, (int)Health);

            if (type != DamageType.Fall) return;
            if(multiplier > 2)
                API.shared.sendNativeToAllPlayers(Hash.SET_PED_TO_RAGDOLL, Client, (multiplier * 10000), (multiplier * 10000), 0, true, true, true);
            //API.triggerClientEvent(Client, "playScreenEffect", "DeathFailMPDark", (damageValue * multiplier) * 1000, false);
        }

        public void EnterInjuredState()
        {
            API.shared.sendNativeToAllPlayers(Hash.SET_PED_TO_RAGDOLL, Client, -1, -1, 0, true, true, true);
            Downed = true;

            API.triggerClientEvent(Client, "playerHasBeenDowned", true);

            if (BulletWounds > StabWounds && BulletWounds > MeleeHits)
                API.ShowPopupMessage(Client, "You are injured!", "You have been shot multiple times, causing you to fall to the ground.");
            else if(StabWounds > BulletWounds && StabWounds > MeleeHits)
                API.ShowPopupMessage(Client, "You are injured!", "You have been stabbed multiple times, and are bleeding out rapidly.");
        }

        public void LeaveInjuredState()
        {
            API.shared.sendNativeToAllPlayers(Hash.SET_PED_TO_RAGDOLL, Client, 0, 0, 0, true, true, true);
            Downed = false;

            API.triggerClientEvent(Client, "playerHasBeenDowned", false);
        }
    }
}