using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Protobuf.Protocol;

public class UIPlayerListContent : UIListContent<P_PlayerProfile>
{
    public TextMeshProUGUI playerId;
    public TextMeshProUGUI playerName;

    public override void Init()
    {

        base.Init();
    }

    public override void Setting(P_PlayerProfile item)
    {
        base.Setting(item);

        playerId.text = (item.Id & 0x00FFFFFF).ToString("X6");
        playerName.text = item.Name;


    }
}
