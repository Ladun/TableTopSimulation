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
    public ColliderBrowser colliderBrowser;
    public CustomContentBrowser contentBrowser;
    public AcceptWindow acceptWindow;

    public void UpdateContentBrowser(string packageName, string packageVersion)
    {
        contentBrowser.packageName.text = packageName;
        contentBrowser.packageVersion.text = packageVersion;

        contentBrowser.UpdateBrowser();
    }

    public void OpenAcceptWindow(System.Action acceptAction, System.Action cancleAction)
    {
        acceptWindow.Open(acceptAction, cancleAction);
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
                contentBrowser.Add(path);
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
                StoreManager.instance.Save(contentBrowser.packageName.text, contentBrowser.packageVersion.text, path);
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
