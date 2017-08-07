using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptSharp;
using PBRP.Logs;

namespace PBRP
{
    public class LoginManager : Script
    {
        public delegate void onPlayerLoggedIn(Player player);
        public static event onPlayerLoggedIn OnPlayerLoggedIn = delegate { };
        private string logoBlob = "";
        public LoginManager() {
            API.onPlayerFinishedDownload += OnPlayerFinishDownload;
            API.onClientEventTrigger += OnClientEventTrigger;
            API.onPlayerBeginConnect += OnPlayerBeginConnect;
            API.onPlayerConnected += OnPlayerConnected;
        }

        private static void OnPlayerConnected(Client player)
        {
            player.FadeOut(100);
            if (player.socialClubName.IsBETA()) return;
            player.kick("You are not a BETA participant");
        }

        private void OnPlayerBeginConnect(Client player, CancelEventArgs cancelConnection)
        {
            if (!Weather.WeatherData.Any())
            {
                cancelConnection.Reason = "Server is initialising, please wait...";
                cancelConnection.Cancel = true;
                return;
            }
            if(!player.socialClubName.IsBETA())
            {
                cancelConnection.Reason = "You are not an accepted BETA Tester";
                cancelConnection.Cancel = true;
                return;
            }

            Ban ban = BanRepository.GetActiveBanBySocialClubName(player.socialClubName);

            if (ban != null)
            {
                ban.DisplayBanInfo(player);
                cancelConnection.Cancel = true;
                return;
            }
            

            if(!player.isCEFenabled)
            {
                API.sendNotificationToPlayer(player, "Please enable CEF in your GTA:N settings.");
                cancelConnection.Cancel = true;
            }

            player.FadeOut(100);
        }

        private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "onLoginSubmitted":
                    Login(sender, arguments[0].ToString(), arguments[1].ToString());
                    break;
                case "player-selected":
                    API.sendNativeToPlayer(sender, Hash._TRANSITION_FROM_BLURRED, 8.0);
                    Player newPlayer = Master.MasterData.Single(m => m.Client == sender).Players[(int)arguments[0]];
                    newPlayer.Client = sender;
                    Console.WriteLine("here");
                    newPlayer.MasterAccount = Master.MasterData.Single(m => m.Client == sender);
                    PlayerInit(newPlayer);
                    sender.FadeOutIn(200, 1000);
                    break;
                case "CreateNewCharacter":
                    API.sendNativeToPlayer(sender, Hash._TRANSITION_FROM_BLURRED, 8.0);
                    sender.setSkin((PedHash)PedSkins.Gender.Where(p => p.Value == 0).ElementAt(new Random().Next(0, PedSkins.Gender.Count(p => p.Value == 0))).Key); 
                    sender.FadeOutIn(300, 2000);
                    sender.position = new Vector3(9.284171, 6512.242, 31.87785);
                    sender.rotation = new Vector3(0, 0, 310);
                    sender.playAnimation("amb@world_human_hang_out_street@male_b@base", "base", 0);
                    sender.dimension = Globals.GetPlayerID(sender) + 1;
                    break;
                case "CreateCharUsernameCheck":
                    API.triggerClientEvent(sender, "createCharUsernameResult", PlayerRepository.GetPlayersWithUsernameCount(arguments[0].ToString()));
                    break;
                case "CancelCharCreate":
                    Master ma = Master.MasterData.Single(me => me.Client == sender);
                    string[] charNames = new string[5];
                    List<string> charImgs = new List<string>() { "", "", "", "", "" };
                    string[] factions = new string[5];
                    int count = 0;
                    foreach (Player p in ma.Players)
                    {
                        charNames[count] = p.Username.Roleplay();
                        charImgs[count] = p.Skin == 797459875 ? Images.SkinBlobs[p.Skin] : "";
                        if (p.FactionId != 0)
                            factions[count] =
                                $"{FactionRepository.GetFactionById(p.FactionId).Name}<br/>{RankRepository.GetRankByFactionAndId(p.FactionId, p.FactionRank).Title}";
                        else
                            factions[count] = "Civilian";
                        count++;
                    }
                    API.triggerClientEvent(sender, "playerLogResult", "success", ma.Players.Count,
                        string.Join(",", charNames), string.Join(",", factions), string.Join(".", charImgs));
                    API.triggerClientEvent(sender, "showLoginCam");
                    break;
                case "CreateNewCharacterProceed":
                    CreateCharacter(sender, arguments[0].ToString(), (int)arguments[1], new DateTime((int)arguments[4], (int)arguments[3], (int)arguments[2]));
                    break;
                case "CreateCharacterChangeGender":
                    if((int)arguments[0] == 0)
                        sender.setSkin((PedHash)PedSkins.Gender.Where(p => p.Value == 0).ElementAt(new Random().Next(0, PedSkins.Gender.Count(p => p.Value == 0))).Key);
                    else
                        sender.setSkin((PedHash)PedSkins.Gender.Where(p => p.Value == 1).ElementAt(new Random().Next(0, PedSkins.Gender.Count(p => p.Value == 1))).Key);
                    break;
                case "SkinSelectCategory":
                    SelectSkinCategory(sender, (int)arguments[0]);
                    break;
                case "SkinSelectChange":
                    SelectSkinChange(sender, (int)arguments[0]);
                    break;
                case "SkinCustomisationOptions":
                    GetSkinCustomisationOptions(sender, (int)arguments[0]);
                    break;
                case "SkinCustomTypeChange":
                    SkinCustomTypeChange(sender, (int)arguments[0], (int)arguments[1], (int)arguments[2]);
                    break;
                case "SkinCustomTextureChange":
                    SkinCustomTextureChange(sender, (int)arguments[0], (int)arguments[1], (int)arguments[2]);
                    break;
                case "CreateCharacterComplete":
                    CompleteCharCreation(sender);
                    break;
            }
        }

        private async void CompleteCharCreation(Client sender)
        {
            Master master = Master.MasterData.Single(me => me.Client == sender);

            if (master.CreatingCharacter == null) return;
            Player newChar = new Player
            {
                Username = master.CreatingCharacter.Username.Replace(" ", "_"),
                Level = 0,
                Health = 100,
                Armour = 0,
                FactionId = 0,
                FactionRank = 0,
                WeaponSkillData = "0,0,0,0,0,0,0,0",
                DateOfBirth = master.CreatingCharacter.DateOfBirth,
                Gender = master.CreatingCharacter.Gender,
                MasterAccount = master,
                MasterId = master.Id,
                Dimension = 0,
                TestDriveBan = false,
                Client = sender,
                LastPosition = new Vector3(-697.0174, 5803.262, 17.33096),
                LastRotation = new Vector3(0, 0, 63.30323)
            };

            List<int> AvailableSkins = new List<int>();

            AvailableSkins = GetAvailableSkins(master);
            int[] compIds = new int[12];
            int[] textureIds = new int[12];
            int[] drawableIds = new int[12];

            for (int i = 0; i < 12; i++)
            {
                textureIds[i] = API.fetchNativeFromPlayer<int>(sender, Hash.GET_PED_TEXTURE_VARIATION, sender, i);
                drawableIds[i] = API.fetchNativeFromPlayer<int>(sender, Hash.GET_PED_DRAWABLE_VARIATION, sender, i);
            }

            Skin newSkin = new Skin
            {
                Model = AvailableSkins[master.CreatingCharacter.CurrentSkinIndex],
                TextureIds = string.Join(",", textureIds),
                DrawableIds = string.Join(",", drawableIds),
                OwnerId = PlayerRepository.AddNewPlayer(newChar)
            };

            Master.MasterData.Single(me => me.Client == sender).Players =
                await PlayerRepository.GetAllPlayerDataByMasterAccount(master);
            Master.MasterData.Single(me => me.Client == sender).CreatingCharacter = null;

            newChar.Skin = SkinRepository.AddNewSkin(newSkin);

            PlayerRepository.UpdateAsync(newChar);

            PlayerInit(newChar);
            sender.FadeOutIn(200, 1300);
            API.triggerClientEvent(sender, "onCharCreated");
        }

        private void SkinCustomTextureChange(Client sender, int comId, int drawId, int texId)
        {
            API.sendNativeToAllPlayers(Hash.SET_PED_COMPONENT_VARIATION, sender, comId, drawId, texId, 0);
        }

        private void SkinCustomTypeChange(Client sender, int comId, int drawId, int texId)
        {
            API.sendNativeToAllPlayers(Hash.SET_PED_COMPONENT_VARIATION, sender, comId, drawId, texId, 0);
        }

        private void SelectSkinCategory(Client sender, int skinClass)
        {
            Master master = Master.MasterData.Single(m => m.Client == sender);
            Master.MasterData.Single(m => m.Client == sender).CreatingCharacter.SkinClass = skinClass;
            int classMax = 0;
            List<int> AvailableSkins = new List<int>();

            AvailableSkins = GetAvailableSkins(master);
            classMax = AvailableSkins.Count - 1;
            master.CreatingCharacter.CurrentSkinIndex = new Random().Next(0, classMax - 1);
            sender.setSkin((PedHash)AvailableSkins[master.CreatingCharacter.CurrentSkinIndex]);
            Master.MasterData.Single(m => m.Client == sender).CreatingCharacter.CurrentSkinIndex = master.CreatingCharacter.CurrentSkinIndex;
            API.triggerClientEvent(sender, "skinSelectClassSkin", classMax, master.CreatingCharacter.CurrentSkinIndex);
        }

        private void SelectSkinChange(Client sender, int index)
        {
            Master master = Master.MasterData.Single(m => m.Client == sender);
            List<int> AvailableSkins = new List<int>();

            AvailableSkins = GetAvailableSkins(master);
            master.CreatingCharacter.CurrentSkinIndex += index;
            if (master.CreatingCharacter.CurrentSkinIndex < 0)
                master.CreatingCharacter.CurrentSkinIndex = AvailableSkins.Count - 1;
            else if (master.CreatingCharacter.CurrentSkinIndex > AvailableSkins.Count - 1)
                master.CreatingCharacter.CurrentSkinIndex = 0;

            sender.setSkin((PedHash)AvailableSkins[master.CreatingCharacter.CurrentSkinIndex]);
            Master.MasterData.Single(m => m.Client == sender).CreatingCharacter.CurrentSkinIndex = master.CreatingCharacter.CurrentSkinIndex;
        }

        private void CreateCharacter(Client sender, string name, int gender, DateTime dob)
        {
            Master.MasterData.Single(m => m.Client == sender).CreatingCharacter = new Player()
            {
                Username = name,
                Gender = gender,
                DateOfBirth = dob,
                Client = sender,
            };

            API.triggerClientEvent(sender, "createCharResult", true);
        }

        private void GetSkinCustomisationOptions(Client player, int compId)
        {
            int drawables = API.fetchNativeFromPlayer<int>(player, Hash.GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS, player, compId);
            int textures = API.fetchNativeFromPlayer<int>(player, Hash.GET_NUMBER_OF_PED_TEXTURE_VARIATIONS, player, compId, 0);

            API.triggerClientEvent(player, "skinCustomOptions", drawables, textures);
        }

        private void OnPlayerFinishDownload(Client player)
        { 
            API.sendNativeToPlayer(player, Hash._TRANSITION_TO_BLURRED, 2.0);

            API.triggerClientEvent(player, "account_prompt_login", logoBlob);
            API.triggerClientEvent(player, "enableNotifications");

            Task.Run(async () =>
            {
                await Task.Delay(500);
                player.FadeIn(500);
                await Task.Delay(500);
                player.FadeIn(500);
            });

            //API.triggerClientEvent(player, "activateSnow", Weather.SnowActive, Weather.DeepPedTracks, Weather.DeepVehicleTracks);


        }

        public List<int> GetAvailableSkins(Master master)
        {
            switch (master.CreatingCharacter.SkinClass)
            {
                case 0:
                    return master.CreatingCharacter.Gender == 0 ? PedSkins.UpperClassMale : PedSkins.UpperClassFemale;
                case 1:
                    return master.CreatingCharacter.Gender == 0 ? PedSkins.MiddleClassMale : PedSkins.MiddleClassFemale;
                case 2:
                    return master.CreatingCharacter.Gender == 0 ? PedSkins.LowerClassMale : PedSkins.LowerClassFemale;
                case 3:
                    return master.CreatingCharacter.Gender == 0 ? PedSkins.ThugsMale : PedSkins.ThugsFemale;
                case 4:
                    if (master.CreatingCharacter.Gender == 0) return PedSkins.CountryMale;
                    else return PedSkins.CountryFemale;
                case 5:
                    if (master.CreatingCharacter.Gender == 0) return PedSkins.HoboMale;
                    else return PedSkins.HoboFemale;
                case 6:
                    if (master.CreatingCharacter.Gender == 0) return PedSkins.MiscMale;
                    else return PedSkins.MiscFemale;
                default:
                    return PedSkins.HoboMale;
            }
        }

        [Command("login")]
        public async void Login(Client sender, string username, string password)
        {
            Master masterAccount;
            if (username.Contains("@"))
                masterAccount = await MasterRepository.GetMasterDataByEmail(username);
            else
                masterAccount = await MasterRepository.GetMasterDataByName(username);

            if (masterAccount != null)
            {
                if (Crypter.CheckPassword(password, masterAccount.Password))
                {
                    string[] charNames = new string[5];
                    List<string> charImgs = new List<string>() { "", "", "", "", "" };
                    string[] factions = new string[5];
                    masterAccount.Players = await PlayerRepository.GetAllPlayerDataByMasterAccount(masterAccount);
                    masterAccount.Client = sender;
                    int count = 0;
                    foreach(Player p in masterAccount.Players)
                    {
                        charNames[count] = p.Username.Roleplay();
                        charImgs[count] = p.Skin == 797459875 ?  Images.SkinBlobs[p.Skin] : "";
                        if (p.FactionId != 0)
                            factions[count] =
                                $"{FactionRepository.GetFactionById(p.FactionId).Name}<br/>{RankRepository.GetRankByFactionAndId(p.FactionId, p.FactionRank).Title}";
                        else
                            factions[count] = "Civilian";
                        count++;
                    }

                    Master.MasterData.Add(masterAccount);
                    API.triggerClientEvent(sender, "playerLogResult", "success", masterAccount.Players.Count,
                        string.Join(",", charNames), string.Join(",", factions), string.Join(".", charImgs));

                    masterAccount.LatestLogin = Server.Date;
                    masterAccount.LatestIP = sender.address;

                    masterAccount.ActiveConnectionLog = new ConnectionLog(masterAccount.Id, -1, API.getPlayerAddress(sender));
                    ConnectionLogRepository.AddNew(masterAccount.ActiveConnectionLog);
                    MasterRepository.UpdateAsync(masterAccount);
                    masterAccount.AdminLevel = 6;
                }
                else
                {
                    API.triggerClientEvent(sender, "playerLogResult", "incorrect-pass");
                    API.SendErrorNotification(sender, "You have entered an incorrect password");
                }
            }
            else
            {
                API.triggerClientEvent(sender, "playerLogResult", "invalid-user");
                API.SendErrorNotification(sender, "You have entered an invalid username");
            }
        }

        
        public async void PlayerInit(Player player)
        {
            API.setPlayerNametag(player.Client, player.Username.Roleplay());
            API.setPlayerName(player.Client, player.Username.Roleplay());
            API.setPlayerHealth(player.Client, (int)player.Health);
            API.setPlayerArmor(player.Client, (int)player.Armour);
            API.setEntityDimension(player.Client, player.Dimension);

            player.IsInInterior = (player.Dimension > 0);
            if (player.IsInInterior) {
                player.PropertyIn = PropertyManager.Properties.FirstOrDefault(p => p.Dimension == player.Dimension);
            }

            Skin playerSkin = await SkinRepository.GetSkinById(player.Skin);
            API.setPlayerSkin(player.Client, (PedHash)playerSkin.Model);

            List<SkinVariations> skinVars = playerSkin.Variations();

            player.ChatIndicatorLabel = API.createTextLabel("typing...", player.Client.position, 50, 0.35f, false, player.Dimension);
            player.ChatIndicatorLabel.color = new Color(0, 0, 0, 0);
            player.ChatIndicatorLabel.attachTo(player.Client, null, new Vector3(0, 0, 0.9f), new Vector3());

            for (int i = 0; i < 12; i++)
            {
                API.setPlayerClothes(player.Client, i, skinVars[i].Drawable, skinVars[i].Texture);
            }

            player.IsLogged = true;

            player.MasterAccount.ActiveConnectionLog.PlayerId = player.Id;
            ConnectionLogRepository.UpdateAsync(player.MasterAccount.ActiveConnectionLog);

            if (player.FactionId > 0)
                player.Faction = Faction.FactionData.Single(f => f.Id == player.FactionId);

            player.Weapons = await WeaponRepository.GetAllWeaponsByPlayerIdAsync(player.Id);

            foreach(Weapon w in player.Weapons)
            {
                API.givePlayerWeapon(player.Client, w.Model, w.Ammo, false, false);
            }

            player.Inventory = await InventoryRepository.GetInventoryByOwnerIdAsync(player.Id);

            player.PopulateWeaponSkills();

            await player.LoadPlayerVehicles();

            foreach (Inventory i in player.Inventory)
            {
                if (i.IsPhone())
                {
                    Phone p = PhoneRepository.GetPhoneByIMEI(long.Parse(i.Value));

                    if (p.IsPrimary)
                    {
                        player.PrimaryPhone = p;
                        if (p.PoweredOn)
                        {
                            if(p.BatteryLevel > 3)
                                p.TurnOn(player.Client);
                            else
                                p.TurnOff(player.Client);
                        }
                        else
                        {
                            p.TurnOff(player.Client);
                        }
                        break;
                    }
                }
            }

            player.InEvent = PlayerEvent.None;

            player.Client.freeze(false);
            player.Client.ToggleCursorLock(false);
            player.Client.transparency = 255;
            API.sendNativeToAllPlayers(Hash.SET_CURRENT_PED_WEAPON, player.Client, (int)WeaponHash.Unarmed, true);

            Player.PlayerData.Add(player.Client, player);

            if (player.LastPosition != new Vector3(0, 0, 0))
                player.Client.position = player.LastPosition;

            API.triggerClientEvent(player.Client, "hasLoggedIn", 
                ((char)player.MasterAccount.KeyCursor).ToString(), 
                ((char)player.MasterAccount.KeyInventory).ToString(), 
                ((char)player.MasterAccount.KeyInteract).ToString());

            API.triggerClientEvent(player.Client, "executeSkillTimer");
            API.triggerClientEvent(player.Client, "loadScoreboard");
            API.triggerClientEvent(player.Client, "initInteractionMenu");

            API.sendNativeToPlayer(player.Client, Hash.SET_CAM_AFFECTS_AIMING, player.Client, true);

            //API.sendNativeToPlayer(player.Client, Hash.SET_PED_CAN_BE_SHOT_IN_VEHICLE, player.Client, true);
            player.Client.invincible = true;
            
            for(int i = 0; i < 12; i++)
            {
                API.exported.doormanager.refreshDoorState(i);
            }
        
            UpdateNatives(player);

            PrisonManager.LoadPrisonSentenceForPlayer(player);
            OnPlayerLoggedIn(player);
        }

        private void UpdateNatives(Player player)
        {
            foreach(Vehicle v in Vehicle.VehicleData.Values)
            {
                if(v.IsDealerVehicle)
                {
                    if (!API.getVehicleOccupants(v.Entity).Any())
                    {
                        v.DealershipEmployee.WarpIntoVehicleForPlayer(player.Client, v.Entity, 0);
                        v.DealershipEmployee.WarpIntoVehicleForPlayer(player.Client, v.Entity, -1);
                    }
                    else
                    {
                        v.DealershipEmployee.WarpIntoVehicleForPlayer(player.Client, v.Entity, 1);
                        v.DealershipEmployee.WarpIntoVehicleForPlayer(player.Client, v.Entity, 0);
                    }
                }
            }

            Weather weather = Weather.WeatherData.FirstOrDefault(w => w.Hour == Server.Date.Hour);
            API.sendNativeToAllPlayers(Hash.ADVANCE_CLOCK_TIME_TO, Server.Date.Hour, Server.Date.Minute, 0);
            if (weather != null)
            {
                API.sendNativeToAllPlayers(Hash.SET_WIND_SPEED, weather.WindSpeed);
                API.sendNativeToAllPlayers(Hash._SET_RAIN_FX_INTENSITY, weather.RainLevel);
                API.sendNativeToAllPlayers(0xF36199225D6D8C86, weather.RainLevel);
                API.triggerClientEvent(player.Client, "setWeatherForPlayer", weather.WeatherType);
                API.triggerClientEvent(player.Client, "activateSnow", weather.SnowActive, weather.DeepPedTracks, weather.DeepVehicleTracks);
            }

            foreach (Inventory i in Inventory.DroppedItems)
            {
                i.ApplyPhysics();
            }

            foreach(Weapon w in Weapon.DroppedWeapons)
            {
                w.ApplyPhysics();
            }
        }
    }
}
