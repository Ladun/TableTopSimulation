using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class ObjectManager
    {
        object _lock = new object();
        public Dictionary<int, Player> Players { get; } = new Dictionary<int, Player>();
        public Dictionary<int, GameObject> Objects { get; } = new Dictionary<int, GameObject>();

        // [UNUSED(1)][TYPE(7)][ID(24)]
        int _objectCounter = 1; // TODO

        public T Add<T>() where T:GameObject, new ()
        {
            T gameObject = new T();
            if (gameObject.ObjectType == GameObjectType.Player)
                return null;

            lock (_lock)
            {                
                gameObject.Id = GenerateId(gameObject.ObjectType);
                Objects.Add(gameObject.Id, gameObject);
            }
            return gameObject;
        }

        public Player Add(PlayerProfile profile)
        {
            Player player = new Player(profile);

            lock (_lock)
            {
                player.Info.ObjectId = profile.Id;
                Players.Add(profile.Id, player);
            }

            return player;
        }

        int GenerateId(GameObjectType type)
        {
            lock (_lock)
            {
                return ((int)type << 24) | (_objectCounter++);
            }
        }

        public static GameObjectType GetObjectTypeById(int id)
        {
            int type = (id >> 24) & 0x7F;
            return (GameObjectType)type;
        }

        public T Remove<T>(int objectId) where T : GameObject
        {
            GameObjectType objectType = GetObjectTypeById(objectId);
            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                {
                    Player obj = null;
                    Players.Remove(objectId, out obj);
                    return obj as T;
                }
                else 
                {
                    GameObject obj = null;
                    Objects.Remove(objectId, out obj);
                    return obj as T;
                }
            }
        }

        public T Find<T>(int objectId) where T: GameObject
        {
            GameObjectType objectType = GetObjectTypeById(objectId);
            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                {
                    Player gameObject = null;
                    if (Players.TryGetValue(objectId, out gameObject))
                    {
                        return gameObject as T;
                    }
                }
                else
                {
                    GameObject gameObject = null;
                    if (Objects.TryGetValue(objectId, out gameObject))
                    {
                        return gameObject as T;
                    }
                }
            }
            return null;
        }

    }
}
