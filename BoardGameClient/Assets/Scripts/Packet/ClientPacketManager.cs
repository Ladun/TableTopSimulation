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
		_onRecv.Add((ushort)MsgId.SLogin, MakePacket<S_Login>);
		_handler.Add((ushort)MsgId.SLogin, PacketHandler.S_LoginHandler);		
		_onRecv.Add((ushort)MsgId.SChat, MakePacket<S_Chat>);
		_handler.Add((ushort)MsgId.SChat, PacketHandler.S_ChatHandler);		
		_onRecv.Add((ushort)MsgId.SFileTransfer, MakePacket<S_FileTransfer>);
		_handler.Add((ushort)MsgId.SFileTransfer, PacketHandler.S_FileTransferHandler);		
		_onRecv.Add((ushort)MsgId.SRoomList, MakePacket<S_RoomList>);
		_handler.Add((ushort)MsgId.SRoomList, PacketHandler.S_RoomListHandler);		
		_onRecv.Add((ushort)MsgId.SPlayerList, MakePacket<S_PlayerList>);
		_handler.Add((ushort)MsgId.SPlayerList, PacketHandler.S_PlayerListHandler);		
		_onRecv.Add((ushort)MsgId.SPlayerInfo, MakePacket<S_PlayerInfo>);
		_handler.Add((ushort)MsgId.SPlayerInfo, PacketHandler.S_PlayerInfoHandler);		
		_onRecv.Add((ushort)MsgId.SMakeRoom, MakePacket<S_MakeRoom>);
		_handler.Add((ushort)MsgId.SMakeRoom, PacketHandler.S_MakeRoomHandler);		
		_onRecv.Add((ushort)MsgId.SEnterRoom, MakePacket<S_EnterRoom>);
		_handler.Add((ushort)MsgId.SEnterRoom, PacketHandler.S_EnterRoomHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveLobby, MakePacket<S_LeaveLobby>);
		_handler.Add((ushort)MsgId.SLeaveLobby, PacketHandler.S_LeaveLobbyHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SInteract, MakePacket<S_Interact>);
		_handler.Add((ushort)MsgId.SInteract, PacketHandler.S_InteractHandler);		
		_onRecv.Add((ushort)MsgId.SSync, MakePacket<S_Sync>);
		_handler.Add((ushort)MsgId.SSync, PacketHandler.S_SyncHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveRoom, MakePacket<S_LeaveRoom>);
		_handler.Add((ushort)MsgId.SLeaveRoom, PacketHandler.S_LeaveRoomHandler);		
		_onRecv.Add((ushort)MsgId.SRoomPlayerList, MakePacket<S_RoomPlayerList>);
		_handler.Add((ushort)MsgId.SRoomPlayerList, PacketHandler.S_RoomPlayerListHandler);
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