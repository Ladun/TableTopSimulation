using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContentBrowserContent : CustomButton
{
    [Header("Default Component")]
    public TextMeshProUGUI nameText1;
    public TMP_InputField nameText2;
    public CustomData md;

    public void Setting(CustomData customMeshData)
    {
        md = customMeshData;
        SetText(md.go.name);
    }

    public void NameChanged(string text)
    {
        md.go.name = text;
        SetText(text);
    }

    private void SetText(string text)
    {
        nameText1.text = text;
        nameText2.text = text;
    }

    public void Deactive()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);

        Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
        sizeDelta.y = 50;

        GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }

    public void Active()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
        sizeDelta.y = 155;

        GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}
