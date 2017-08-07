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
    public class NPC : Script {
        public NPC() {
            API.onUpdate += NPCTick;
        }

        public static List<NPC> NPCList = new List<NPC>();
        public static void NPCTick()
        {
            foreach (NPC t in NPCList)
            {
                t.sequenceTick();
            }
        }

        struct SequenceKey {
            public readonly long triggerTick;
            public readonly int delay;

            public SequenceKey(int del, long tick) {
                delay = del;
                triggerTick = tick;
            }
        }

        // --- Start NPC Class

        public NPC(PedHash pedHash, string nameStr, Vector3 startPosition, int facingAngle, int dimension) {
            model = pedHash;
            name = nameStr;

            pedHandle = API.createPed(pedHash, startPosition, facingAngle, dimension);
            ActionSequence = new Dictionary<SequenceKey, Action>();

            onFinishSequence += baseFinishSequence;
            onStartSequence += baseStartSequence;
            onSequenceStepComplete += baseSequenceStepComplete;

            onDestinationReached += baseDestinationReached;
            onDestinationFail += baseDestinationFail;

            API.onUpdate += sequenceTick;
            NPCList.Add(this);
        }

        public PedHash model;
        public Ped pedHandle;
        public string name { get; set; }
        public int dimension {
            get { return API.getEntityDimension(pedHandle); }
            set { API.setEntityDimension(pedHandle, value); }
        }
        public Vector3 position {
            get { return API.getEntityPosition(pedHandle); }
            set { API.setEntityPosition(pedHandle, value); }
        }

        // --- Events
        public ColShape destColShape = null;

        public API.EmptyEvent onFinishSequence;
        public API.EmptyEvent onStartSequence;
        public API.EmptyEvent onSequenceStepComplete;

        public ColShapeEvent onDestinationReached;
        public ColShapeEvent onDestinationFail;

        // --- Base event handlers
        public void baseFinishSequence() {}
        public void baseStartSequence() {}
        public void baseSequenceStepComplete() {}

        public void baseDestinationReached(ColShape shape, NetHandle entity) {
            if(shape != destColShape) { return; }
            if(entity != pedHandle.handle) { return; }
        }
        public void baseDestinationFail(ColShape shape, NetHandle entity) { }

        // --- Sequencing

        private readonly Dictionary<SequenceKey, Action> ActionSequence;
        public bool isSequenceActive = false;

        public void sequenceTick() {
            if (!isSequenceActive) { return; }
            if (ActionSequence.Count == 0) { return; }
            if (ActionSequence.First().Key.triggerTick <= API.TickCount) {
                PlayNextAction();
            }
        }

        public void StartActionSequence() {
            isSequenceActive = true;
        }

        private void PlayNextAction() {
            Action func = ActionSequence.First().Value;

            RemoveAction();
            func(); // Call the function
            onSequenceStepComplete();
        }

        public void AddAction(int delay, Action callback) {
            long del = delay;
            if(ActionSequence.Count > 0) {
                del = ((del * 1000) + (ActionSequence.Last().Key.delay * 1000));
            } else {
                del = (del * 1000) + 1;
            }
            
            ActionSequence.Add(new SequenceKey(delay, (del + API.TickCount)), callback);
        }

        public void RemoveAction() {
            ActionSequence.Remove(ActionSequence.First().Key);
        }

        public void ClearActions() {
            ActionSequence.Clear();
        }

        public void Speak(string message) {
            API.shared.SendCloseMessage(pedHandle, 15f, name + " says: " + message);
        }

        public void MoveTo(Vector3 dest, int speed = 2, int duration = -1) { MoveTo(dest.X, dest.Y, dest.Z, speed, duration); }
        public void MoveTo(float x, float y, float z, int speed = 2, int duration = -1) {
            Vector3 destination = new Vector3(x, y, z);
            Vector3 start = position;

            if (duration == -1) { duration = (int)Math.Floor((start.DistanceTo(destination) / speed)); }
            pedHandle.MoveTo(x, y, z, speed, duration);

            API.deleteColShape(destColShape);
            destColShape = API.createCylinderColShape(destination, 2.0f, 2.5f);
            destColShape.onEntityEnterColShape += onDestinationReached;
        }

        public void DriveTo(GrandTheftMultiplayer.Server.Elements.Vehicle veh, Vector3 dest, float speed) { DriveTo(veh, dest.X, dest.Y, dest.Z, speed); }
        public void DriveTo(GrandTheftMultiplayer.Server.Elements.Vehicle veh, float x, float y, float z, float speed) {
            pedHandle.DriveTo(veh, x, y, z, speed);

            API.deleteColShape(destColShape);
            destColShape = API.createCylinderColShape(new Vector3(x, y, z), 2.5f, 2.5f);
            destColShape.onEntityEnterColShape += onDestinationReached;
        }
    }
}
