using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Server.Constant;
using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;

namespace PBRP
{
    public enum AnimalState
    {
        Wandering = 0,
        Fleeing = 1,
        Attacking = 2,
        Eating = 3,
        Dead = 4
    }
    public enum AnimalMeatType
    {
        None = -1,
        Venison = 0,
        Rabbit = 1,
        Seagull = 2,
        Pigeon = 3,
        Swallow = 3,
        Pork = 4,
        Beef = 5,
        Crow = 6,
        Hawk = 7,
        Lion = 8,

    }

    public enum AnimalType
    {
        LargeLand = 0,
        SmallLand = 1,
        Bird = 2,
        Marine = 3
    }

    public class Wildlife
    {
        public Wildlife(PedHash model)
        {
            switch (model)
            {
                case PedHash.Deer:
                    Type = AnimalType.LargeLand;
                    Meat = AnimalMeatType.Venison;
                    Health = 75;
                    Aggressiveness = 3;
                    break;
                case PedHash.Rabbit:
                    Type = AnimalType.SmallLand;
                    Meat = AnimalMeatType.Rabbit;
                    Health = 30;
                    Aggressiveness = 2;
                    break;
                case PedHash.Boar:
                    break;
                case PedHash.Pigeon:
                    Type = AnimalType.Bird;
                    Meat = AnimalMeatType.Pigeon;
                    Health = 30;
                    Aggressiveness = 0;
                    break;
            }
            Model = model;
            BloodLevel = 100;
            IsInjured = false;
            IsDead = true;
            IsEating = false;

            BulletWounds = 0;
            StabWounds = 0;
            Hunger = 0;
            State = AnimalState.Wandering;
            CurrentPosition = new Vector3(-1110.023, 4994.859, 177.0264);
            CurrentDesination = CurrentPosition;
            Ped = Spawn();

            CurrentRotation = API.shared.getEntityRotation(Ped);
        }
        public static List<Wildlife> ActiveWildlife { get; set; }
        public PedHash Model { get; set; }
        public Ped Ped { get; set; }
        public float Health { get; set; }

        public float BloodLevel { get; set; }
        public float Hunger { get; set; }

        public bool IsInjured { get; set; }
        public bool IsDead { get; set; }

        public int BulletWounds { get; set; }
        public int StabWounds { get; set; }

        public int FleeDuration { get; set; }

        public bool IsEating { get; set; }

        public int movementTick { get; set; }

        public int InZone { get; set; }

        public DateTime PositionUpdated { get; set; }

        public AnimalState State { get; set; }
        public AnimalMeatType Meat { get; set; }
        public AnimalType Type { get; set; }

        public int Aggressiveness { get; set; }

        public Vector3 CurrentPosition { get; set; }
        public Vector3 CurrentRotation { get; set; }
        public Vector3 CurrentDesination { get; set; }

        public static List<Wildlife> DefaultWildlifeData = new List<Wildlife>()
        {
            new Wildlife(PedHash.Deer),
            new Wildlife(PedHash.Deer),
            new Wildlife(PedHash.Deer),
            new Wildlife(PedHash.Deer),
            new Wildlife(PedHash.Deer),
            new Wildlife(PedHash.Deer),
            new Wildlife(PedHash.Deer),
            //new Wildlife(PedHash.Deer),
            //new Wildlife(PedHash.Deer),
            //new Wildlife(PedHash.Deer),
            //new Wildlife(PedHash.Deer),
            //new Wildlife(PedHash.Deer),
            new Wildlife(PedHash.Rabbit),
            new Wildlife(PedHash.Rabbit),
            new Wildlife(PedHash.Rabbit),
            new Wildlife(PedHash.Rabbit),
            new Wildlife(PedHash.Rabbit),
            new Wildlife(PedHash.Rabbit),
        };

        public Ped Spawn()
        {
            Vector3 spawnPos = CurrentPosition;
            if (Type == AnimalType.Bird)
                spawnPos.Z += 15;

            return API.shared.createPed(Model, CurrentPosition, API.shared.RandomNumber(359));
        }

        public void MoveTo(Vector3 newPosition)
        {
            float speed = 3;
            switch (State)
            {
                case AnimalState.Wandering:
                    speed = 1f;
                    break;
                case AnimalState.Fleeing:
                    speed = 4;
                    break;
                case AnimalState.Eating:
                    speed = 0;
                    break;
            }

            API.shared.setEntityPositionFrozen(Ped, false);
            API.shared.sendNativeToAllPlayers(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, Ped, false);
            API.shared.sendNativeToAllPlayers(Hash.TASK_SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, Ped, false);
            API.shared.sendNativeToAllPlayers(Hash.TASK_FOLLOW_NAV_MESH_TO_COORD, Ped, newPosition.X, newPosition.Y, newPosition.Z, speed, -1, 0f, 0, 0f);
        }
    }
}
