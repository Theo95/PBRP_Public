using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP
{
    public static class APIExtensions
    {
        public static void SendCloseMessage(this ServerAPI api, Client player, float radius, string header, string msg)
        {
            List<Client> nearPlayers = api.getPlayersInRadiusOfPosition(radius, player.position);
            foreach (Client target in nearPlayers)
            {
                if (target.dimension == player.dimension)
                    api.sendChatMessageToPlayer(target, header, msg);
            }
        }

        public static void SendCloseMessage(this ServerAPI api, Client player, float radius, string msg)
        {
            List<Client> nearPlayers = api.getPlayersInRadiusOfPosition(radius, player.position);
            foreach (Client target in nearPlayers)
            {
                if (target.dimension == player.dimension)
                    api.sendChatMessageToPlayer(target, "~#ffffff~", msg);
            }
            api.sendChatMessageToPlayer(player, "~#ffffff~", msg);
        }
        public static void SendCloseMessage(this ServerAPI api, Entity player, float radius, string msg)
        {
            List<Client> nearPlayers = API.shared.getPlayersInRadiusOfPosition(radius, player.position);
            foreach (Client target in nearPlayers)
            {
                if (target.dimension == player.dimension)
                    api.sendChatMessageToPlayer(target, "~#ffffff~", msg);
            }
        }

        public static void SendCloseMessage(this ServerAPI api, Entity player, float radius, string header, string msg)
        {
            List<Client> nearPlayers = API.shared.getPlayersInRadiusOfPosition(radius, player.position);
            foreach (Client target in nearPlayers)
            {
                if(target.dimension == player.dimension)
                   api.sendChatMessageToPlayer(target, header, msg);
            }
        }


        public static void SendMessageToAllFactionMemebers(this ServerAPI api, Faction faction, string message)
        {
            foreach (Player p in Player.PlayerData.Values)
            {
                if (p.Faction == faction)
                {
                    api.sendChatMessageToPlayer(p.Client, message);
                }
            }
        }

        public static void setEntityProofs(this ServerAPI api, NetHandle entity, bool bulletProof, bool fireProof, bool explosionProof, bool collisionProof, bool meleeProof)
        {
            api.sendNativeToAllPlayers(Hash.SET_ENTITY_PROOFS, entity, bulletProof, fireProof, explosionProof, collisionProof, meleeProof, true, 1, true);
        }

        public static void SetPedIntoVehicle(this ServerAPI api, Ped ped, GrandTheftMultiplayer.Server.Elements.Vehicle veh, int seat)
        {
           api.sendNativeToAllPlayers(Hash.TASK_WARP_PED_INTO_VEHICLE, ped, veh, seat);
        }

        public static NetHandle CreatePedInVehicle(this ServerAPI api, GrandTheftMultiplayer.Server.Elements.Vehicle veh, int type, PedHash model, int seat)
        {
            return api.fetchNativeFromPlayer<NetHandle>(Player.IDs.Where(p => p != null).First(), Hash.CREATE_PED_INSIDE_VEHICLE, veh, type, model, seat, false, true);
        }

        public static void SendErrorNotification(this ServerAPI api, Client sender, string message, int time = 3)
        {
            api.triggerClientEvent(sender, "errorNotification", message, time);
        }

        public static void SendAdminNotification(this ServerAPI api, Client sender, string message, int time = 3)
        {
            api.triggerClientEvent(sender, "adminNotification", message, time);
        }

        public static void SendSuccessNotification(this ServerAPI api, Client sender, string message, int time = 3)
        {
            api.triggerClientEvent(sender, "successNotification", message, time);
        }

        public static void SendInfoNotification(this ServerAPI api, Client sender, string message, int time = 3)
        {
            api.triggerClientEvent(sender, "infoNotification", message, time);
        }
        public static void SendWarningNotification(this ServerAPI api, Client sender, string message, int time = 3)
        {
            api.triggerClientEvent(sender, "warningNotification", message, time);
        }

        public static void ShowInputPrompt(this ServerAPI api, Client sender, string returnEvent, string title, string message, string customYes = "", string customNo = "", bool keepMouseEnabled = false)
        {
            api.triggerClientEvent(sender, "userPrompted", true);
            api.triggerClientEvent(sender, "confirmationUIInput", returnEvent, title, message, customYes, customNo, keepMouseEnabled);
        }

        public static void ShowPopupPrompt(this ServerAPI api, Client sender, string returnEvent, string title, string message, string customYes = "", string customNo = "", bool keepMouseEnabled = false)
        {
            api.triggerClientEvent(sender, "userPrompted", true);
            api.triggerClientEvent(sender, "confirmationUIYesNo", returnEvent, title, message, customYes, customNo, keepMouseEnabled);
        }

        public static void ShowPopupMessage(this ServerAPI api, Client sender, string title, string message, bool keepMouseEnabled = false)
        {
            api.triggerClientEvent(sender, "confirmationUIMessage", "null", title, message, keepMouseEnabled);
        }

        public static bool IsAiming(this Client player)
        {
            return API.shared.fetchNativeFromPlayer<bool>(player, Hash.IS_AIM_CAM_ACTIVE, player);
        }

        public static bool IsInFirstPerson(this ServerAPI api, Client player)
        {
            if (api.fetchNativeFromPlayer<int>(player, (ulong)0xEE778F8C7E1142E2, API.shared.fetchNativeFromPlayer<uint>(player, (ulong)0x19CAFA3C87F7C2FF, player)) == 4) return true;
            else return false;
        }
    }
}
