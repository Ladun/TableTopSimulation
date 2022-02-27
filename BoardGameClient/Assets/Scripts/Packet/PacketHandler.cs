using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{

    #region Global

    public static void S_ChatHandler(PacketSession session, IMessage packet)
	{
		S_Chat chatPacket = packet as S_Chat;

		Managers.Instance.GetUIManager<LobbyUIManager>().UpdateChatting(chatPacket);
	}

	public static void S_FileTransferHandler(PacketSession session, IMessage packet)
	{
		S_FileTransfer filePacket = packet as S_FileTransfer;

		if(filePacket.SendCode == 0)
        {
			C_FileTransfer cFilePacket = new C_FileTransfer();
			cFilePacket.SendCode = 1;
			cFilePacket.Name = filePacket.Name;
            if (Managers.Instance.Package.HasFile(filePacket.Name))
			{
				cFilePacket.Filebytes = ByteString.CopyFrom(Managers.Instance.Package.GetFileByte(filePacket.Name));
			}
			Managers.Instance.Network.Send(cFilePacket);
		}
		else if(filePacket.SendCode == 1)
        {
			//if (filePacket.HasFilebytes)
			{
				Managers.Instance.Package.SaveFile(filePacket.Name, filePacket.Filebytes.ToByteArray());
			}
        }

	}
	#endregion

	#region Intro Scene
	public static void S_LoginHandler(PacketSession session, IMessage packet)
	{
		S_Login loginPacket = packet as S_Login;

		if (loginPacket.Success)
		{
			IntroScene scene = Managers.Instance.GetScene<IntroScene>();
			if (scene == null)
				return;
			scene.EnterLobby();
		}
        else
        {
			// Login Failed
        }
	}
    #endregion

    #region Lobby Scene
    public static void S_MakeRoomHandler(PacketSession session, IMessage packet)
	{
		S_MakeRoom makeRoomPacket = packet as S_MakeRoom;

		RoomInfo roomInfo = makeRoomPacket.RoomInfo;

		LobbyScene scene = Managers.Instance.GetScene<LobbyScene>();
		scene.SendEnterRoom(roomInfo);
		Debug.Log("S_MakeRoomHandler");
	}

	public static void S_RoomListHandler(PacketSession session, IMessage packet)
	{
		S_RoomList roomListPacket = packet as S_RoomList;

		LobbyScene scene = Managers.Instance.GetScene<LobbyScene>();
		if (scene == null)
			return;

		List<RoomInfo> roomList = new List<RoomInfo>();


		foreach (RoomInfo room in roomListPacket.LobbyRooms)
		{
			roomList.Add(room);
		}

		List<int> playerCounts = new List<int>();
		foreach (int c in roomListPacket.CurrentPlayerCounts)
		{
			playerCounts.Add(c);
		}

		scene.UpdateRoomList(roomList, playerCounts);


	}

	public static void S_PlayerListHandler(PacketSession session, IMessage packet)
	{
		S_PlayerList movePacket = packet as S_PlayerList;

		LobbyUIManager uiManager = Managers.Instance.GetUIManager<LobbyUIManager>();
		if (uiManager == null)
			return;

		List<P_PlayerProfile> playerList = new List<P_PlayerProfile>();

		foreach (P_PlayerProfile player in movePacket.PlayerProfiles)
		{
			playerList.Add(player);
		}

		uiManager.UpdatePlayerList(playerList);
	}

	public static void S_PlayerInfoHandler(PacketSession session, IMessage packet)
	{
		S_PlayerInfo movePacket = packet as S_PlayerInfo;

		Debug.Log("S_PlayerInfoHandler");
	}
	
	public static void S_EnterRoomHandler(PacketSession session, IMessage packet)
	{
		S_EnterRoom enterGamePacket = packet as S_EnterRoom;


		if (enterGamePacket.SuccessCode == 1)
		{
			LobbyScene scene = Managers.Instance.GetScene<LobbyScene>();
			if (scene == null)
				return;
			scene.EnterGame(enterGamePacket.RoomInfo, enterGamePacket.PlayerInfo, enterGamePacket.Colors);
		}
        else
        {
			LobbyUIManager uiManager = Managers.Instance.GetUIManager<LobbyUIManager>();
			if (uiManager == null)
				return;

			uiManager.CloseLoading();
			uiManager.OpenPopupItem("InformationDisplayPopup");
			string failMessage = "Room is Full";
			if (enterGamePacket.SuccessCode == 3)
				failMessage = "There is no Room";
			uiManager.GetPopupItem("InformationDisplayPopup").GetComponent<InformationDisplayPopup>().Setting(failMessage);
		}

	}
	public static void S_LeaveLobbyHandler(PacketSession session, IMessage packet)
	{
		S_LeaveLobby enterGamePacket = packet as S_LeaveLobby;

		LobbyScene scene = Managers.Instance.GetScene<LobbyScene>();
		if (scene == null)
			return;
		scene.LeaveLobby();
	}
	#endregion

	#region Game Scene


	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;

		GameScene scene = Managers.Instance.GetScene<GameScene>();
		if (scene == null)
		{
			// ERROR
			return;
		}
		
		foreach (ObjectInfo objectInfo in spawnPacket.Objects)
		{
			Debug.LogFormat("S_SpawnHandler: Spawn {0} {1}", objectInfo.Name, objectInfo.ObjectId);
			scene.objectManager.Add(objectInfo);
		}
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;

		GameScene scene = Managers.Instance.GetScene<GameScene>();
		if (scene == null)
		{
			// ERROR
			return;
		}

		foreach (int id in despawnPacket.ObjectIds)
		{
			Debug.Log("Despawn: " + id);
			scene.objectManager.Remove(id);
		}
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;


		GameScene scene = Managers.Instance.GetScene<GameScene>();
		if (scene == null)
			return;

		for (int i = 0; i < movePacket.ObjectId.Count; i++)
		{

			int objectId = movePacket.ObjectId[i];
			Vector3 pos = Utils.Dim3Info2Vector3(movePacket.Pos[i]); 
			Vector3 angle = Utils.Dim3Info2Vector3(movePacket.Angle[i]);


			GameObjectType objectType = ObjectManager.GetObjectTypeById(objectId);
			if (objectType == GameObjectType.Player)
			{
				Player target = scene.objectManager.Find<Player>(objectId);

				if (target == null)
					return;

				target.SetMoveInfo(pos, angle, movePacket.Sync);
			}
			else 
			{
				TableObject tableObject = scene.objectManager.Find<TableObject>(objectId);
				if (tableObject == null)
					return;

				tableObject.SetMoveInfo(pos, angle, movePacket.Sync);
			}
		}
	}
	

	public static void S_InteractHandler(PacketSession session, IMessage packet)
	{
		S_Interact interactPacket = packet as S_Interact;

		GameScene scene = Managers.Instance.GetScene<GameScene>();
		if (scene == null)
			return;

		TableObject target = scene.objectManager.Find<TableObject>(interactPacket.ObjectId);
		if (target == null)
			return;

		Player p = scene.objectManager.Find<Player>(interactPacket.PlayerId);
		if (p == null)
			return;

		foreach (ObjectEvent e in interactPacket.Events)
		{
			target.DoEvent(e, p);
        }

	}

	public static void S_SyncHandler(PacketSession session, IMessage packet)
	{
		S_Sync syncPacket = packet as S_Sync;


		GameScene scene = Managers.Instance.GetScene<GameScene>();
		if (scene == null)
			return;

		// TODO : 모든 오브젝트의 정보를 넘기긱
	}


	public static void S_LeaveRoomHandler(PacketSession session, IMessage packet)
	{
		S_LeaveRoom leaveGamePacket = packet as S_LeaveRoom;

		Debug.Log("S_LeaveGameHandler");

		GameScene scene = Managers.Instance.GetScene<GameScene>();
		if (scene == null)
			return;

		scene.LeaveGame();
	}

	public static void S_RoomPlayerListHandler(PacketSession session, IMessage packet)
	{
		S_RoomPlayerList movePacket = packet as S_RoomPlayerList;

		Debug.Log("S_PlayerListHandler");
		GameUIManager uiManager = Managers.Instance.GetUIManager<GameUIManager>();
		if (uiManager == null)
			return;

		List<P_PlayerProfile> playerList = new List<P_PlayerProfile>();

		foreach (P_PlayerProfile player in movePacket.PlayerProfiles)
		{
			playerList.Add(player);
		}

		uiManager.UpdatePlayerList(playerList);
	}

	#endregion
}
