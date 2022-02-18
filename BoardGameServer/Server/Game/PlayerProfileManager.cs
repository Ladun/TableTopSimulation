using Google.Protobuf;
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

        public PlayerProfile Login(ClientSession session, string name)
        {
            PlayerProfile profile = new PlayerProfile();
            profile.Id = GenerateId();
            if (string.IsNullOrEmpty(name))
                name = "Player_" + profile.Id;
            profile.Name = name;
            profile.isLogin = true;
            profile.Session = session;

            lock (_lock)
            {
                _players.Add(profile.Id, profile);
            }

            return profile;
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

        public List<PlayerProfile> GetProfiles()
        {
            List<PlayerProfile> l = new List<PlayerProfile>();
            lock (_lock)
            {
                foreach (PlayerProfile p in _players.Values)
                {
                    l.Add(p);
                }
            }

            return l;
        }

        public void Broadcast(IMessage packet)
        {
            lock (_lock)
            {
                foreach (PlayerProfile _p in _players.Values)
                {
                    _p.Session.Send(packet);
                }
            }

        }
    }
}
