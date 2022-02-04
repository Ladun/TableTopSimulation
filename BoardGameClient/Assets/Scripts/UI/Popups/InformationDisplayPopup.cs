using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Google.Protobuf.Protocol;

public class InformationDisplayPopup : MonoBehaviour
{

    private TextMeshProUGUI inform;
    private CanvasGroup cg;
    private float time;
    public float maxTime=1f;

    private void Awake()
    {
        inform = transform.Find("Inform").GetComponent<TextMeshProUGUI>();
        cg = GetComponent<CanvasGroup>();

    }

    private void Update()
    {
        if(time > 0)
        {
            time -= Time.deltaTime;
            cg.alpha = time / maxTime;
        }
        else if(time <= 0)
        {
            Managers.Instance.GetUIManager<UIManager>().ClosePopupItem(transform);
        }
    }

    public void Setting(string text)
    {
        inform.text = text;
        time = maxTime;
        cg.alpha = 1;
    }
}
