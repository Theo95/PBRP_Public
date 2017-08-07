using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.IO;

namespace PBRP
{
    public class ATMCommands : Script
    {
        //[Command("atm")]
        //public void AccessATM(Client player, float x, float y, float z)
        //{

        //    foreach (Vector3 atm in ATMManager.PlayerPositionAtATM.Keys)
        //    {
        //        if (player.position.DistanceTo(atm) < 3.0)
        //        {
        //            List<Vector3> offsets = ATMManager.PlayerPositionAtATM[atm];
        //            player.position = atm;
        //            player.rotation = offsets[0];

        //            API.triggerClientEvent(player, "onExecuteATM", offsets[1], offsets[2]);
        //            return;
        //        }
        //    }
        //    // 0.8508 0.999864215     -0.2058 -0.202

        //    //player.position = new Vector3(154.950, 6641.938, 31.62891);
        //    //player.position = new Vector3(172.05, 6636.83, 31.634);
        //    //player.rotation = new Vector3(0, 0, -49.62966);
        //}

        [Command("nudge")]
        public void NudgePlayer(Client player, string dir, float dist)
        {
            Player p = Player.PlayerData[player];
            Vector3 camPos;

            switch(dir)
            {
                case "left":
                    camPos = Player.GetPositionInFrontOfPlayer(player, p.ATMEditDistance, p.ATMEditHeight);
                    camPos = Player.GetPositionLeftOfPlayer(camPos, player.rotation.Z, dist);
                    p.ATMCamPos = camPos;
                    p.ATMCamOffset = Player.GetPositionInFrontOfPoint(camPos, player.rotation.Z, 0.4f);
                    p.ATMCamOffset.Z = p.ATMCamPos.Z - 0.087f;
                    API.triggerClientEvent(player, "onExecuteATM", camPos, p.ATMCamOffset);
                    break;
                case "right":
                    camPos = Player.GetPositionInFrontOfPlayer(player, p.ATMEditDistance, p.ATMEditHeight);
                    camPos = Player.GetPositionRightOfPlayer(camPos, player.rotation.Z, dist);
                    p.ATMCamPos = camPos;
                    p.ATMCamOffset = Player.GetPositionInFrontOfPoint(camPos, player.rotation.Z, 0.4f);
                    p.ATMCamOffset.Z = p.ATMCamPos.Z - 0.087f;
                    API.triggerClientEvent(player, "onExecuteATM", camPos, p.ATMCamOffset);
                    break;
            }
        }

        [Command("rot")]
        public void NudgePlayer2(Client player, string dir, float amount)
        {
            Player p = Player.PlayerData[player];

            switch (dir)
            {
                case "left":
                    player.rotation.Z += amount;
                    Player.GetPositionInFrontOfPlayer(player, p.ATMEditDistance, p.ATMEditHeight);
                    p.ATMCamOffset = Player.GetPositionInFrontOfPoint(p.ATMCamPos, player.rotation.Z, 0.4f);
                    p.ATMCamOffset.Z = p.ATMCamPos.Z - 0.087f;
                    API.triggerClientEvent(player, "onExecuteATM", p.ATMCamPos, p.ATMCamOffset);
                    break;
                case "right":
                    player.rotation.Z -= amount;
                    p.ATMCamOffset = Player.GetPositionInFrontOfPoint(p.ATMCamPos, player.rotation.Z, 0.4f);
                    p.ATMCamOffset.Z = p.ATMCamPos.Z - 0.087f;
                    API.triggerClientEvent(player, "onExecuteATM", p.ATMCamPos, p.ATMCamOffset);
                    break;
            }
        }

        [Command("atmtest")]
        public void PositionAtmScreen(Client player, float dist, float height)
        {
            //0.61 0.2
            // 
            Player p = Player.PlayerData[player];

            p.ATMEditDistance = dist;
            p.ATMEditHeight = height;
            // API.triggerClientEvent(player, "onExecuteATM", new Vector3(155.8008, 6642.84, 31.78489), new Vector3(156.0066, 6643.042, 31.69789));
            Vector3 camPos = Player.GetPositionInFrontOfPlayer(player, dist, height);
            //Vector3 camPos = new Vector3(player.position.X - 0.8508, player.position.Y + 0.999864215, player.position.Z + 0.15598);
            p.ATMCamPos = camPos;
            p.ATMCamOffset = Player.GetPositionInFrontOfPoint(camPos, player.rotation.Z, 0.4f);
            p.ATMCamOffset.Z = camPos.Z - 0.087f;
            API.triggerClientEvent(player, "onExecuteATM", camPos, p.ATMCamOffset);
        }

        [Command("atmexport", GreedyArg = true)]
        public void ATMPosExport(Client player, string atmName)
        {
            Player p = Player.PlayerData[player];

            Console.WriteLine(player.position);
            Console.WriteLine(player.rotation);
            Console.WriteLine(p.ATMCamPos);
            Console.WriteLine(p.ATMCamOffset);
            File.AppendAllText("atmexport.txt", String.Format("({0}, {1}, {2}), (0, 0, {3}), ({4}, {5}, {6}) ({7}, {8}, {9}) //{10}",
                player.position.X, player.position.Y, player.position.Z, player.rotation.Z, p.ATMCamPos.X, p.ATMCamPos.Y, p.ATMCamPos.Z, p.ATMCamOffset.X, p.ATMCamOffset.Y, p.ATMCamOffset.Z, atmName));

            API.SendInfoNotification(player, "New ATM Location successfully logged");
        }

    }
}
