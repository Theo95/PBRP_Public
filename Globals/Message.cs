using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;

namespace PBRP
{

    public class Message : Script
    {
        public static void Error(Client player, string message)
        {
            API.shared.sendChatMessageToPlayer(player, "~#ff0000~", String.Format("ERROR: ~w~{0}", message));
        }

        public static void Warning(Client player, string message)
        {
            API.shared.sendChatMessageToPlayer(player, "~#ffffff~", String.Format("{0}", message));
        }

        public static void Info(Client player, string message)
        {
            API.shared.sendChatMessageToPlayer(player, "~#ffffff~", String.Format("{0}", message));
        }

        public static void PlayerNotConnected(Client player)
        {
            API.shared.SendErrorNotification(player, "The player you specified is not connected.");
        }

        public static void NotAuthorised(Client player)
        {
            API.shared.SendErrorNotification(player, "You are not authorised to use this command.");
        }

        public static void AdminMessage(Client player, string message)
        {
            API.shared.SendAdminNotification(player, message);
        }

        public static void Syntax(Client player, string message)
        {
            API.shared.sendChatMessageToPlayer(player, "~#666666~", message);
        }

        public static void Radio(Client player, string message)
        {
            API.shared.sendChatMessageToPlayer(player, "~#826E41~", message);
        }
    }
}
