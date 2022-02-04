using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

using static Server.Game.Define;

namespace Server.Game.Room
{
    public class CircleMapInfo : MapInfo
    {
        public float radius;

        public CircleMapInfo(float radius) 
        {
            Type = MapType.Circle;
            this.radius = radius;
        }

        public override Vector3 GetPlayerPosition(int index, int playerCount)
        {
            float anglePerPlayer = 360 / playerCount;

            float deg2rad = MathF.PI / 180;

            float x = MathF.Cos(deg2rad * (index * anglePerPlayer)) * radius;
            float z = MathF.Sin(deg2rad * (index * anglePerPlayer)) * radius;

            return new Vector3(x, 0, z);
        }
    }
}
