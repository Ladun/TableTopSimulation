using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using UnityEditor;

public class IntroUIManager : UIManager
{
    [SerializeField] private TMP_InputField playerNicknameInput;
    [SerializeField] private TMP_InputField serverIpInput;

    public TMP_Dropdown serverIpDropdown;
    private List<string> serverIps = new List<string>();

    public string playerNickName
    {
        get
        {
            return playerNicknameInput.text;
        }
    }

    public string currentServerIp
    {
        get
        {
            //if (serverIps.Count < 1)
            //    return "";

            //return serverIpDropdown.options[serverIpDropdown.value].text;

            return serverIpInput.text;
        }
    }

    private void Awake()
    {
        UpdateDropdownMenu();
#if UNITY_EDITOR
        serverIpInput.text = "127.0.0.1";
#endif
    }

    private void UpdateDropdownMenu()
    {
        serverIpDropdown.options.Clear();
        foreach(string serverIp in serverIps)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = serverIp;
            serverIpDropdown.options.Add(option);
        }
    }

    public void AddServerIp(TMP_InputField serverInput) 
    {
        bool b = CheckIPValid(serverInput.text);
        print(b);
        if (!b)
            return;
        serverIps.Add(serverInput.text);
        UpdateDropdownMenu();
    }
    public static bool CheckIPValid(string strIP)
    {
        if (string.IsNullOrEmpty(strIP))
            return false;

        IPAddress result = null;

        if (!IPAddress.TryParse(strIP, out result))
            return false;
        print(result.ToString());
        return true;
    }
}
