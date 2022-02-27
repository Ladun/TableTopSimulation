using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ContentBrowserContent : CustomButton, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Default Component")]
    public CustomData md;
    public int contentIdx;
    // Summary
    public TextMeshProUGUI nameText1;

    // Detail
    public TMP_InputField nameText2;
    public Image matTexture;
    public CustomButton deleteButton;

    // For drag
    private Transform _parent;
    private Vector2 offset;

    private void Start()
    {
        _parent = transform.parent;
        matTexture.GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.instance.OpenTextureFileBrowser(this);
        });
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = eventData.position - (Vector2)transform.position;
        transform.SetParent(transform.parent.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position - offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(_parent);
        UIManager.instance.contentBrowser.SetContentPosition(this, ((Vector2)transform.parent.position - eventData.position).y);
    }


    public void Setting(CustomData customMeshData, int idx, System.Action<CustomButton> deleteAction)
    {
        md = customMeshData;
        idx = contentIdx;

        SetText(md.go.name);
        for (int i = 0; i < md.meshRenderers.Length; i++)
        {
            // TODO: meshRenderer가 1개 이상일 때 UI에 이미지 추가하기
            Texture2D texture = md.GetTexture(i) as Texture2D;
            if (texture != null)
                SetTexture(texture);
        }

        deleteButton.onClick = deleteAction;
    }

    public void LoadTexture(int idx, string path)
    {
        StartCoroutine(Utils.CoLoadTexture(path, (texture) =>
        {
            md.SetTexture(idx, texture);
            SetTexture(texture);
        }));
    }

    public void SetTexture(Texture2D tex)
    {
        Rect rect = new Rect(0, 0, tex.width, tex.height);
        matTexture.sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
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
        sizeDelta.y = 195;

        GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}
