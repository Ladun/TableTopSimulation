using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Preset: TableObject
    {


        protected override void Init()
        {
            ObjectType = GameObjectType.Preset;

            eventDict.Add(TableObjectEventType.Over, OverEvent);
            eventDict.Add(TableObjectEventType.Select, SelectEvent);
            eventDict.Add(TableObjectEventType.Lock, LockEvent);
        }
    }
}
