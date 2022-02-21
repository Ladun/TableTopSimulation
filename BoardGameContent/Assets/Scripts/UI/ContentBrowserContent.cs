using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContentBrowserContent : CustomButton
{
    [Header("Default Component")]
    // Summary
    public TextMeshProUGUI nameText1;

    // Detail
    public TMP_InputField nameText2;
    public TextMeshProUGUI matTextureName;
    public Image matTexture;
    public CustomData md;

    private void Start()
    {
        matTexture.GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.instance.OpenTextureFileBrowser(this);
        });
    }

    public void Setting(CustomData customMeshData)
    {
        md = customMeshData;

        Texture2D texture = md.GetTexture() as Texture2D;
        if(texture != null)
            SetTexture(texture);
        SetText(md.meshRenderer.name);
    }

    public void LoadTexture(string path)
    {
        StartCoroutine(CoLoadTexture(path));
    }

    IEnumerator CoLoadTexture(string path)
    {
        // Start a download of the given URL
        using (WWW www = new WWW(path))
        {
            // Wait for download to complete
            yield return www;

            // assign texture
            md.meshRenderer.material.mainTexture = www.texture;
            SetTexture(www.texture);
        }
    }

    public void SetTexture(Texture2D tex)
    {
        Rect rect = new Rect(0, 0, tex.width, tex.height);
        matTexture.sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
        matTextureName.text = tex.name;
    }

    public void NameChanged(string text)
    {
        md.meshRenderer.name = text;
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
        sizeDelta.y = 195;

        GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}
