using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class RoomManager
    {
        public static RoomManager Instance { get; } = new RoomManager();

        object _lock = new object();
        Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        int _roomId = 1;

        public GameRoom Add(int playerId, string roomName, MapType mapType, float v1, float v2, int maxPlayers)
        {
            if (string.IsNullOrEmpty(roomName))
                roomName = "Room_" + _roomId;

            GameRoom gameRoom = new GameRoom(playerId, roomName, mapType, v1, v2, maxPlayers);

            lock (_lock)
            {
                gameRoom.RoomId = _roomId;
                _rooms.Add(_roomId, gameRoom);
                _roomId++;
            }

            return gameRoom;
        }

        public bool Remove(int roomId)
        {
            lock (_lock)
            {
                GameRoom room = null;
                bool b = _rooms.Remove(roomId, out room);
                if (b)
                {
                    room.DeleteRoom();
                }

                return b;
            }
        }

        public GameRoom Find(int roomId)
        {
            lock (_lock)
            {
                GameRoom room = null;
                if(_rooms.TryGetValue(roomId, out room))
                {
                    return room;
                }
                return null;
            }
        }

        public GameRoom FindByOwnerId(int ownerId)
        {
            GameRoom r = null;
            lock (_lock)
            {
                foreach(GameRoom room in _rooms.Values)
                {
                    if(room.OwnerId == ownerId)
                    {
                        r = room;
                        break;
                    }
                }
            }
            return r;
        }

        public void GetRoomList(S_RoomList roomPacket, int roomId, string roomKeyword)
        {
            lock (_lock)
            {
                foreach(GameRoom room in _rooms.Values)
                {
                    //TODO roomId와 roomName에 맞게
                    roomPacket.LobbyRooms.Add(room.GetRoomInfo());
                    roomPacket.CurrentPlayerCounts.Add(room.CurrentPlayerCount);
                }
            }
        }

    }
}
