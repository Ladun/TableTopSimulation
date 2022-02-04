using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using static Server.Game.Define;

namespace Server.Game.Room
{
    public class RectMapInfo : MapInfo
    {

        public float width;
        public float height;

        public RectMapInfo(float width, float height)
        {
            Type = MapType.Rectangle;
            this.width = width;
            this.height = height;
        }
        public override Vector3 GetPlayerPosition(int index, int playerCount)
        {
            if(width > height)
            {

            }
            else
            {

            }

            return Vector3.zero;
        }
    }
}
