using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomContentBrowser : MonoBehaviour
{
    public class WrappedData
    {
        public bool IsDirectory { get { return data == null; } }
        public string dirName;

        //
        public CustomData data;
        public int dataIdx;

        public string GetText()
        {
            if (data == null)
                return dirName;
            else
                return data.go.name;
        }

        public void SetText(string text)
        {
            if (data == null)
                dirName = text;
            else
                data.go.name =text;
        }
    }

    [Header("Default Components")]
    public ContentBrowserContent contentPrefab;
    public Transform contentParent;
    private List<ContentBrowserContent> _contents = new List<ContentBrowserContent>();

    public TMP_InputField packageName;
    public TMP_InputField packageVersion;

    private List<WrappedData> wrappedData = new List<WrappedData>();
    public List<WrappedData> WrappedDatas { get { return wrappedData; } }

    public void UpdateBrowser()
    {

        int idx = 0;
        for (int i = 0; i < wrappedData.Count; i++)
        {
            ContentBrowserContent content = GetContentObject(idx++);

            content.gameObject.SetActive(true);
            content.Setting(wrappedData[i], i, (btn)=>
            {
                ContentBrowserContent cbc = btn as ContentBrowserContent;

                UIManager.instance.contentBrowser.Delete(cbc.contentIdx);
            });
        }

        for (; idx < contentParent.childCount; idx++)
            contentParent.GetChild(idx).gameObject.SetActive(false);
    }

    // Reference: ContentBrowser -> Add Directory
    public void Add(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            wrappedData.Add(new WrappedData() { data =null, dirName="Directory" });
        }
        else
        {
            wrappedData.Add(new WrappedData() { data = ContentManager.instance.AddObj(path), dataIdx=ContentManager.instance.data.Count - 1 });
        }
        UpdateBrowser();
    }

    public void Delete(int idx)
    {
        // TODO: 오브젝트 삭제, 디렉토리 소속, 등등
        if (wrappedData[idx].IsDirectory)
        {
        }
        else
        {
            ContentManager.instance.DeleteObj(wrappedData[idx].dataIdx);
        }
        wrappedData.RemoveAt(idx);
        UIManager.instance.contentBrowser.UpdateBrowser();
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
                if (!wrappedData[idx].IsDirectory)
                {
                    ContentManager.instance.ActiveData(wrappedData[idx].dataIdx);
                    UIManager.instance.colliderBrowser.Open();
                }
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
