using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Protobuf.Protocol;

public class UIRoomListContent : UIListContent<RoomInfo>
{
    private TextMeshProUGUI roomId;
    private TextMeshProUGUI roomName;
    private Button enterButton;

    public override void Init()
    {
        roomId = transform.Find("RoomId").GetComponent<TextMeshProUGUI>();
        roomName = transform.Find("RoomName").GetComponent<TextMeshProUGUI>();
        enterButton = transform.Find("Enter").GetComponent<Button>();

        enterButton.onClick.AddListener(() =>
        {
            if (item != null)
            {
                Managers.Instance.GetScene<LobbyScene>().selectedRoomInfo = item;
                Managers.Instance.GetUIManager<LobbyUIManager>().OpenPopupItem("RoomEnterPopup");
            }
        });

        base.Init();
    }

    public override void Setting(RoomInfo item)
    {
        base.Setting(item);

        roomId.text = item.RoomId.ToString();
        roomName.text = item.Name;


    }
}
