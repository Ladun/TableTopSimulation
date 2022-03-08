using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class TableObject : GameObject
    {
        // Event Type -> bit 위치에 따른 이벤트 여부
        // [over/unover(0)] 
        // [select/deselect(1)]
        // [lock/unlock(2)]
        // [Merge(3)]

        public TableObjectSet parentSet = null;
        protected Dictionary<TableObjectEventType, Func<ObjectEvent, Player, ObjectEvent>> eventDict = new Dictionary<TableObjectEventType, Func<ObjectEvent, Player, ObjectEvent>>();

        public HashSet<int> overPlayerIds = new HashSet<int>();
        public bool Over
        {
            get
            {
                return overPlayerIds.Count > 0;
            }
        }

        public int OwnerPlayerId { get; set; }
        public bool Select
        {
            get
            {
                return OwnerPlayerId != 0;
            }
        }
        public bool Lock;


        protected override void Init()
        {
            ObjectType = GameObjectType.TableObject;

            eventDict.Add(TableObjectEventType.Over, OverEvent);
            eventDict.Add(TableObjectEventType.Select, SelectEvent);
            eventDict.Add(TableObjectEventType.Lock, LockEvent);
            eventDict.Add(TableObjectEventType.Merge, MergeEvent);
        }


        public ObjectEvent DoEvent(ObjectEvent e, Player player)
        {
            Func<ObjectEvent, Player, ObjectEvent> eventHandler = null;

            if (eventDict.TryGetValue(e.ObjectEventId, out eventHandler))
            {
                return eventHandler.Invoke(e, player);
            }

            return null;

        }

        #region Event Handler
        protected ObjectEvent OverEvent(ObjectEvent e, Player player)
        {
            // Over event
            // ObjectValue: over(1), unover(0)
            bool over = e.ObjectValue == 1;
            if (over)
            {
                if (!overPlayerIds.Contains(player.Id))
                    overPlayerIds.Add(player.Id);
            }
            else
            {
                overPlayerIds.Remove(player.Id);
            }
            return e;
        }
        public ObjectEvent SelectEvent(ObjectEvent e, Player player)
        {
            // Select event
            // ObjectValue: Select(1), Deselect(0)
            bool select = e.ObjectValue == 1;
            if (select)
            {
                if (OwnerPlayerId == 0)
                {
                    if (!Over)
                        OverEvent(new ObjectEvent()
                        {
                            ObjectEventId = TableObjectEventType.Over,
                            ObjectValue = 1
                        }, player);
                    OwnerPlayerId = player.Id;
                    player.interactObject = this;

                    return e;
                }
            }
            else
            {
                if (OwnerPlayerId != 0 && OwnerPlayerId == player.Id)
                {
                    if (Over)
                        OverEvent(new ObjectEvent()
                        {
                            ObjectEventId = TableObjectEventType.Over,
                            ObjectValue = 0
                        }, player);
                    OwnerPlayerId = 0;
                    player.interactObject = null;
                    return e;
                }
            }   
            return null;

        }
        protected ObjectEvent LockEvent(ObjectEvent e, Player player)
        {
            // Lock event
            // ObjectValue: Lock(1), Unlock(0)
            if (!Select)
            {
                if (e.ObjectValue == 1) // Lock
                {
                    Lock = true;
                }
                else // Unlock
                {
                    Lock = false;
                }
            }
            return e;
        }


        protected ObjectEvent MergeEvent(ObjectEvent e, Player player)
        {
            // Merge event
            // ObjectValue: TableObject ID

            TableObject to = Room.objectManager.Find<TableObject>(e.ObjectValue);
            if (to == null)
                return null;

            TableObjectSet set = null;

            if (ObjectManager.GetObjectTypeById(Id) == GameObjectType.TableObject) // Self is T.O
            {
                if (to.parentSet == null) // Other is T.O
                {
                    set = Room.EnterObject<TableObjectSet>("tableObjectSet", "", to.Info.Pos, to.Info.Angle);

                    set.Add(to);
                    set.Add(this);
                }
                else // Other is Set
                {
                    set = to.parentSet;
                    set.Add(this);
                }
                OverEvent(new ObjectEvent()
                {
                    ObjectEventId = TableObjectEventType.Over,
                    ObjectValue = 0
                }, player);
                SelectEvent(new ObjectEvent()
                {
                    ObjectEventId = TableObjectEventType.Select,
                    ObjectValue = 0
                }, player);
            }
            else // Self is Set
            {
                if (to.parentSet != null) // Other is Set
                {
                    set = to.parentSet;
                    set.Add(this as TableObjectSet);
                }
            }

            if (set != null)
                e.Flag = set.Id;


            return e;
        }

        #endregion
    }
}
