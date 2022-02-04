using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Google.Protobuf.Protocol;


[RequireComponent(typeof(Outline))]
public class RoomPostit : MonoBehaviour
{
    private RoomInfo item;

    [SerializeField]private TextMeshProUGUI roomId;
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI roomPopulation;
    [SerializeField] private Button enterButton;

    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        enterButton.onClick.AddListener(() =>
        {
            if (item != null)
            {
                Managers.Instance.GetScene<LobbyScene>().selectedRoomInfo = item;
                Managers.Instance.GetUIManager<LobbyUIManager>().OpenPopupItem("RoomEnterPopup");
            }
        });

    }

    public void Setting(RoomInfo item, int currentPlayer)
    {
        this.item = item;

        roomId.text = item.RoomId.ToString();
        roomName.text = item.Name;
        roomPopulation.text = currentPlayer + "/" + item.MaxPlayers;
    }

    private void OnMouseEnter()
    {
        outline.enabled = true;
    }

    private void OnMouseExit()
    {
        outline.enabled = false;
    }
}
