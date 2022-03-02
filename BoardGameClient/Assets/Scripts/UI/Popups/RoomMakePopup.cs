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

    public Transform packageListPrefab;
    public Transform packageListParent;

    private void Awake()
    {
        roomName = transform.Find("RoomName").GetComponent<TMP_InputField>();
        maxPlayers = transform.Find("MaxPlayers").GetComponent<TMP_InputField>();
    }

    private void OnEnable()
    {
        int idx = 0;
        foreach(var key in Managers.Instance.Package.packageCodes.Keys)
        {
            Transform packageListContent = null;
            if(idx < packageListParent.childCount)
            {
                packageListContent = packageListParent.GetChild(idx);
            }
            else
            {
                packageListContent = Instantiate(packageListPrefab);
                packageListContent.SetParent(packageListParent);
                packageListContent.localScale = Vector3.one;
            }

            packageListContent.gameObject.SetActive(true);

            // Settings
            PackageManager.StoreData data = Managers.Instance.Package.packageCodes[key];
            packageListContent.GetChild(0).GetComponent<TextMeshProUGUI>().text = key;

            idx++;
        }
        for (; idx < packageListParent.childCount; idx++)
        {
           packageListParent.GetChild(idx).gameObject.SetActive(false);
        }
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

    public List<string> GetPackageCodes()
    {
        List<string> codes = new List<string>();
        for (int i = 0; i < packageListParent.childCount; i++)
        {
            Transform c = packageListParent.GetChild(i);
            if (c.GetComponentInChildren<CheckBox>().IsChecked)
            {
                codes.Add(c.GetChild(0).GetComponent<TextMeshProUGUI>().text);
            }
        }

        return codes;
    }

}
