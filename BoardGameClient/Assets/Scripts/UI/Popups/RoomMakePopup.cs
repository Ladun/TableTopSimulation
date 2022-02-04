using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Google.Protobuf.Protocol;

public class RoomMakePopup : MonoBehaviour
{
    private TMP_InputField roomName;
    private TMP_InputField maxPlayers;

    private void Awake()
    {
        roomName = transform.Find("RoomName").GetComponent<TMP_InputField>();
        maxPlayers = transform.Find("MaxPlayers").GetComponent<TMP_InputField>();
    }
    
    public string GetRoomName()
    {
        return roomName.text;
    }
    public int GetMaxPlayers()
    {
        return int.Parse(maxPlayers.text);
    }

    public void ValidateMaxPlayer()
    {
        if(string.IsNullOrEmpty(maxPlayers.text))
        {
            maxPlayers.text = "3";
        }
    }

}
