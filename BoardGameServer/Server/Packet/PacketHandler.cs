using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using Server;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{

    #region Intro Scene
    public static void C_LoginHandler(PacketSession session, IMessage packet)
    {
        C_Login loginPacket = packet as C_Login;
        ClientSession clientSession = session as ClientSession;

        Console.WriteLine("Login: " + clientSession.MyPlayer.Id + "| " + loginPacket.Name);
        PlayerProfileManager.Instance.Login(clientSession.MyPlayer.Id, loginPacket.Name);
        // 여기에 로그인 같은거 추가하면 됨

        S_Login sLoginPacket = new S_Login();
        sLoginPacket.Success = true; // 로그인 여부에 따라서 바꾸면 됨
        clientSession.Send(sLoginPacket);
    }
    #endregion

    #region Lobby Scene
    public static void C_MakeRoomHandler(PacketSession session, IMessage packet)
    {
        C_MakeRoom makeRoomPacket = packet as C_MakeRoom;
        ClientSession clientSession = session as ClientSession;

        int playerId = clientSession.MyPlayer.Id;

        if (RoomManager.Instance.FindByOwnerId(playerId) != null)
            return;

        RoomInfo roomInfo = RoomManager.Instance.Add(playerId, makeRoomPacket.Name, makeRoomPacket.Type, makeRoomPacket.V1, makeRoomPacket.V2, makeRoomPacket.MaxPlayers).GetRoomInfo();

        Console.WriteLine("MakeRoom: " + playerId + "| " + roomInfo.Name );

        S_MakeRoom sMakeRoomPacket = new S_MakeRoom();
        sMakeRoomPacket.RoomInfo = roomInfo;

        clientSession.Send(sMakeRoomPacket);
    }
    public static void C_RoomListHandler(PacketSession session, IMessage packet)
    {
        C_RoomList roomListPacket = packet as C_RoomList;
        ClientSession clientSession = session as ClientSession;


        S_RoomList sRoomListPacket = new S_RoomList();
        RoomManager.Instance.GetRoomList(sRoomListPacket, roomListPacket.RoomId, roomListPacket.RoomKeyword);

        clientSession.Send(sRoomListPacket);
    }
    public static void C_PlayerListHandler(PacketSession session, IMessage packet)
    {
        C_PlayerList playerListPacket = packet as C_PlayerList;
        ClientSession clientSession = session as ClientSession;

        S_PlayerList sPlayerListPacket = new S_PlayerList();

        PlayerProfileManager.Instance.GetPacketProfiles(sPlayerListPacket);

        clientSession.Send(sPlayerListPacket);
    }
    public static void C_PlayerInfoHandler(PacketSession session, IMessage packet)
    {
        C_PlayerInfo playerInfoPacket = packet as C_PlayerInfo;
        ClientSession clientSession = session as ClientSession;
    }

    public static void C_EnterRoomHandler(PacketSession session, IMessage packet)
    {
        C_EnterRoom enterPacket = packet as C_EnterRoom;
        ClientSession clientSession = session as ClientSession;

        GameRoom room = RoomManager.Instance.Find(enterPacket.RoomInfo.RoomId);
        if (room == null)
        {
            S_EnterRoom sEnterPacket = new S_EnterRoom();
            sEnterPacket.SuccessCode = 3; // There is no room -> 3
            clientSession.Send(sEnterPacket);
            Console.WriteLine("No Room!");
            return;
        }

        room.EnterPlayer(clientSession.MyPlayer);
    }
    public static void C_LeaveLobbyHandler(PacketSession session, IMessage packet)
    {
        C_LeaveLobby lobbyPacket = packet as C_LeaveLobby;
        ClientSession clientSession = session as ClientSession;

        PlayerProfileManager.Instance.Logout(clientSession.MyPlayer.Id);

        S_LeaveLobby sLobbyPacket = new S_LeaveLobby();
        clientSession.Send(sLobbyPacket);
    }
    #endregion

    #region Game Scene

    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        GameRoom room = clientSession.MyPlayer.Room;
        if (room == null)
            return;

        // TODO 검증 과정
        room.HandleMove(movePacket);
    }
    public static void C_SpawnHandler(PacketSession session, IMessage packet)
    {
        C_Spawn spawnPacket = packet as C_Spawn;
        ClientSession clientSession = session as ClientSession;

        GameRoom room = clientSession.MyPlayer.Room;
        if (room == null)
            return;

        if (spawnPacket.ObjectType == GameObjectType.Preset)
            room.EnterObject<Preset>(spawnPacket.Name, spawnPacket.Pos, spawnPacket.Angle);

        else
            room.EnterObject<TableObject>(spawnPacket.Name, spawnPacket.Pos, spawnPacket.Angle);

    }

    public static void C_DespawnHandler(PacketSession session, IMessage packet)
    {
        C_Despawn despawnPacket = packet as C_Despawn;
        ClientSession clientSession = session as ClientSession;

        GameRoom room = clientSession.MyPlayer.Room;
        if (room == null)
            return;

        foreach (int objectId in despawnPacket.ObjectIds)
        {
            Console.WriteLine("Despawn: " + objectId);
            room.LeaveGame(objectId);
        }

    }

    public static void C_InteractHandler(PacketSession session, IMessage packet)
    {
        C_Interact interactPacket = packet as C_Interact;
        ClientSession clientSession = session as ClientSession;

        GameRoom room = clientSession.MyPlayer.Room;
        if (room == null)
            return;

        room.HandleInteract(interactPacket, clientSession.MyPlayer.Id);

    }
    public static void C_LeaveRoomHandler(PacketSession session, IMessage packet)
    {
        C_LeaveRoom leavePacket = packet as C_LeaveRoom;
        ClientSession clientSession = session as ClientSession;


        GameRoom room = clientSession.MyPlayer.Room;
        if (room == null)
            return;

            room.LeaveGame(clientSession.MyPlayer.Id);
    }

    public static void C_RoomPlayerListHandler(PacketSession session, IMessage packet)
    {
        C_RoomPlayerList playerListPacket = packet as C_RoomPlayerList;
        ClientSession clientSession = session as ClientSession;

        S_RoomPlayerList sPlayerListPacket = new S_RoomPlayerList();

        GameRoom room = clientSession.MyPlayer.Room;
        if (room == null)
            return;
        room.GetPacketProfiles(sPlayerListPacket);

        clientSession.Send(sPlayerListPacket);
    }
    #endregion
}
