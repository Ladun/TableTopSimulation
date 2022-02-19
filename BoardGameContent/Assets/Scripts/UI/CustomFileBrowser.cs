using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Text.RegularExpressions;

public class CustomFileBrowser : MonoBehaviour
{
    [Header("Default Components")]
    public FileBrowserContent contentPrefab;
    public TMP_InputField currentPathText;
    public TMP_InputField findFilter;
    public Transform contentParent;
    private List<FileBrowserContent> contents;

    [Header("Directory Options")]
    public Color directoryTextColor;

    private void Start()
    {
        gameObject.SetActive(false);
        SetPath(Application.dataPath);
    }
    
    // Reference: FileBrowser -> CurrentPath -> Home
    public void SetPathToHome()
    {
        SetPath(Application.dataPath);
    }

    public void SetPath(string path)
    {
        currentPathText.text = path;
        UpdateFileBrowser();
    }

    public void UpdateFileBrowser()
    {
        Regex reg = new Regex(findFilter.text);

        int idx = 0;
        DirectoryInfo parentPath = Directory.GetParent(currentPathText.text);
        if (parentPath != null)
        {
            FileBrowserContent content = GetContentObject(idx++);
            content.Setting(parentPath.FullName, "..");
        }

        string[] dirs = Directory.GetDirectories(currentPathText.text);
        _UpdateContents(ref idx, dirs, reg);

        string[] files = Directory.GetFiles(currentPathText.text);
        _UpdateContents(ref idx, files, reg);

        for (; idx < contentParent.childCount; idx++)
            contentParent.GetChild(idx).gameObject.SetActive(false);
    }

    private void _UpdateContents(ref int idx, string[] paths, Regex reg)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            if (!string.IsNullOrEmpty(findFilter.text) && !reg.IsMatch(paths[i]))
                continue;

            FileBrowserContent content = GetContentObject(idx++);

            content.gameObject.SetActive(true);
            content.Setting(paths[i]);
        }
    }

    private FileBrowserContent GetContentObject(int idx)
    {
        FileBrowserContent content;
        if (idx < contentParent.childCount)
        {
            content = contentParent.GetChild(idx).GetComponent<FileBrowserContent>();
        }
        else
        {
            content = Instantiate(contentPrefab);
            content.transform.SetParent(contentParent);
            content.onClick = (CustomButton b) =>
            {
                FileBrowserContent fb = b as FileBrowserContent;

                if (File.GetAttributes(fb.originPath).HasFlag(FileAttributes.Directory))
                {
                    SetPath(fb.originPath);
                }
                else
                {
                    if (fb.originPath.EndsWith(".obj"))
                    {
                        ContentManager.instance.AddObj(fb.originPath);
                    }
                }

            };
        }
        return content;
    }


    // Reference: OpenFileBrowser
    public void OpenClose()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            UpdateFileBrowser();
        }
    }

}
