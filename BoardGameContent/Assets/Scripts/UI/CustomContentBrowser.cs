using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomContentBrowser : MonoBehaviour
{
    [Header("Default Components")]
    public ContentBrowserContent contentPrefab;
    public Transform contentParent;
    private List<ContentBrowserContent> _contents = new List<ContentBrowserContent>();

    public TMP_InputField packageName;
    public TMP_InputField packageVersion;

    public void UpdateBrowser()
    {

        int idx = 0;
        for (int i = 0; i < ContentManager.instance.data.Count; i++)
        {
            ContentBrowserContent content = GetContentObject(idx++);

            int _i = i;
            content.gameObject.SetActive(true);
            content.Setting(ContentManager.instance.data[i], i, (btn)=>
            {
                ContentBrowserContent cbc = btn as ContentBrowserContent;

                ContentManager.instance.DeleteObj(cbc.contentIdx);
                UIManager.instance.contentBrowser.UpdateBrowser();
            });
        }

        for (; idx < contentParent.childCount; idx++)
            contentParent.GetChild(idx).gameObject.SetActive(false);
    }

    public void SetContentPosition(ContentBrowserContent cbc, float posY) 
    {
        int i = 0;
        float cumulativeHeight = 0;
        for(; i < contentParent.childCount; i++)
        {
            RectTransform rt = contentParent.GetChild(i).GetComponent<RectTransform>();
            cumulativeHeight += rt.sizeDelta.y;
            if (posY <= cumulativeHeight) 
            {
                break;
            }
        }

        cbc.transform.SetSiblingIndex(i);
    }


    private ContentBrowserContent GetContentObject(int idx)
    {
        ContentBrowserContent content;
        if (idx < _contents.Count)
        {
            content = _contents[idx];
        }
        else
        {
            content = Instantiate(contentPrefab);
            content.transform.SetParent(contentParent); 
            content.onClick = (CustomButton b) =>
            {
                UIManager.instance.contentBrowser.ActiveData(idx);
                ContentManager.instance.ActiveData(idx);
            };
            _contents.Add(content);
        }
        return content;
    }

    private void ActiveData(int idx)
    {
        for(int i = 0; i < _contents.Count; i++)
        {
            _contents[i].Deactive();
        }
        _contents[idx].Active();
    }

    // Reference: ContentBrowser -> Close Button
    public void OpenClose()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
