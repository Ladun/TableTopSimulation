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

    public Transform packageListPrefab;
    public Transform packageListParent;


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


        int idx = 0;
        foreach (var key in roomInfo.PackageCodes)
        {
            Transform packageListContent = null;
            if (idx < packageListParent.childCount)
            {
                packageListContent = packageListParent.GetChild(idx);
            }
            else
            {
                packageListContent = Instantiate(packageListPrefab);
                packageListContent.SetParent(packageListParent);
                packageListContent.localScale = Vector3.one;
                packageListContent.GetChild(1).GetComponent<Button>().interactable = false;
            }

            packageListContent.gameObject.SetActive(true);

            // Settings
            packageListContent.GetChild(0).GetComponent<TextMeshProUGUI>().text = key;
            if (Managers.Instance.Package.HasPackage(key))
                packageListContent.GetComponentInChildren<CheckBox>().IsChecked = true;
            else
                packageListContent.GetComponentInChildren<CheckBox>().IsChecked = false;


            idx++;
        }
        for (; idx < packageListParent.childCount; idx++)
        {
            packageListParent.GetChild(idx).gameObject.SetActive(false);
        }
    }

}
