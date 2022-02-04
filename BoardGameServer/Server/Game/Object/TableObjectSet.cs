using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class TableObjectSet : TableObject
    {

        public List<TableObject> containedObjects = new List<TableObject>();

        protected override void Init()
        {
            ObjectType = GameObjectType.TableObjectSet;

            eventDict.Add(TableObjectEventType.Over, OverEvent);
            eventDict.Add(TableObjectEventType.Select, SelectEvent);
            eventDict.Add(TableObjectEventType.Lock, LockEvent);
            eventDict.Add(TableObjectEventType.Merge, MergeEvent);
            eventDict.Add(TableObjectEventType.Pick, PickEvent);
            eventDict.Add(TableObjectEventType.Shuffle, ShuffleEvent);
        }

        public void Add(TableObject to)
        {

            containedObjects.Add(to);
            to.parentSet = this;
        }

        public void Add(TableObjectSet otherSet)
        {
            foreach (TableObject to in otherSet.containedObjects)
            {
                Add(to);
            }
            otherSet.FreeAllObject();
        }

        public void Remove(TableObject to)
        {
            containedObjects.Remove(to);
            if (to.parentSet == this)
                to.parentSet = null;
        }
        public void FreeAllObject()
        {
            foreach (TableObject to in containedObjects)
            {
                if(to.parentSet == this)
                    to.parentSet = null;
            }
        }


        private ObjectEvent PickEvent(ObjectEvent e, Player p)
        {
            SelectEvent(new ObjectEvent()
            {
                ObjectEventId = TableObjectEventType.Select,
                ObjectValue = 0
            }, p);
            TableObject to = containedObjects[containedObjects.Count - 1];
            Remove(to);

            if (e.Flag == 1)
            {
                to.SelectEvent(new ObjectEvent()
                {
                    ObjectEventId = TableObjectEventType.Select,
                    ObjectValue = 1
                }, p);
            }

            return e;
        }

        private ObjectEvent ShuffleEvent(ObjectEvent e, Player p)
        {
            int size = containedObjects.Count;
            int[] shuffledIdx = new int[size];
            for (int i = 0; i < size; i++)
                shuffledIdx[i] = i;

            Random rand = new Random();

            for (int i = 0; i < size; i++)
            {
                int r = rand.Next(size);

                int t = shuffledIdx[i];
                shuffledIdx[i] = shuffledIdx[r];
                shuffledIdx[r] = t;
            }

            for (int i = 0; i < size; i++)
            {
                TableObject to = containedObjects[i];
                containedObjects[i] = containedObjects[shuffledIdx[i]];
                containedObjects[shuffledIdx[i]] = to;
            }

            e.ShuffleIdx.Add(shuffledIdx);
            return e;
        }
    }
}
