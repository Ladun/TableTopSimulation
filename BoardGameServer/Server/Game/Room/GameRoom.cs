using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Text;
using static Server.Game.Define;
using ServerCore;

namespace Server.Game
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }
        public int OwnerId { get; private set; }

        public int CurrentPlayerCount
        {
            get
            {
                return objectManager.Players.Count;
            }
        }

        private string _roomName;
        public string RoomName { get { return _roomName; } }
        private int _maxPlayers;

        public ObjectManager objectManager = new ObjectManager();
        private MapInfo _mapInfo;

        private List<CColor> _colors = new List<CColor>();
        private List<string> _packageCodes = new List<string>();

        public GameRoom(int ownerId, string roomName, MapType mapType, float v1, float v2, int maxPlayers, RepeatedField<string> packageCodes)
        {
            OwnerId = ownerId;
            _roomName = roomName;
            _maxPlayers = maxPlayers;
            for(int i = 0; i < packageCodes.Count; i++)
            {
                _packageCodes.Add(packageCodes[i]);
            }

            _mapInfo = MapInfo.CreateMapInfo(mapType, v1, v2);

            Random random = new Random();
            for (int i = 0; i < maxPlayers; i++)
            {
                CColor color = new CColor();
                color.R = random.Next(256);
                color.G = random.Next(256);
                color.B = random.Next(256);
                _colors.Add(color);
            }
        }

        #region Enter/Leave

        public T EnterObject<T>(string name, string packageCode, Dim3Info pos, Dim3Info angle) where T : TableObject, new()
        {
            T tableObject = objectManager.Add<T>();
            tableObject.Info.Pos = pos;
            tableObject.Info.Angle = angle;
            tableObject.Room = this;
            tableObject.Info.Name = name;
            tableObject.Info.PackageCode = packageCode;

            EnterGame(tableObject);

            Logger.Instance.Print($"TableObject Id[{tableObject.Id}] spawned", Logger.LogLevel.DEBUG);

            return tableObject;
        }

        public void EnterPlayer(PlayerProfile profile)
        {
            if (CurrentPlayerCount >= _maxPlayers)
            {

                S_EnterRoom enterPacket = new S_EnterRoom();
                enterPacket.SuccessCode = 2; // Room is Full -> 2
                lock (_lock)
                {
                    profile.Session.Send(enterPacket);
                }
                return;
            }

            Player player = objectManager.Add(profile);
            player.Info.Flag = objectManager.Players.Count - 1;
            player.Room = this;
            player.Profile.Room = this;
            player.Info.Name = profile.Name;

            player.Info.Pos = Vector3.Vector32Dim3(_mapInfo.GetPlayerPosition(player.Info.Flag, 3));

            EnterGame(player);
        }
        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            GameObjectType objectType = ObjectManager.GetObjectTypeById(gameObject.Id);

            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                {
                    Player player = gameObject as Player;

                    // 본인한테 정보 전송
                    {
                        S_EnterRoom enterPacket = new S_EnterRoom();
                        enterPacket.RoomInfo = GetRoomInfo();
                        enterPacket.PlayerInfo = player.Info;
                        enterPacket.SuccessCode = 1;
                        foreach (CColor c in _colors)
                            enterPacket.Colors.Add(c);
                        player.Profile.Session.Send(enterPacket);

                        S_Spawn spawnPacket = new S_Spawn();
                        foreach (Player p in objectManager.Players.Values)
                        {
                            if (player != p)
                                spawnPacket.Objects.Add(p.Info);
                        }
                        foreach(GameObject go in objectManager.Objects.Values)
                        {
                            spawnPacket.Objects.Add(go.Info);
                        }
                        player.Profile.Session.Send(spawnPacket);
                    }
                }

                // 타인한테 정보 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.Objects.Add(gameObject.Info);
                    foreach(Player p in objectManager.Players.Values)
                    {
                        if (gameObject.Id != p.Id)
                            p.Profile.Session.Send(spawnPacket);
                    }
                }
            }
        }

        public void LeaveGame(int objectId)
        {
            GameObjectType objectType = ObjectManager.GetObjectTypeById(objectId);
            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                {
                    if (objectId == OwnerId)
                    {
                        RoomManager.Instance.Remove(RoomId);
                        return;
                    }

                    // Remove
                    Player player = objectManager.Remove<Player>(objectId);
                    Logger.Instance.Print("Remove Player: " + player);
                    if (player == null)
                        return;
                       player.Room = null;
                    player.Profile.Room = null; 

                    // 특정 오브젝트를 select하고 있으면 deselect
                    if(player.interactObject != null)
                    {
                        ObjectEvent oe = new ObjectEvent();
                        oe.ObjectEventId = TableObjectEventType.Select; // Select event
                        oe.ObjectValue = 0; // Deselect

                        player.interactObject.DoEvent(oe, player);

                        S_Interact interactPacket = new S_Interact();
                        interactPacket.ObjectId = player.interactObject.Id;
                        interactPacket.PlayerId = player.Id;
                        interactPacket.Events.Add(oe);

                        Broadcast(interactPacket);
                    }

                    // 본인한테 정보 전송
                    {
                        S_LeaveRoom leavePacket = new S_LeaveRoom();
                        player.Profile.Session.Send(leavePacket);
                    }
                }
                else if(objectType == GameObjectType.TableObject)
                {
                    TableObject to = objectManager.Remove<TableObject>(objectId);
                    if (to == null)
                        return;
                    //to.Room = null;

                    // 선택하고 있는 player가 있다면 null로 밀어줌
                    if(to.OwnerPlayerId != 0)
                    {
                        Player p = objectManager.Find<Player>(to.OwnerPlayerId);
                        if (p == null)
                            return;
                        p.interactObject = null;
                    }
                }
                else if(objectType == GameObjectType.TableObjectSet)
                {
                    TableObjectSet set = objectManager.Remove<TableObjectSet>(objectId);
                    if (set == null)
                        return;

                    set.FreeAllObject();
                }
                else if(objectType == GameObjectType.Preset)
                {
                    Preset preset = objectManager.Remove<Preset>(objectId);
                    if (preset == null)
                        return;
                }

                // 타인한테 정보 전송
                {
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.ObjectIds.Add(objectId);
                    foreach(Player p in objectManager.Players.Values)
                    {
                        if (p.Id != objectId)
                            p.Profile.Session.Send(despawnPacket);
                    }
                }
            }
        }

        public void DeleteRoom()
        {
            foreach (Player p in objectManager.Players.Values)
            {
                p.Room = null;
                p.Profile.Room = null;

                // 모든 플레이어한테 정보 전송
                {
                    S_LeaveRoom leavePacket = new S_LeaveRoom();
                    p.Profile.Session.Send(leavePacket);
                }
            }
        }
       
        public void Broadcast(IMessage packet)
        {
            lock (_lock)
            {
                foreach(Player _p in objectManager.Players.Values)
                {
                    _p.Profile.Session.Send(packet);
                }
            }

        }

        #endregion

        #region Handle
        public void HandleInteract(C_Interact packet, int playerId)
        {
            lock (_lock)
            {
                TableObject to = objectManager.Find<TableObject>(packet.ObjectId);
                if (to == null)
                    return;

                Player player = objectManager.Find<Player>(playerId);
                if (player == null)
                    return;

                S_Interact interactPacket = new S_Interact();
                interactPacket.ObjectId = packet.ObjectId;
                interactPacket.PlayerId = player.Id;

                foreach (ObjectEvent e in packet.Events)
                {
                    ObjectEvent ae = to.DoEvent(e, player);
                    if(ae != null)
                        interactPacket.Events.Add(ae);
                }

                Broadcast(interactPacket);

            }
        }

        public void HandleMove(C_Move packet)
        {
            S_Move sMovePacket = new S_Move();
            for (int i = 0; i < packet.Pos.Count; i++)
            {
                lock (_lock)
                {
                    GameObject go = objectManager.Find<GameObject>(packet.ObjectId[i]);
               
                    if (go == null)
                        continue;

                    go.Info.Pos = packet.Pos[i];
                    go.Info.Angle = packet.Angle[i];
                }

                sMovePacket.Pos.Add(packet.Pos[i]);
                sMovePacket.Angle.Add(packet.Angle[i]);
                sMovePacket.ObjectId.Add(packet.ObjectId[i]);

            }
            Broadcast(sMovePacket);
        }


        public void GetPacketProfiles(S_RoomPlayerList playerListPacket)
        {

            lock (_lock)
            {
                foreach (Player p in objectManager.Players.Values)
                {
                   playerListPacket.PlayerProfiles.Add(p.Profile.GetPacketProfile());
                }
            }
        }


        #endregion

        public RoomInfo GetRoomInfo()
        {

            RoomInfo roomInfo = new RoomInfo()
            {
                RoomId = RoomId,
                Name = _roomName,
                OwnerId = OwnerId,
                MaxPlayers = _maxPlayers
            };

            for(int i = 0; i < _packageCodes.Count; i++)
            {
                roomInfo.PackageCodes.Add(_packageCodes[i]);
            }

            return roomInfo;
        }
    }
}
