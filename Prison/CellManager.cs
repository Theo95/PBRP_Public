using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.Managers;

namespace PBRP
{
    public class CellManager : Script {
        public CellManager() {
            API.onResourceStart += onResourceStart;
            API.onEntityEnterColShape += onEntityEnterColShape;
            API.onUpdate += onEscortTick;
        }

        private static void onResourceStart() {
            CellGuard = API.shared.createPed(PedHash.Sheriff01SMY, new Vector3(459.7444, -989.1557, 24.91488), -90, 22);
            CellGuard.invincible = true;
            CellEntrance = API.shared.createCylinderColShape(new Vector3(465, -989.1557, 24.91488), 4.0f, 2.5f);
        }

        // --- Properties

        public static Ped CellGuard = null;
        public static string CellGuardName = "Steven Townsend";
        public static ColShape CellEntrance = null;
        public static bool isCurrentlyEscorting = false;
        public static Player escortee = null;
        public static Dictionary<long, Func<Player, bool>> CellGuardEscortSteps = new Dictionary<long, Func<Player, bool>>();

        public static string[] LetOutMessages = {
            "I hope you're feeling rehabilitated. Let's go.",
            "You've learned your lesson. I hope. Time to go",
            "I bet I'll be seeing you again this time next week!"
        };

        public static string[] GoodbyeMessages = {
            "Have a good one.",
            "See you later!",
            "Have a nice day",
        };

        // --- Methods

        public static string GetRandomLetOutMessage() {
            int maxNum = LetOutMessages.Count();
            int rng = new Random().Next(maxNum);

            return LetOutMessages.ElementAt(rng);
        }

        public static string GetRandomGoodbyeMessage()
        {
            int maxNum = GoodbyeMessages.Count();
            int rng = new Random().Next(maxNum);

            return GoodbyeMessages.ElementAt(rng);
        }

        private static void onEntityEnterColShape(ColShape colshape, NetHandle entity) {
            if ((colshape != CellEntrance) || (API.shared.getEntityType(entity) != EntityType.Player)) { return; }
            if (isCurrentlyEscorting) { return; }

            Client playerC = API.shared.getPlayerFromHandle(entity);
            if (playerC == null) { return; }

            Player player;
            Player.PlayerData.TryGetValue(playerC, out player);
            if (player == null) { return; }

            if (FactionManager.IsPlayerAnOfficer(player)) { return; }
            API.shared.SendCloseMessage(CellGuard, 15f, CellGuardName + " says: You're not supposed to be here, leave. Now.");
        }


        public static void StartCellExitSequenceForPlayer(Client player) { StartCellExitSequenceForPlayer(Player.PlayerData[player]); }
        public static void StartCellExitSequenceForPlayer(Player player)
        {
            API.shared.setEntityPosition(player.Client.handle, new Vector3(464, -991.75, 25.0649));
            API.shared.setEntityRotation(player.Client.handle, new Vector3(0, 0, 0));

            if (isCurrentlyEscorting == true) { return; }
            isCurrentlyEscorting = true;
            escortee = player;

            Vector3 firstPosition = new Vector3(462.6322, -990.9704, 24.91486);

            CellGuard.MoveTo(firstPosition.X, firstPosition.Y, firstPosition.Z, 2, 6);
            CellGuard.LookAtEntity(player.Client.handle, 120);
            API.shared.SendCloseMessage(CellGuard, 15f, CellGuardName + " says: " + GetRandomLetOutMessage());

            AddStepToCellSequence(6, StepTwoExitSequenceForPlayer);
        }

        public static bool StepTwoExitSequenceForPlayer(Player player)
        {
            Vector3 secondPosition = new Vector3(446.715, -987.9465, 30.6896);
            Vector3 secondRotation = new Vector3(0, 0, 52.5);

            CellGuard.MoveTo(secondPosition.X, secondPosition.Y, secondPosition.Z, 2);

            AddStepToCellSequence(24, StepThreeExitSequenceForPlayer);
            return true;
        }

        public static bool StepThreeExitSequenceForPlayer(Player player)
        {
            Vector3 secondPosition = new Vector3(446.715, -987.9465, 30.6896);
            if (CellGuard.position.DistanceTo(secondPosition) > 15.0f)
            {
                Console.WriteLine(CellGuard.position.DistanceTo(secondPosition));
                AddStepToCellSequence(3, StepTwoExitSequenceForPlayer);
                return true;
            }

            CellGuard.LookAtEntity(player.Client.handle);
            AddStepToCellSequence(1, StepFourExitSequenceForPlayer);
            return true;
        }

        public static bool StepFourExitSequenceForPlayer(Player player)
        {
            Vector3 initialPosition = new Vector3(459.7444, -989.1557, 24.91488);
            if (player.Client.position.DistanceTo(new Vector3(446.715, -987.9465, 30.6896)) <= 15f)
            {
                API.shared.SendCloseMessage(player.Client, 15f, CellGuardName + " says: " + GetRandomGoodbyeMessage());
            }

            CellGuard.MoveTo(initialPosition.X, initialPosition.Y, initialPosition.Z, 2);
            AddStepToCellSequence(28, StepFiveExitSequenceForPlayer);
            return true;
        }

        public static bool StepFiveExitSequenceForPlayer(Player player)
        {
            Vector3 initialPosition = new Vector3(459.775, -989.1557, 24.91488);
            Vector3 initialRotation = new Vector3(0, 0, -90);

            if (CellGuard.position.DistanceTo(initialPosition) > 15.0f)
            {
                AddStepToCellSequence(3, StepFourExitSequenceForPlayer);
                return true;
            }

            CellGuard.moveRotation(initialRotation, 2);
            isCurrentlyEscorting = false;
            escortee = null;
            return true;
        }

        public static void onEscortTick()
        {
            if (isCurrentlyEscorting == true)
            {
                if (CellGuardEscortSteps.Count == 0) { return; }

                if (CellGuardEscortSteps.First().Key <= API.shared.TickCount)
                {
                    CallCellStep(CellGuardEscortSteps.First().Value);
                }
            }
        }

        public static void AddStepToCellSequence(int delay, Func<Player, bool> callback)
        {
            if (CellGuardEscortSteps.Count != 0) { return; }
            CellGuardEscortSteps.Add(API.shared.TickCount + (delay * 1000), callback);
        }

        public static void CallCellStep(Func<Player, bool> callback)
        {
            RemoveStepFromCellSequence();
            callback(escortee);
        }

        public static void RemoveStepFromCellSequence()
        {
            CellGuardEscortSteps.Remove(CellGuardEscortSteps.ElementAt(0).Key);
        }
    }
}
