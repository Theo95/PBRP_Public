using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.Constant;

namespace PBRP
{
    public class PlayerManager : Script
    {
        public static bool GetDebugReading = false;
        private static long _secTick;
        public PlayerManager()
        {
            API.onChatMessage += OnChatMessage;
            ServerInit.OnSecond += OnSecond;
            Player.OnPlayerSecond += OnPlayerSecond;
            API.onClientEventTrigger += OnClientTriggerEvent;
            API.onPlayerWeaponAmmoChange += API_onPlayerWeaponAmmoChange;
            API.onPlayerWeaponSwitch += API_onPlayerWeaponSwitch;
        }

        private void API_onPlayerWeaponSwitch(Client player, WeaponHash oldValue)
        {
            //Do the anti-cheat

        }

        private static void API_onPlayerWeaponAmmoChange(Client player, WeaponHash weapon, int oldValue)
        {
            var p = Player.PlayerData[player];
            var weapons = p.Weapons;
            foreach (var weap in weapons)
            {
                var ammo = API.shared.getPlayerWeaponAmmo(player, weap.Model);
                if (weap.Ammo != ammo)
                {
                    weap.Ammo = ammo;
                    API.shared.triggerClientEvent(player, "validatePlayerAimingAt");
                }
            }
        }

        private void OnPlayerSecond(Player player)
        {
            player.Ping = API.getPlayerPing(player.Client);
        }

        private static void OnSecond()
        {
            _secTick++;
            if (_secTick % 10 != 0) return;
            var playerData = Player.PlayerData.Values.ToList();
            foreach (var p in playerData)
            {
                Player.OnSecond(p);
            }
        }

        private void OnClientTriggerEvent(Client sender, string eventName, params object[] arguments)
        {
            Console.WriteLine("[DEBUG:] Event Name: {0} Player: {1} No of arguments: {2}", eventName, sender.name, arguments.Count());
            switch (eventName)
            {
                case "ToggleCursorLock":
                    sender.ToggleCursorLock((bool)arguments[0]);
                    break;
                case "PlayerALTF4":
                    API.kickPlayer(sender, "Leaving server");
                    break;
                case "isPlayerAimingAtPlayer":
                    Console.WriteLine(arguments[0]);
                    if (arguments[0] is int) return;
                    try
                    {
                        if (Convert.ToInt32(arguments[0]) == -1) return;
                    }
                    catch {

                        if (API.getEntityType((NetHandle)arguments[0]) == EntityType.Player)
                        {
                            var attacker = API.getPlayerFromHandle((NetHandle)arguments[0]);
                            var player = Player.PlayerData[sender];
                            try
                            {
                                if (arguments[1] is bool)
                                {
                                    player.IsClimbing = (bool)arguments[1];
                                    player.IsVaulting = (bool)arguments[2];
                                }
                            }
                            catch { }


                            Weapon.ApplyDamageToPlayer(Player.PlayerData[attacker], player);

                       
                            //Weapon.IncreaseWeaponSkill(player, ShotType.HitPlayer);
                        }
                    }
                    break;
                case "PlayerPOVCameraChange":
                    Player.PlayerData[sender].IsInFirstPerson = (bool)arguments[0];
                    break;
                case "PlayerJustAttacked":
                    if (sender.currentWeapon == WeaponHash.StunGun) { API.triggerClientEvent(sender, "validatePlayerAimingAt"); return; }
                    if (API.getPlayerFromHandle((NetHandle)arguments[0]) == sender) return;
                    //if (!API.IsWeaponClass(sender.currentWeapon, WeaponClass.Melee) && !API.IsWeaponClass(sender.currentWeapon, WeaponClass.Blade) 
                    //    && !API.IsWeaponClass(sender.currentWeapon, WeaponClass.Fists))
                    //Weapon.IncreaseWeaponSkill(Player.PlayerData[API.getPlayerFromHandle((NetHandle)arguments[0])], ShotType.Missed);

                    Weapon.ApplyDamageToPlayer(Player.PlayerData[sender], Player.PlayerData[API.getPlayerFromHandle((NetHandle)arguments[0])]);
                    break;
                case "PlayerHasFallen":
                    Player.PlayerData[sender].ApplyDamage(DamageType.Fall, (int)arguments[0]);
                    break;
                case "PlayerAttackedInVehicle":
                    break;
                case "PlayerAimingBlur":
                    sender.ToBlurred(Convert.ToSingle(arguments[0]));
                    break;
                case "PlayerAimingStopBlur":
                    sender.FromBlurred(Convert.ToSingle(arguments[0]));
                    break;
                case "OnPlayerAim":
                    Player.PlayerData[sender].IsAiming = (bool)arguments[0];
                    break;
                case "ChatBubbleShow":
                {
                    var p = Player.PlayerData[sender];
                    if (p.ChatIndicatorLabel == null) return;
                    p.ChatIndicatorLabel.color = new Color(255, 255, 255, 255);
                    p.ChatIndicatorLabel.dimension = sender.dimension;
                }
                    break;
                case "ChatBubbleHide":
                {
                    Player p = Player.PlayerData[sender];
                    if (p.ChatIndicatorLabel == null) return;
                    p.ChatIndicatorLabel.color = new Color(0, 0, 0, 0);
                    p.ChatIndicatorLabel.dimension = sender.dimension;
                }
                    break;
            }
        }

        private void OnChatMessage(Client sender, string message, CancelEventArgs cancel)
        {
            var p = Player.PlayerData[sender];
            GeneralCommands.SendCloseMessage(sender, 15.0f, "~#ffffff~", API.getPlayerName(sender) + " says: " + message);
            if (p.PrimaryPhone?.InCallWith != null)
            {
                if (p.PrimaryPhone.CallConnected)
                {
                    if (!p.PrimaryPhone.MicMuted)
                    {
                        if (p.PrimaryPhone.Speakerphone)
                            API.SendCloseMessage(p.PrimaryPhone.InCallWith.Client, 15.0f,
                                string.Format("[speakerphone]: {0}", message));
                        else
                            API.sendChatMessageToPlayer(p.PrimaryPhone.InCallWith.Client,
                                string.Format("[phone]: {0}", message));
                    }
                }
            }
            cancel.Cancel = true; // Because we do this in PDArmouryManager and it's the last event
        }
    }
}
