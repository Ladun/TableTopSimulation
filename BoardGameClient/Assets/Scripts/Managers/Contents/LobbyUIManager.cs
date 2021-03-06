using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;
using Google.Protobuf.Collections;
using UnityEngine.UI;

public class LobbyUIManager : UIManager
{

    public UIListContentsInfo<UIGridListViewer, P_PlayerProfile> playerContent;
    public List<Transform> popupList;
    public ChatUI chatUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InteractPopupItem("Profile");
            Managers.Instance.GetScene<LobbyScene>().SendUpdatePlayerList();
        }
    }


    public void UpdatePlayerList(List<P_PlayerProfile> items)
    {
        playerContent.parent.UpdateItemList(playerContent.prefab, items);
    }

    public void InteractPopupItem(string popupName)
    {
        Transform popup = popupList.Find(x => x.name.Equals(popupName));
        if (popup != null)
        {
            InteractPopupItem(popup);
        }
    }

    public Transform OpenPopupItem(string popupName)
    {
        Transform popup = popupList.Find(x => x.name.Equals(popupName));
        if(popup != null)
        {
            OpenPopupItem(popup);
        }

        return popup;
    }

    public void ClosePopupItem(string popupName)
    {
        Transform popup = popupList.Find(x => x.name.Equals(popupName));
        if (popup != null)
        {
            ClosePopupItem(popup);
        }
    }

    public void UpdateChatting(S_Chat chat)
    {
        chatUI.AddChat(chat.Chat);
    }

}
