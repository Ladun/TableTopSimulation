using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomContentBrowser : MonoBehaviour
{
    [Header("Default Components")]
    public ContentBrowserContent contentPrefab;
    public Transform contentParent;
    private List<ContentBrowserContent> contents = new List<ContentBrowserContent>();

    public void UpdateBrowser()
    {

        int idx = 0;
        for (int i = 0; i < ContentManager.instance.data.Count; i++)
        {
            ContentBrowserContent content = GetContentObject(idx++);

            content.gameObject.SetActive(true);
            content.Setting(ContentManager.instance.data[i]);
        }

        for (; idx < contentParent.childCount; idx++)
            contentParent.GetChild(idx).gameObject.SetActive(false);
    }


    private ContentBrowserContent GetContentObject(int idx)
    {
        ContentBrowserContent content;
        if (idx < contents.Count)
        {
            content = contents[idx];
        }
        else
        {
            content = Instantiate(contentPrefab);
            content.transform.SetParent(contentParent); 
            content.onClick = (CustomButton b) =>
            {
                print(idx);
                UIManager.instance.contentBrowser.ActiveData(idx);
                ContentManager.instance.ActiveData(idx);
            };
            contents.Add(content);
        }
        return content;
    }

    private void ActiveData(int idx)
    {
        for(int i = 0; i < contents.Count; i++)
        {
            contents[i].Deactive();
        }
        contents[idx].Active();
    }
}
