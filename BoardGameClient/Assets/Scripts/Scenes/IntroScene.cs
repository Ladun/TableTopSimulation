using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroScene : BaseScene
{

    private IntroUIManager uiManager
    {
        get
        {
            return (IntroUIManager)baseUIManager;
        }
    }

    public Queue<System.Action> introDelayAction = new Queue<System.Action>();

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Intro;
        Managers.Instance.SetScene(this);

        SettingFinish = true;
    }

    public override void Clear()
    {

    }
    private void Update()
    {
        while (introDelayAction.Count > 0)
        {
            System.Action act = introDelayAction.Dequeue();
            act.Invoke();
        }
    }

    public void Exit()
    {
        Application.Quit(0);
    }
    public void EnterLobby()
    {
        Managers.Instance.Scene.LoadScene("Lobby", null, null); ;
    }

    public void FailedToConnect()
    {
        uiManager.CloseLoading();
    }

    #region Send Packet

    public void ConnectToServer()
    {
        string serverIp = uiManager.currentServerIp;
        if (string.IsNullOrEmpty(serverIp))
            return;

        if (Managers.Instance.Network.Connect(serverIp))
        {
            uiManager.OpenLoading("Connect To Server...");
        }
    }

    public void SendLogin()
    {
        C_Login login = new C_Login();
        string playerName = uiManager.playerNickName;

        if (string.IsNullOrEmpty(playerName))
            login.Name = "";
        else
            login.Name = playerName;

        Managers.Instance.Network.Send(login);
    }

    #endregion

}
