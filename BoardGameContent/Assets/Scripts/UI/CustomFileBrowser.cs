using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Text.RegularExpressions;

public class CustomFileBrowser : MonoBehaviour
{
    public enum SelectType { File, Directory }

    [Header("Default Components")]
    public FileBrowserContent contentPrefab;
    public TMP_InputField currentPathText;
    public TMP_InputField findFilter;
    public Transform contentParent;
    private List<FileBrowserContent> contents = new List<FileBrowserContent>();

    [Header("Directory Options")]
    public Color directoryTextColor;

    // Selection
    private List<FileBrowserContent> selected = new List<FileBrowserContent>();
    private System.Action<string> _selectedAction;
    private System.Func<string, bool> _filter;
    private SelectType _selectType;

    private void Start()
    {
        gameObject.SetActive(false);
        ValueClear();

        SetPathToHome();
    }

    private void ValueClear()
    {
        _filter = null;
        _selectedAction = null;
        selected.Clear();
        _selectType = SelectType.File;
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
            if (!File.GetAttributes(paths[i]).HasFlag(FileAttributes.Directory) &&
                _filter != null && !_filter.Invoke(paths[i]))
                continue;

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
                FileBrowserContent fb = b as FileBrowserContent;
                fb.selected = !fb.selected;

                if (File.GetAttributes(fb.originPath).HasFlag(FileAttributes.Directory))
                {
                    SetPath(fb.originPath);
                }
                else
                {
                    if (fb.selected)
                    {
                        selected.Add(fb);
                    }
                    else
                    {
                        selected.Remove(fb);
                    }
                }

            };
            contents.Add(content);
        }
        return content;
    }

    public void Open(System.Func<string, bool> filter, System.Action<string> selectedAction, SelectType selectType=SelectType.File)
    {
        _selectedAction = selectedAction;
        _filter = filter;
        _selectType = selectType;

        gameObject.SetActive(true);
        UpdateFileBrowser();
    }

    public void Close()
    {
        ValueClear();
        gameObject.SetActive(false);
    }

    // FileBrowser -> Selected
    public void Selected()
    {
        UIManager.instance.OpenAcceptWindow(() =>
        {
            if (_selectedAction != null)
            {
                if (_selectType == SelectType.File)
                {
                    for (int i = 0; i < selected.Count; i++)
                    {
                        _selectedAction.Invoke(selected[i].originPath);
                    }
                }
                else
                {
                    _selectedAction.Invoke(currentPathText.text);
                }
            }
            ValueClear();
            gameObject.SetActive(false);
        }, null);
    }


}
