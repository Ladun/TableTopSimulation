using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBoard : MonoBehaviour
{
    public GameObject rightPageButton;
    public GameObject leftPageButton;

    public Transform postisParent;


    private List<RoomInfo> roomInfos;
    private List<int> playerCounts;
    private int pageCount;
    private int currentPageIndex;

    public void SendUpdateRoomList()
    {
        int roomId = 0;
        string roomKeyword = "";

        Managers.Instance.GetScene<LobbyScene>().SendUpdateRoomList(roomId, roomKeyword);
    }

    public void SettingRoomList(List<RoomInfo> items, List<int> playerCounts)
    {
        roomInfos = items;
        this.playerCounts = playerCounts;
        pageCount = roomInfos.Count / 9 + 1;
        currentPageIndex = 0;
        ViewRoomInfo();
        SetPageButton();
    }

    private void ViewRoomInfo()
    {
        for (int i = 0; i < 9; i++)
        {
            int _i = currentPageIndex * 9 + i;
            Transform t = postisParent.GetChild(i);
            if (_i < roomInfos.Count)
            {
                RoomPostit rp = t.GetComponent<RoomPostit>();
                rp.Setting(roomInfos[_i], playerCounts[_i]);

                t.gameObject.SetActive(true);
            }
            else
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    public void Next()
    {
        currentPageIndex = (currentPageIndex + 1) % pageCount;
        ViewRoomInfo();
        SetPageButton();
    }

    public void Prev()
    {

        currentPageIndex = (currentPageIndex - 1 + pageCount) % pageCount;
        ViewRoomInfo();
        SetPageButton();
    }

    private void SetPageButton()
    {
        if(currentPageIndex + 1 < pageCount)
            rightPageButton.SetActive(true);
        else
            rightPageButton.SetActive(false);

        if (currentPageIndex - 1 > 0)
            leftPageButton.SetActive(true);
        else
            leftPageButton.SetActive(false);
    }

}
