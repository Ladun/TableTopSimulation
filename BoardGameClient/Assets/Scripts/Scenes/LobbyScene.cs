using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    public LobbyBoard board;

    public RoomInfo selectedRoomInfo;

    [Header("Camera")]
    public SimpleCameraPositionController cpc;
    public Transform cameraMainPos;
    public Transform cameraBoardPos;

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Lobby; 

        Managers.Instance.SetScene(this);

        SendUpdateRoomList(0, "");
        SendUpdatePlayerList();

        SettingFinish = true;
    }

    public override void Clear()
    {

    }


    public void Exit()
    {
        Application.Quit(0);
    }

    public void LeaveLobby()
    {
        Managers.Instance.Scene.LoadScene("Intro", null, null); ;
    }

    public void EnterGame(RoomInfo info, ObjectInfo playerInfo, RepeatedField<CColor> colors)
    {
        Managers.Instance.Scene.LoadScene("Game",
            () => Managers.Instance.GetScene<GameScene>() != null,
            () =>
            {
                Managers.Instance.GetScene<GameScene>().Setting(info, playerInfo, colors);
            }); ;
    }

    #region Packet Send
    public void SendFile()
    {
        C_FileTransfer fileTransfer = new C_FileTransfer();
        fileTransfer.SendCode = 0;
        fileTransfer.Name = "Card.obj";
        Managers.Instance.Network.Send(fileTransfer);
    }

    public void SendChat(string text)
    {
        C_Chat chat = new C_Chat();
        chat.Chat = text;
        Managers.Instance.Network.Send(chat);
    }

    public void SendGoIntro()
    {
        //C_LeaveLobby leaveLobbyPacket = new C_LeaveLobby();
        //Managers.Instance.Network.Send(leaveLobbyPacket);

        Managers.Instance.Network.Disconnect();
        LeaveLobby();
    }

    public void SendUpdateRoomList(int roomId, string roomKeyword)
    {
        C_RoomList roomList = new C_RoomList();
        roomList.RoomId = roomId;
        roomList.RoomKeyword = roomKeyword;
        Managers.Instance.Network.Send(roomList);
    }

    public void SendUpdatePlayerList()
    {
        C_PlayerList playerList = new C_PlayerList();
        Managers.Instance.Network.Send(playerList);
    }

    // Reference: Lobby(Scene)->RoomMakePopup
    public void SendMakeRoom(RoomMakePopup rmpp)
    {
        C_MakeRoom makeRoomPacket = new C_MakeRoom();
        makeRoomPacket.Name = rmpp.GetRoomName();
        makeRoomPacket.MaxPlayers = rmpp.GetMaxPlayers();
        makeRoomPacket.Type = MapType.Circle;
        makeRoomPacket.V1 = 10;
        List<string> codes = rmpp.GetPackageCodes();
        for(int i = 0; i < codes.Count; i++)
        {
            makeRoomPacket.PacakgeCodes.Add(codes[i]);
        }

        Managers.Instance.Network.Send(makeRoomPacket);

        Managers.Instance.GetUIManager<LobbyUIManager>().OpenLoading("Waiting For Make Room...");
    }

    // Reference: Lobby(Scene)->RoomEnterPopup
    public void SendEnterRoom()
    {
        SendEnterRoom(selectedRoomInfo);
    }
    
    public void SendEnterRoom(RoomInfo roomInfo)
    {
        C_EnterRoom enterRoomPacket = new C_EnterRoom();
        enterRoomPacket.RoomInfo = roomInfo;
        Managers.Instance.Network.Send(enterRoomPacket);

        Managers.Instance.GetUIManager<LobbyUIManager>().OpenLoading("Waiting For Enter Room...", clear:(roomInfo == selectedRoomInfo));
    }
    #endregion

    #region Lobby Design

    public void UpdateRoomList(List<RoomInfo> items, List<int> playerCounts)
    {
        board.SettingRoomList(items, playerCounts);
    }

    public void CameraMoveToBoard()
    {
        MoveCamera(cameraBoardPos.position, cameraBoardPos.eulerAngles);
    }

    public void CameraMoveToMain()
    {
        MoveCamera(cameraMainPos.position, cameraMainPos.eulerAngles);
    }
    private void MoveCamera(Vector3 p, Vector3 a)
    {
        cpc.SetTargetPos(p, a);
    }
    #endregion

}
