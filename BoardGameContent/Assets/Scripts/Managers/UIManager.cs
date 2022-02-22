using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
        fileBrowser.Open(
            (path) => {
                Regex reg = new Regex(@"[\w]*\.obj$");
                return reg.IsMatch(path);              
            }, 
            (path) =>
            {
                ContentManager.instance.AddObj(path);
                contentBrowser.UpdateBrowser();
            }
        );
    }
    public void OpenTextureFileBrowser(ContentBrowserContent cbc)
    {
        fileBrowser.Open(
            (path) => {
                Regex reg = new Regex(@"[\w]*\.(png|jpg|jpeg)$");
                return reg.IsMatch(path);
            }, 
            (path) =>
            {
                cbc.LoadTexture(0, path);
            }
        );
    }

    public void OpenSaveFileBrowser()
    {
        fileBrowser.Open(
            (path) => {
                return false;//File.GetAttributes(path).HasFlag(FileAttributes.Directory);
            }, 
            (path) =>
            {
                StoreManager.instance.Save(path);
            }, CustomFileBrowser.SelectType.Directory);
    }
    public void OpenLoadFileBrowser()
    {
        fileBrowser.Open(
            (path) => {
                Regex reg = new Regex(@"[\w]*package\.json$");
                return reg.IsMatch(path);
            },
            (path) =>
            {
                StoreManager.instance.Load(path);
            });
    }
}
