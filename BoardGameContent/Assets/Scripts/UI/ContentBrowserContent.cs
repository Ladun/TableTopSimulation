using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ContentBrowserContent : CustomButton, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Default Component")]
    public CustomContentBrowser.WrappedData wd;
    public int contentIdx { get; set; }
    // Summary
    public TextMeshProUGUI nameText1;

    // Detail
    public TMP_InputField nameText2;
    public Image matTexture;
    public CustomButton colliderButton;
    public CustomButton deleteButton;

    // For drag
    private Transform _parent;
    private Vector2 offset;
    private bool isDrag;

    private void Start()
    {
        _parent = transform.parent;
        matTexture.GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.instance.OpenTextureFileBrowser(this);
        });

        colliderButton.onClick = (btn) =>
        {
            UIManager.instance.colliderBrowser.Open();
        };
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if(!isDrag)
            if (onClick != null)
                onClick.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = eventData.position - (Vector2)transform.position;
        transform.SetParent(transform.parent.parent);
        isDrag = true;
        UIManager.instance.contentBrowser.OpenDragViewer();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position - offset;
        UIManager.instance.contentBrowser.UpdateDragViewerPos(((Vector2)transform.parent.position - eventData.position).y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(_parent);
        UIManager.instance.contentBrowser.SetContentPosition(this, ((Vector2)transform.parent.position - eventData.position).y);
        isDrag = false;
        UIManager.instance.contentBrowser.CloseDragViewer();
    }


    public void Setting(CustomContentBrowser.WrappedData wrappedData, int idx, System.Action<CustomButton> deleteAction)
    {
        transform.GetChild(1).gameObject.SetActive(false);
        wd = wrappedData;
        contentIdx = idx;

        SetText(wd.GetText());
        if (!wd.IsDirectory)
        {
            for (int i = 0; i < wd.data.meshRenderers.Length; i++)
            {
                // TODO: meshRenderer가 1개 이상일 때 UI에 이미지 추가하기
                Texture2D texture = wd.data.GetTexture(i) as Texture2D;
                if (texture != null)
                    SetTexture(texture);
            }
        }

        deleteButton.onClick = deleteAction;
    }

    public void LoadTexture(int idx, string path)
    {
        StartCoroutine(Utils.CoLoadTexture(path, (texture) =>
        {
            wd.data.SetTexture(idx, texture);
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
        wd.SetText(text);
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
        nameText2.ActivateInputField();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);

        if (wd.IsDirectory)
        {
            transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
            Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
            sizeDelta.y = 195;

            GetComponent<RectTransform>().sizeDelta = sizeDelta;
        }
    }
}
