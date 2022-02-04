using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class PlayerProfileManager
    {
        public static PlayerProfileManager Instance { get; } = new PlayerProfileManager();

        object _lock = new object();
        Dictionary<int, PlayerProfile> _players = new Dictionary<int, PlayerProfile>();

        // [UNUSED(1)][TYPE(7)][ID(24)]
        int _counter = 1; // TODO

        public PlayerProfile Add()
        {
            PlayerProfile profile = new PlayerProfile();

            lock (_lock)
            {
                profile.Id = GenerateId();
                _players.Add(profile.Id, profile);
            }
            return profile;
        }

        int GenerateId()
        {
            lock (_lock)
            {
                return ((int)GameObjectType.Player << 24) | (_counter++);
            }
        }

        public bool Remove(int objectId)
        {
            lock (_lock)
            {
                return _players.Remove(objectId);
            }
        }

        public PlayerProfile Find(int objectId)
        {
            lock (_lock)
            {
                PlayerProfile player = null;
                if (_players.TryGetValue(objectId, out player))
                {
                    return player;
                }
            }
            return null;
        }

        public void Login(int playerId, string name)
        {
            lock (_lock)
            {
                PlayerProfile player = Find(playerId);
                if (player == null)
                    return;
                if (string.IsNullOrEmpty(name))
                    name = "Player_" + playerId;
                player.Name = name;
                player.isLogin = true;
            }
        }

        public void Logout(int playerId)
        {
            lock (_lock)
            {
                PlayerProfile player = Find(playerId);
                if (player == null)
                    return;
                player.isLogin = false;
            }
        }

        public void GetPacketProfiles(S_PlayerList playerListPacket)
        {

            lock (_lock)
            {
                foreach(PlayerProfile p in _players.Values)
                {
                    if(p.isLogin)
                        playerListPacket.PlayerProfiles.Add(p.GetPacketProfile());
                }
            }
        }
    }
}
