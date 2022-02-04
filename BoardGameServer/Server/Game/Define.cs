using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Define
    {
        public struct Vector3
        {
            public float x;
            public float y;
            public float z;

            public Vector3(float x, float y, float z) { this.x = x; this.y = y;this.z = z; }

            public static Vector3 zero { get { return new Vector3(0, 0, 0); } }

            public static Vector3 operator+(Vector3 a, Vector3 b)
            {
                return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
            }
            public static Vector3 operator -(Vector3 a, Vector3 b)
            {
                return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            }

            public static Vector3 Dim32Vector3(Dim3Info info)
            {
                return new Vector3(info.X, info.Y, info.Z);
            }
            public static Dim3Info Vector32Dim3(Vector3 info)
            {
                Dim3Info dim = new Dim3Info();
                dim.X = info.x;
                dim.Y = info.y;
                dim.Z = info.z;
                return dim;
            }
        }
    }
}
