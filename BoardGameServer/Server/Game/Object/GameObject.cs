using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; }

        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }
        public GameRoom Room { get; set; }

        public ObjectInfo _info;
        public ObjectInfo Info { 
            get
            {
                if(_info == null)
                {
                    _info = new ObjectInfo();
                    _info.Pos = new Dim3Info();
                    _info.Angle = new Dim3Info();
                }
                return _info;
            }
            set
            {
                _info = value;
            }
        }

        public GameObject()
        {
            Init();
        }

        protected virtual void Init()
        {

        }

    }
}
