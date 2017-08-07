using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PBRP
{
    public class GeneralAdminCommands : Script
    {
        public static float waterX { get; set; }
        public static float waterY { get; set; }
        public static float waterHeight { get; set; }
        public static float waterRadius { get; set; }
        public GeneralAdminCommands()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if(eventName == "cancel-fly-mode")
            {
                sender.position = Player.PlayerData[sender].LastPosition;
                API.setEntityCollisionless(sender.handle, false);
                API.setEntityTransparency(sender.handle, 255);
                API.freezePlayer(sender, false);
                Player.PlayerData[sender].IsInFlyMode = false;
            }
        }

        [Command("fly", "")]
        public void ActivateFlyMode(Client sender)
        {
            Player user = Player.PlayerData[sender];
            if (user.MasterAccount.AdminLevel > 2 )
            {
                if (!sender.isInVehicle)
                {
                    if (!user.IsInFlyMode)
                    {
                        if (sender.isInVehicle) API.warpPlayerOutOfVehicle(sender);
                        //API.setEntityCollisionless(sender.handle, true);
                        //API.setEntityTransparency(sender.handle, 0);
                        API.freezePlayer(sender, true);
                        user.IsInFlyMode = true;
                        user.Client.nametagVisible = false;
                        user.LastPosition = sender.position;
                    }
                    else
                    {
                        API.setEntityCollisionless(sender.handle, false);
                        //API.setEntityTransparency(sender.handle, 255);
                        user.Client.nametagVisible = true;
                        API.freezePlayer(sender, false);
                        user.IsInFlyMode = false;
                    }
                    API.triggerClientEvent(sender, "activateFlyMode");
                    Console.WriteLine("inflymode");
                    Player.PlayerData[sender] = user;
                }
                else API.SendErrorNotification(sender, "You can't use fly mode whilst in a vehicle.");
            }
            else Message.NotAuthorised(sender);
        }

        [Command("incognito", "")]
        public void ActivateInvisibilty(Client sender)
        {
            Player user = Player.PlayerData[sender];
            if (user.MasterAccount.AdminLevel < 4) { return; }

            if (!user.IncognitoMode)
            {
                sender.transparency = 0;
                sender.nametagVisible = false;
                API.SendInfoNotification(sender, "~#ccca57~Incognito mode activated");
                if (sender.isInVehicle) API.setEntityTransparency(sender.vehicle, 0);
            }
            else
            {
                sender.transparency = 255;
                sender.nametagVisible = true;
                API.sendNotificationToPlayer(sender, "~#ccca57~Incognito mode deactivated", true);
                if (sender.isInVehicle) API.setEntityTransparency(sender.vehicle, 255);
            }
            user.IncognitoMode = !user.IncognitoMode;
        }

        [Command("spawnobj")]
        public void SpawnObject(Client player, int obj = -1)
        {
            Player user = Player.PlayerData[player];
            if (user.MasterAccount.AdminLevel > 4)
            {
                API.sendNativeToAllPlayers(Hash.PLACE_OBJECT_ON_GROUND_PROPERLY, API.createObject(obj, player.position.Add(new Vector3(1, 1, 0)), new Vector3(0, 0, 0)));
            }
        }

        [Command("createfire")]
        public void CreateTestFire(Client player, bool isGas)
        {
            API.sendNativeToAllPlayers(Hash.START_SCRIPT_FIRE, player.position.X + 1, player.position.Y + 1, player.position.Z, 25, isGas);

        }

        [Command("testanim", GreedyArg = true)]
        public void TestAnim(Client player, string animlib = "", string animdist = "", string type = "")
        {

            Player p = Player.PlayerData[player];
            p.CanAttack = false;
            switch (type)
            {
                case "upperbodymove":
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), animlib, animdist);
                    break;
                case "upperbody":
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), animlib, animdist);
                    break;
                case "looped":
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop), animlib, animdist);
                    break;
                case "loopedmove":
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), animlib, animdist);
                    break;
                case "once":
                    API.playPlayerAnimation(player, 0, animlib, animdist);
                    break;
                case "oncepause":
                    API.playPlayerAnimation(player, (int)(AnimationFlags.StopOnLastFrame), animlib, animdist);
                    break;
                default:
                    API.stopPlayerAnimation(player);
                    p.CanAttack = true;
                    break;
            }
        }

        [Command("testmov")]
        public void MoveTopls(Client player, float x, float y, float z)
        {
            Wildlife.ActiveWildlife[0].MoveTo(player.position);
        }


        [Command("gotowildlife")]
        public void GotoWildlife(Client player)
        {
            player.position = API.getEntityPosition(Wildlife.ActiveWildlife[0].Ped);
        }

        [Command("unfreezewildlife")]
        public void unfreezeWildlife(Client player)
        {
            API.setEntityPositionFrozen(Wildlife.ActiveWildlife[0].Ped, false);
        }

        [Command("testscenario")]
        public void TestScenario(Client player, string scen)
        {
            if(scen == "none") { API.stopPlayerAnimation(player); return; }
            API.playPlayerScenario(player, scen);
        }

        [Command("throwtest")]
        public void ThrowWeapon(Client client, int force)
        {
            Player player = Player.PlayerData[client];
            Console.WriteLine(client.name);
            if (client.currentWeapon != WeaponHash.Unarmed)
            {
                string weaponName = NameToHash.Weapons.Keys.First(k => NameToHash.Weapons[k] == player.Client.currentWeapon);
                Weapon weap = player.Weapons.Single(w => w.Model.GetHashCode() == player.Client.currentWeapon.GetHashCode());

                Vector3 pos = Player.GetPositionInFrontOfPlayer(player.Client, 1f, 0.5f);
                Vector3 rot = new Vector3(90, 0, client.rotation.Z);

                weap.DroppedAt = Server.Date;
                weap.DroppedPos = pos;
                weap.DroppedRot = rot;
                weap.DroppedDimension = player.Client.dimension;
                weap.CurrentOwnerId = -1;
                weap.DroppedObj = API.createObject(NameToHash.WeaponObjects[weaponName], pos, rot, weap.DroppedDimension);
                weap.ThrowPhysics(client, force, -1);

                Weapon.DroppedWeapons.Add(weap);
                API.removePlayerWeapon(player.Client, weap.Model);
                player.Weapons.Remove(weap);
                WeaponRepository.UpdateAsync(weap);
            }
            else API.SendErrorNotification(player.Client, "You can only drop the weapon you are holding.");
        }

        [Command("testscream")]
        public void TestScream(Client sender)
        {
            API.sendNativeToAllPlayers(0x40CF0D12D142A9E8, sender);
        }


        [Command("testvehiclespeed")]
        public void TestVehSpeed(Client sender, float value)
        {
            if(sender.isInVehicle)
            {
                API.sendNativeToAllPlayers(Hash._SET_VEHICLE_ENGINE_POWER_MULTIPLIER, sender.vehicle, value);
            }
        }

        [Command("testweather")]
        public async Task WeatherTestAsync(Client sender)
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync("http://api.wunderground.com/api/41f62bac4c7f23ef/hourly/q/CA/San_Diego.json");

                dynamic weatherData = JsonConvert.DeserializeObject(json);

                Console.WriteLine(weatherData.hourly_forecast[0].temp.metric);

                // Now parse with JSON.Net
            }
        }

        [Command("changeskin")]
        public void ChangeSkinPedSkin(Client sender, int hash)
        {
            API.deleteEntity(ServerInit.SkinImagePed);
            ServerInit.SkinImagePed = API.createPed((PedHash)hash, new Vector3(-697.0174, 5803.262, 17.33096), 90);
        }

        [Command("testcamshake")]
        public void TestWeaponShake(Client player, float amp)
        {
            API.triggerClientEvent(player, "test-weapon-shake", amp);
            API.sendNativeToPlayer(player, Hash.SET_CAM_AFFECTS_AIMING, player, true);
        }
        [Command("injury")]
        public void BrightnessTest(Client player, int duration, int pause)
        {
            Player.PlayerData[player].IsBlurred = true;
            Player.PlayerData[player].BeginInjuryBlur(duration, pause, 1000);
        }

        [Command("stopinjury")]
        public void Injry(Client player)
        {
            PlayerManager.GetDebugReading = !PlayerManager.GetDebugReading;
        }

        [Command("testdecal")]
        public void PlaceDecal(Client player, int decalType,float width, float height)
        {
            API.sendNativeToAllPlayers(Hash.ADD_DECAL, decalType, player.position.X, player.position.Y, player.position.Z - 0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, width, height, 0.0f, 0.0f, 0.0f, 1.0f, 240000000f, true, true, true);
        }

        [Command("placetree")]
        public void GetTreePosition(Client player)
        {
            File.AppendAllText("trees.txt",
                string.Format("new Vector3({0}, {1}, {2}),\r\n", player.position.X, player.position.Y,
                    player.position.Z));
        }
    }
}
