using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;

namespace PBRP
{
    public class EmergencyCallHandler:Script
    {
        public EmergencyCallHandler()
        {
            API.onClientEventTrigger += OnClientEvent;
            API.onChatMessage += OnChatMessage;
        }

        public void OnClientEvent(Client player, String eventName, params object[] args)
        {
            if (eventName == "load_call_types")
            {
                sendCallTypesToClient(player);
            }
            else if(eventName == "call_type_selected")
            {
                callTypeSelected((CallType)args[0], (String)args[1], player);
            }
        }

        private void OnChatMessage(Client sender, string message, CancelEventArgs cancel)
        {
            Player player = Player.PlayerData[sender];
            if (player.EmsCallStatus == 1)
            {
                player.PlayerEmsCallInProgress.CallLocationX = player.Client.position.X;
                player.PlayerEmsCallInProgress.CallLocationY = player.Client.position.Y;
                player.PlayerEmsCallInProgress.CallLocationZ = player.Client.position.Z;
                if (message.ToLower() == "police")
                {
                    player.PlayerEmsCallInProgress.RefferedTo = EmsType.Police;
                    API.sendChatMessageToPlayer(sender, "Dispatcher", "Got it, and what's the situation?");
                    API.triggerClientEvent(sender, "load_calltype_menu");
                    sendCallTypesToClient(sender);
                    player.EmsCallStatus = 2;
                }
                else if (message.ToLower() == "fire" || message.ToLower() == "ambulance")
                {
                    player.PlayerEmsCallInProgress.RefferedTo = EmsType.Fire;

                    API.sendChatMessageToPlayer(sender, "Dispatcher", "Got it, and what's the situation?");
                    API.triggerClientEvent(sender, "load_calltype_menu");
                    sendCallTypesToClient(sender);
                    player.EmsCallStatus = 2;
                }
                else { API.sendChatMessageToPlayer(sender, "Dispatcher", "Uh, sorry, I didn't catch that. Which service do you require?"); }
            }
            else if (player.EmsCallStatus == 2)
            {
                player.PlayerEmsCallInProgress.CallDescription = message;
                player.EmsCallStatus = 3;
                API.sendChatMessageToPlayer(sender, "Dispatcher", "Okay, we'll send a unit right away - can I take your name?");
            }
            else if (player.EmsCallStatus == 3)
            {
                player.PlayerEmsCallInProgress.CallerNameGiven = message;
                API.sendChatMessageToPlayer(sender, "Dispatcher", "Your call has been logged, a unit should be with you shortly");
                player.EmsCallStatus = 0;
                callCompleted(player.PlayerEmsCallInProgress);
            }
        }

        public static void EmsCalled(Client caller, String number)
        {
            Player player = Player.PlayerData[caller];
            API.shared.sendChatMessageToPlayer(caller, "Dispatcher:", "911, which service do you require?");
            player.EmsCallStatus = 1;
            player.PlayerEmsCallInProgress = new EmsCall();
            player.PlayerEmsCallInProgress.PhoneNumber = number;
            player.PlayerEmsCallInProgress.CallTime = DateTime.Now;
            player.PlayerEmsCallInProgress.CallerId = player.Id;
        }

        public void sendCallTypesToClient(Client player)
        {

            var values = Enum.GetValues(typeof(CallType)).Cast<CallType>().ToList();
            var json = JsonConvert.SerializeObject(values.Select(ty => ty.ToString()));
            Console.Write(json);
            API.triggerClientEvent(player, "add_call_types", json);

        }

        public void callTypeSelected(CallType type, String street, Client player)
        {
            Player p = Player.PlayerData[player];
            p.PlayerEmsCallInProgress.CallType = type;
            p.PlayerEmsCallInProgress.CallLocationStreetName = street;
            p.EmsCallStatus = 2;
            API.sendChatMessageToPlayer(player, "Dispatcher", "Can you describe the situation?");
        }

        public void callCompleted(EmsCall call)
        {
            call.CallStatus = CallStatus.Active;
            EmsCallRepository.AddEmsCall(call);
        }


    }
}
