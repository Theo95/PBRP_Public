using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.API;
using System;

namespace PBRP
{
    public class Utils : Script {

        public static void SetEntityFacingEntity(NetHandle entity, NetHandle toEntity) {
            SetEntityFacingVector(entity, API.shared.getEntityPosition(toEntity));
        }

        public static void SetEntityFacingVector(NetHandle entity, Vector3 position) {
            Vector3 up = new Vector3(0.0f, 1.0f, 1.0f);
            Vector3 entityPos = API.shared.getEntityPosition(entity);

            double angleRad = Math.Atan2((entityPos.Y - position.Y), (entityPos.X - position.X));
            double angleDeg = (angleRad * (180 / Math.PI));

            API.shared.setEntityRotation(entity, new Vector3(0.0f, 0.0f, angleDeg));
        }
    }
}
