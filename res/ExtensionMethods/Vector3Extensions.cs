using GrandTheftMultiplayer.Shared.Math;
using System;

namespace PBRP
{
    public static class Vector3Extensions
    {
        public static bool IsInArea(this Vector3 pos, Vector3[] pORect)
        {
            float distance = (pORect[0].DistanceTo2D(pORect[2]) < pORect[1].DistanceTo2D(pORect[3]) ? pORect[0].DistanceTo2D(pORect[2]) : pORect[1].DistanceTo2D(pORect[2])) / 1.7f;

            int count = 0;
            foreach (Vector3 v in pORect)
                if (pos.DistanceTo2D(v) <= distance) count++;

            return count >= 2;
        }

        public static Vector3 Forward(this Vector3 point, float rot, float dist)
        {
            var angle = rot;
            double xOff = -(Math.Sin((angle * Math.PI) / 180) * dist);
            double yOff = Math.Cos((angle * Math.PI) / 180) * dist;

            return point.Add(new Vector3(xOff, yOff, 0));
        }

        public static Vector3 Backward(this Vector3 point, float rot, float dist)
        {
            var angle = rot;
            double xOff = (Math.Cos((angle * Math.PI) / 180) * dist);
            double yOff = -(Math.Sin((angle * Math.PI) / 180) * dist);

            return point.Add(new Vector3(xOff, yOff, 0));
        }

        public static float ClampAngle(float angle)
        {
            return (float)(angle + Math.Ceiling(-angle / 360) * 360);
        }
    }
}
