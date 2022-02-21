using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region SingleTon
    private static UIManager _instance;
    public static UIManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
            }
            return _instance;
        }
    }
    #endregion

    public CustomFileBrowser fileBrowser;
    public CustomContentBrowser contentBrowser;

    public void UpdateContentBrowser()
    {
        contentBrowser.UpdateBrowser();
    }

    public void OpenObjFileBrowser()
    {
        fileBrowser.Open(@"[\w]*\.obj$", (path) =>
        {
            ContentManager.instance.AddObj(path);
            contentBrowser.UpdateBrowser();
        });
    }
    public void OpenTextureFileBrowser(ContentBrowserContent cbc)
    {
        fileBrowser.Open(@"[\w]*\.(png|jpg|jpeg)$", (path) =>
        {
            cbc.LoadTexture(path);
        });
    }
}
