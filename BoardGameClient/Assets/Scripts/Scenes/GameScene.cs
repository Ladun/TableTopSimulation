using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    public ObjectManager objectManager { get; } = new ObjectManager();
    private GameUIManager uiManager
    {
        get { return baseUIManager as GameUIManager; }
    }

    private RoomInfo _roomInfo;

    public Player myPlayer;
    public CameraController cam;

    public List<Color> colors { get; } = new List<Color>();

    protected override void Init()
    {
        base.Init();

        Debug.Log("GameScene Loaded");
        SceneType = Define.Scene.Game;
        Managers.Instance.SetScene(this);

        myPlayer.gameObject.SetActive(true);
    }

    public override void Clear()
    {

    }


    public void Exit()
    {
        Application.Quit(0);
    }

    public void Setting(RoomInfo roomInfo, ObjectInfo playerInfo, RepeatedField<CColor> colors)
    {
        this._roomInfo = roomInfo;

        // Get Players Color
        foreach (CColor c in colors)
        {
            this.colors.Add(new Color(c.R / 255f, c.G / 255f, c.B / 255f));
        }

        // Player Setting
        myPlayer.gameObject.SetActive(true);
        objectManager.AddPlayer(myPlayer, playerInfo);

        // Camera Setting
        cam.Init(Utils.Dim3Info2Vector3(playerInfo.Pos));
        uiManager.LoadAllGames(roomInfo.PackageCodes);

        SettingFinish = true;
    }
    public void LeaveGame()
    {
        Managers.Instance.Scene.LoadScene("Lobby", null, null); 
    }

    #region Packet Send

    public void SendPlayerList()
    {
        C_RoomPlayerList playerList = new C_RoomPlayerList();
        Managers.Instance.Network.Send(playerList);
    }

    public void SendGoLobby()
    {
        C_LeaveRoom leaveRoomPacket = new C_LeaveRoom();


        Managers.Instance.Network.Send(leaveRoomPacket);
    }

    public void SendSpawnObject(string objName, string packageCode, GameObjectType type)
    {
        Vector3 pos = cam.GetUpperTablePosition() + Vector3.up * 0.1f;

        C_Spawn spawnPacket = new C_Spawn();
        spawnPacket.Name = objName;
        spawnPacket.PackageCode = packageCode;
        spawnPacket.Pos = Utils.Vector32Dim3Info(pos);
        spawnPacket.Angle = Utils.Vector32Dim3Info(new Vector3(0, cam.transform.eulerAngles.y, 0));
        spawnPacket.ObjectType = type;

        Managers.Instance.Network.Send(spawnPacket);
    }

    public void SendDespawnObject(int objId)
    {
        C_Despawn despawnPacket = new C_Despawn();
        
        despawnPacket.ObjectIds.Add(objId);

        Managers.Instance.Network.Send(despawnPacket);

    }

    private void SendObjectEvent(int objectId, TableObjectEventType eventType, int v, int flag = 0)
    {
        C_Interact interactPacket = new C_Interact();
        interactPacket.ObjectId = objectId;

        ObjectEvent objectEvent = new ObjectEvent();
        objectEvent.ObjectEventId = eventType;
        objectEvent.ObjectValue = v;
        objectEvent.Flag = flag;
        //to.DoEvent(objectEvent, myPlayer);

        interactPacket.Events.Add(objectEvent);

        Managers.Instance.Network.Send(interactPacket);
    }

    public void SendLockObject(TableObject to, bool lockValue)
    {
        SendObjectEvent(to.Id, TableObjectEventType.Lock, lockValue ? 1 : 0);
    }
    public void SendPickObject(TableObjectSet to)
    {
        SendObjectEvent(to.Id, TableObjectEventType.Pick, 0, 1);
    }
    public void SendShuffleObject(TableObject to)
    {
        SendObjectEvent(to.Id, TableObjectEventType.Shuffle, 0);
    }

    #endregion


}
