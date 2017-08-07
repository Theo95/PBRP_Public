using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PBRP
{
    public static class PedExtensionMethods
    { 
        public static void EnterVehicle(this Ped ped, GrandTheftMultiplayer.Server.Elements.Vehicle veh, int seat, int duration = 20, int flag = 1)
        {
            Task.Run(async () =>
            {
                for (int i = 0; i < duration; i++)
                {
                    await Task.Delay(1000);
                    API.shared.setEntityPositionFrozen(ped, false);
                    API.shared.sendNativeToAllPlayers(Hash.TASK_ENTER_VEHICLE, ped, veh, -1, seat, 1.0, flag, 0);
                }
            });
        }

        public static void WarpIntoVehicle(this Ped ped, GrandTheftMultiplayer.Server.Elements.Vehicle veh, int seat)
        {
            Task.Run(async () =>
            {
                for (int i = 0; i < 3; i++)
                {
                    await Task.Delay(1000);
                    API.shared.setEntityPositionFrozen(ped, false);
                    API.shared.sendNativeToAllPlayers(Hash.TASK_WARP_PED_INTO_VEHICLE, ped, veh, seat);
                }
            });
        }

        public static void WarpIntoVehicleForPlayer(this Ped ped, Client player, GrandTheftMultiplayer.Server.Elements.Vehicle veh, int seat)
        {
            Task.Run(async () =>
            {
                for (int i = 0; i < 3; i++)
                {
                    await Task.Delay(1000);
                    API.shared.setEntityPositionFrozen(ped, false);
                    API.shared.sendNativeToPlayer(player, Hash.TASK_WARP_PED_INTO_VEHICLE, ped, veh, seat);
                }
            });
        }

        public static void ExitVehicle(this Ped ped)
        {
            Task.Run(async () =>
            {
                for (int i = 0; i < 6; i++)
                {
                    await Task.Delay(1000);
                    API.shared.setEntityPositionFrozen(ped, false);
                    API.shared.sendNativeToAllPlayers(Hash.TASK_LEAVE_ANY_VEHICLE, ped, 0, 0);
                }
            });
        }

        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="ped"></param>
        ///  <param name="x"></param>
        ///  <param name="y"></param>
        ///  <param name="z"></param>
        /// <param name="speed"></param>
        /// <param name="duration"></param>
        public static void MoveTo(this Ped ped, float x, float y, float z, int speed, int duration = 20)
        {
            Task.Run(async () =>
            {
                for(int i = 0; i < duration; i++)
                {
                    await Task.Delay(1000);
                    API.shared.setEntityPositionFrozen(ped, false);
                    API.shared.sendNativeToAllPlayers(Hash.TASK_GO_TO_COORD_ANY_MEANS, ped.handle, x, y, z, 1.0, 0, 0, 786603, 0xbf800000);
                }
            });
        }

        public static void LookAtEntity(this Ped ped, NetHandle entity, int duration = 20)
        {
            Task.Run(() =>
            {
                for (int i = 0; i < duration; i++)
                {
                    API.shared.setEntityPositionFrozen(ped, false);
                    API.shared.sendNativeToAllPlayers(Hash.TASK_TURN_PED_TO_FACE_ENTITY, ped.handle, entity, 2000);
                }
            });
        }

        public static void TurnToPosition(this Ped ped, Vector3 position, int duration = 20) {
            Task.Run(() => {
                for (int i = 0; i < duration; i++) {
                    API.shared.setEntityPositionFrozen(ped, false);
                    API.shared.sendNativeToAllPlayers(Hash.TASK_TURN_PED_TO_FACE_COORD, ped.handle, position, 2000);
                }
            });
        }

        public static void DriveTo(this Ped ped, GrandTheftMultiplayer.Server.Elements.Vehicle veh, float x, float y, float z, float speed)
        {
            Task.Run(async () =>
            {
                while (veh.position.DistanceTo(new Vector3(x, y, z)) > 5)
                {
                    await Task.Delay(1000);
                    API.shared.setEntityPositionFrozen(ped, false);
                    API.shared.sendNativeToAllPlayers(Hash.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE, ped.handle, veh, x, y, z, speed, 1 | 2 | 16 | 32 | 128 | 256 | 262144, 10f);
                }
            });
        }

        public static void ClimbOver(this Ped ped, GrandTheftMultiplayer.Server.Elements.Vehicle veh)
        {
            Task.Run(() =>
            {
                API.shared.sendNativeToAllPlayers(Hash.TASK_SHUFFLE_TO_NEXT_VEHICLE_SEAT, ped, veh);
            });
        }

        public static async Task<bool> IsInVehicle(this Ped ped, Client sender)
        {
            return await Task.Run(() => API.shared.fetchNativeFromPlayer<bool>(sender, Hash.IS_PED_IN_ANY_VEHICLE, ped, true));
        }

        public static void ClearTasks(this Ped ped)
        {
            API.shared.sendNativeToAllPlayers(Hash.CLEAR_PED_TASKS_IMMEDIATELY, ped);
        }
    }

    public static class PlayerExtensions
    {
        public static void EnterVehicle(this Client player, GrandTheftMultiplayer.Server.Elements.Vehicle veh, int seat)
        {
            Task.Run(async () =>
            {
                for (int i = 0; i < 4; i++)
                {
                    await Task.Delay(1000);
                    API.shared.setEntityPositionFrozen(player, false);
                    API.shared.sendNativeToPlayersInDimension(player.dimension, Hash.TASK_ENTER_VEHICLE, player, veh, -1, seat, 1.0, 1, 0);
                }
            });
        }

        public static void FadeOutIn(this Client player, int time, int wait)
        {
            Task.Run(async () =>
            {
                API.shared.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_OUT, time);
                await Task.Delay(wait);
                API.shared.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_IN, time);
            });
        }

        public static void FadeOut(this Client player, int time)
        {
            API.shared.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_OUT, time);
        }

        public static void FadeIn(this Client player, int time)
        {
            API.shared.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_IN, time);
        }

        public static double GetSpeed(this Client player)
        {
            Vector3 velocity = API.shared.getPlayerVelocity(player);
            return Math.Sqrt(
                velocity.X * velocity.X +
                velocity.Y * velocity.Y +
                velocity.Z * velocity.Z
            );
        }

        public static int Ped(this Client player)
        {   
            int id = API.shared.fetchNativeFromPlayer<int>(player, Hash.GET_PLAYER_PED, player);
            Console.WriteLine(id);
            return id;
        }

        public static void ClearChat(this Client player)
        {
            for(int i = 0; i < 10; i++)
            {
                API.shared.sendChatMessageToPlayer(player, " ");
            }
        }

        public static void ToggleCursorLock(this Client player, bool tog)
        {
            API.shared.triggerClientEvent(player, "ToggleCursorLock", tog);
        }

        public static Player GetClosestPlayer(this Client player, int distance = 20)
        {
            Player closestPlayer = null;
            Vector3 pPos = player.position;
            foreach(Player p in Player.PlayerData.Values)
            {
                if (p.Client == player) continue;
                if(closestPlayer == null || pPos.DistanceTo(p.Client.position) < pPos.DistanceTo(closestPlayer.Client.position))
                {
                    if (pPos.DistanceTo(p.Client.position) > distance) continue;
                    closestPlayer = p;
                }
            }
            return closestPlayer;
        }

        public static bool IsClimbing(this Client player)
        {
            return API.shared.fetchNativeFromPlayer<bool>(player, Hash.IS_PED_CLIMBING, player);
        }

        public static bool IsVaulting(this Client player)
        {
            return API.shared.fetchNativeFromPlayer<bool>(player, Hash.IS_PED_VAULTING, player);
        }

        public static void BeginInjuryBlur(this Player player, int duration, int pause, float fadeTime, bool playOnlyOnce = false)
        {
            Task.Run(async () =>
            {
                do
                {
                    API.shared.sendNativeToPlayer(player.Client, Hash._TRANSITION_TO_BLURRED, fadeTime);
                    await Task.Delay(duration);
                    API.shared.sendNativeToPlayer(player.Client, Hash._TRANSITION_FROM_BLURRED, fadeTime);
                    await Task.Delay(pause);
                }
                while (player.IsBlurred && !playOnlyOnce);
               
            });
        }

        public static void StopInjuryBlur(this Player player)
        {
            player.IsBlurred = false;
            API.shared.sendNativeToPlayer(player.Client, Hash._TRANSITION_TO_BLURRED, 0);
            API.shared.sendNativeToPlayer(player.Client, Hash._TRANSITION_FROM_BLURRED, 1000.0);
        }

        public static void ToBlurred(this Client player, float fadeTime)
        {
            API.shared.sendNativeToPlayer(player, Hash._TRANSITION_FROM_BLURRED, 0);
            API.shared.sendNativeToPlayer(player, Hash._TRANSITION_TO_BLURRED, fadeTime);
        }

        public static void FromBlurred(this Client player, float fadeTime)
        {
            API.shared.sendNativeToPlayer(player, Hash._TRANSITION_TO_BLURRED, 0);
            API.shared.sendNativeToPlayer(player, Hash._TRANSITION_FROM_BLURRED, fadeTime);
        }
    }

    public static class PhoneExtension
    {
        public static async void TurnOn(this Phone phone, Client sender)
        {
            await Task.Run(async () =>
            {
                API.shared.triggerClientEvent(sender, "initiatePhone");
                await Task.Delay(1000);
                API.shared.triggerClientEvent(sender, "PhoneBootScreen");
                await Task.Delay(2000);

                if (phone.BatteryLevel > 3)
                {
                    API.shared.triggerClientEvent(sender, "PhoneHomeScreen", phone.BatteryLevel, phone.InstalledSim, Phone.PhoneWallpapers[phone.WallpaperId], phone.PassActive);

                    List<PhoneApp> phoneApps = PhoneAppRepository.GetPhoneAppsByPhoneId(phone.Id);
                    API.shared.triggerClientEvent(sender, "LoadPhoneApps", phoneApps.Count, string.Join(",", phoneApps.Select(pa => pa.Position)), string.Join(",", phoneApps.Select(pa => PhoneApp.AppInfo[pa.AppId][0])),
                        string.Join(".", phoneApps.Select(pa => PhoneApp.AppInfo[pa.AppId][1])));

                    API.shared.triggerClientEvent(sender, "phoneUpdateClock", ServerInit.ServerHour, ServerInit.ServerMinute);
                }

                phone.PoweredOn = true;
            });  
        }

        public static async void Show(this Phone phone, Client sender)
        {
            await Task.Run(async () =>
            {
                API.shared.triggerClientEvent(sender, "initiatePhone");
                await Task.Delay(1000);
                if(phone.BatteryLevel > 3)
                {
                    API.shared.triggerClientEvent(sender, "PhoneHomeScreen", phone.BatteryLevel, phone.InstalledSim, Phone.PhoneWallpapers[phone.WallpaperId], phone.PassActive);

                    List<PhoneApp> phoneApps = PhoneAppRepository.GetPhoneAppsByPhoneId(phone.Id);
                    API.shared.triggerClientEvent(sender, "LoadPhoneApps", phoneApps.Count, string.Join(",", phoneApps.Select(pa => pa.Position)), string.Join(",", phoneApps.Select(pa => PhoneApp.AppInfo[pa.AppId][0])),
                        string.Join(".", phoneApps.Select(pa => PhoneApp.AppInfo[pa.AppId][1])));

                    API.shared.triggerClientEvent(sender, "phoneUpdateClock", ServerInit.ServerHour, ServerInit.ServerMinute);
                }
            });
        }

        public static void Home(this Phone phone, Client sender)
        {

            if (phone.BatteryLevel > 3)
            {
                API.shared.triggerClientEvent(sender, "ReloadHomeScreen", Phone.PhoneWallpapers[phone.WallpaperId]);

                List<PhoneApp> phoneApps = PhoneAppRepository.GetPhoneAppsByPhoneId(phone.Id);
                API.shared.triggerClientEvent(sender, "LoadPhoneApps", phoneApps.Count, string.Join(",", phoneApps.Select(pa => pa.Position)), string.Join(",", phoneApps.Select(pa => PhoneApp.AppInfo[pa.AppId][0])),
                    string.Join(".", phoneApps.Select(pa => PhoneApp.AppInfo[pa.AppId][1])));

                API.shared.triggerClientEvent(sender, "phoneUpdateClock", ServerInit.ServerHour, ServerInit.ServerMinute);
            }
        }

        public static void TurnOff(this Phone phone, Client sender)
        {
            API.shared.triggerClientEvent(sender, "InitiatePhoneOff");
            phone.PoweredOn = false;
        }

        public static void Hide(this Phone phone, Client sender)
        {
            API.shared.triggerClientEvent(sender, "hidePhoneUI");
        }

        public static bool IsPhone(this Inventory i)
        {
            return i.Type == InventoryType.SmartPhone1 || i.Type == InventoryType.SmartPhone2 || i.Type == InventoryType.SmartPhone3 || i.Type == InventoryType.SmartPhone4 ||
                   i.Type == InventoryType.SmartPhone5 || i.Type == InventoryType.BrickPhone1 || i.Type == InventoryType.BrickPhone2;
        }

        public static bool IsPhone(InventoryType i)
        {
            return i == InventoryType.SmartPhone1 || i == InventoryType.SmartPhone2 || i == InventoryType.SmartPhone3 || i == InventoryType.SmartPhone4 ||
                   i == InventoryType.SmartPhone5 || i == InventoryType.BrickPhone1 || i == InventoryType.BrickPhone2;
        }
    }

    public static class InventoryExtension
    {
        public static void ApplyPhysics(this Inventory inv)
        {
            API.shared.sendNativeToAllPlayers(Hash.FREEZE_ENTITY_POSITION, inv.DroppedObj, false);
            API.shared.sendNativeToAllPlayers(Hash.SET_ENTITY_DYNAMIC, inv.DroppedObj, true);
        }
    }

    public static class WeaponExtension
    {
        public static void ApplyPhysics(this Weapon inv)
        {
            API.shared.sendNativeToAllPlayers(Hash.SET_OBJECT_PHYSICS_PARAMS, inv.DroppedObj, 20f, 1.7f, 0f, 0f, 0f, 9.8f, 0f, 0f, 0f, 0f, 2f);
            API.shared.sendNativeToAllPlayers(Hash.SET_ACTIVATE_OBJECT_PHYSICS_AS_SOON_AS_IT_IS_UNFROZEN, inv.DroppedObj, true);
            API.shared.sendNativeToAllPlayers(Hash.FREEZE_ENTITY_POSITION, inv.DroppedObj, false);
            API.shared.sendNativeToAllPlayers(Hash.SET_ENTITY_DYNAMIC, inv.DroppedObj, true);
            API.shared.sendNativeToAllPlayers(Hash.APPLY_FORCE_TO_ENTITY_CENTER_OF_MASS, inv.DroppedObj, 4, inv.DroppedX, inv.DroppedY, inv.DroppedZ, true, false, false, false);
        }

        public static void ThrowPhysics(this Weapon inv, Client player, int rip, float height)
        {
            API.shared.sendNativeToAllPlayers(Hash.SET_OBJECT_PHYSICS_PARAMS, inv.DroppedObj, 20f, 1.7f, 0f, 0f, 0f, 9.8f, 0f, 0f, 0f, 0f, 2f);
            API.shared.sendNativeToAllPlayers(Hash.SET_ENTITY_DYNAMIC, inv.DroppedObj, true);
            API.shared.sendNativeToAllPlayers(Hash.FREEZE_ENTITY_POSITION, inv.DroppedObj, false);
            
            Task.Run(async() =>
            {
                int distance = rip;
                while (distance > 0)
                {
                    await Task.Delay(2);
                    //API.shared.sendNativeToAllPlayers(Hash.APPLY_FORCE_TO_ENTITY, inv.DroppedObj, 3, Convert.ToSingle(-(Math.Sin((inv.DroppedRot.Z * Math.PI) / 180) * 2f)), Convert.ToSingle((Math.Cos((inv.DroppedRot.Z * Math.PI) / 180) * 2f)), 0f, 0f, 0f, 0f, 0, true, false, true, false, false);
                    API.shared.sendNativeToAllPlayers(Hash.APPLY_FORCE_TO_ENTITY, inv.DroppedObj, 3, 0f, 0f, height, 0f, 0f, 0f, 1, true, false, true, false, false);
                    distance--;
                }
            });
            
        }
    }
}
