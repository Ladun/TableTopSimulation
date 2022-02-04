using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

using static Server.Game.Define;

namespace Server.Game.Room
{
    public abstract class MapInfo
    {

        public MapType Type { get; protected set; }

        public abstract Vector3 GetPlayerPosition(int index, int playerCount);

        public static MapInfo CreateMapInfo(MapType mapType, float v1, float v2)
        {
            switch (mapType)
            {
                case MapType.Circle:

                    return new CircleMapInfo(v1);

                case MapType.Rectangle:
                    return new RectMapInfo(v1, v2);
            }
            return null;
        }
    }
}
