using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class ChatUI : MonoBehaviour
{
    public TextMeshProUGUI board;
    public TMP_InputField inputText;
    public int maxLine = 20;
    public Transform cover;

    private List<string> chatList = new List<string>();
    private CanvasGroup cg;

    public bool IsChat { get { return inputText.isFocused; } }

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        board.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(inputText.text.Length > 0)
                SendChat(inputText);
            inputText.ActivateInputField();
        }

        cover.gameObject.SetActive(!inputText.isFocused);
        cg.alpha = inputText.isFocused ? 1 : 0.5f;

    }

    // Reference: transform -> Cover
    public void SetFocus()
    {
        inputText.ActivateInputField();
    }

    // Reference: transform -> Input -> Send Button
    public void SendChat(TMP_InputField inputf)
    {
        Managers.Instance.GetScene<LobbyScene>().SendChat(inputf.text);
        inputText.text = "";
        inputText.ActivateInputField();
    }

    public string GetChat()
    {
        StringBuilder sb = new StringBuilder();

        for(int i = 0; i < chatList.Count; i++)
        {
            sb.AppendLine(chatList[i]);
        }

        return sb.ToString();
    }

    public void AddChat(string chat)
    {
        if(chatList.Count >= maxLine)
        {
            chatList.RemoveAt(0);
        }

        chatList.Add(chat);

        board.text = GetChat();
    }
}
