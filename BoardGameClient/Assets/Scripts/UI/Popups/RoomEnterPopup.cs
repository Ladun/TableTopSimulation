using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Google.Protobuf.Protocol;

public class RoomEnterPopup : MonoBehaviour
{
    private TextMeshProUGUI roomId;
    private TextMeshProUGUI roomName;


    private void Awake()
    {
        roomId = transform.Find("RoomId").GetComponent<TextMeshProUGUI>();
        roomName = transform.Find("RoomName").GetComponent<TextMeshProUGUI>();

    }
    private void OnEnable()
    {
        RoomInfo roomInfo = Managers.Instance.GetScene<LobbyScene>().selectedRoomInfo;
        roomId.text = roomInfo.RoomId.ToString();
        roomName.text = roomInfo.Name;
    }

}
