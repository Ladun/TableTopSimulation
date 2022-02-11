using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }		

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.CLogin, MakePacket<C_Login>);
		_handler.Add((ushort)MsgId.CLogin, PacketHandler.C_LoginHandler);		
		_onRecv.Add((ushort)MsgId.CChat, MakePacket<C_Chat>);
		_handler.Add((ushort)MsgId.CChat, PacketHandler.C_ChatHandler);		
		_onRecv.Add((ushort)MsgId.CRoomList, MakePacket<C_RoomList>);
		_handler.Add((ushort)MsgId.CRoomList, PacketHandler.C_RoomListHandler);		
		_onRecv.Add((ushort)MsgId.CPlayerList, MakePacket<C_PlayerList>);
		_handler.Add((ushort)MsgId.CPlayerList, PacketHandler.C_PlayerListHandler);		
		_onRecv.Add((ushort)MsgId.CPlayerInfo, MakePacket<C_PlayerInfo>);
		_handler.Add((ushort)MsgId.CPlayerInfo, PacketHandler.C_PlayerInfoHandler);		
		_onRecv.Add((ushort)MsgId.CMakeRoom, MakePacket<C_MakeRoom>);
		_handler.Add((ushort)MsgId.CMakeRoom, PacketHandler.C_MakeRoomHandler);		
		_onRecv.Add((ushort)MsgId.CEnterRoom, MakePacket<C_EnterRoom>);
		_handler.Add((ushort)MsgId.CEnterRoom, PacketHandler.C_EnterRoomHandler);		
		_onRecv.Add((ushort)MsgId.CLeaveLobby, MakePacket<C_LeaveLobby>);
		_handler.Add((ushort)MsgId.CLeaveLobby, PacketHandler.C_LeaveLobbyHandler);		
		_onRecv.Add((ushort)MsgId.CSpawn, MakePacket<C_Spawn>);
		_handler.Add((ushort)MsgId.CSpawn, PacketHandler.C_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.CDespawn, MakePacket<C_Despawn>);
		_handler.Add((ushort)MsgId.CDespawn, PacketHandler.C_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.CMove, MakePacket<C_Move>);
		_handler.Add((ushort)MsgId.CMove, PacketHandler.C_MoveHandler);		
		_onRecv.Add((ushort)MsgId.CInteract, MakePacket<C_Interact>);
		_handler.Add((ushort)MsgId.CInteract, PacketHandler.C_InteractHandler);		
		_onRecv.Add((ushort)MsgId.CLeaveRoom, MakePacket<C_LeaveRoom>);
		_handler.Add((ushort)MsgId.CLeaveRoom, PacketHandler.C_LeaveRoomHandler);		
		_onRecv.Add((ushort)MsgId.CRoomPlayerList, MakePacket<C_RoomPlayerList>);
		_handler.Add((ushort)MsgId.CRoomPlayerList, PacketHandler.C_RoomPlayerListHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
	
		if(CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}