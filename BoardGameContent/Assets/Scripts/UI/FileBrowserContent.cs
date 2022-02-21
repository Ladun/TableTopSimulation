using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class FileBrowserContent : CustomButton
{
    [Space(10)]
    public string originPath;
    public string lastPath;

    public Color selectedColor;
    public bool selected;

    public void Setting(string path, string displayPath=null)
    {
        // Custom Button setting
        selected = false;
        graphic.color = normalColor;

        // FileBrowserContent Setting
        originPath = path;
        if (string.IsNullOrEmpty(displayPath))
        {
            string[] splited = path.Split('\\');
            lastPath = splited[splited.Length - 1];
        }
        else
        {
            lastPath = displayPath;
        }

        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = lastPath;
    }

    protected override Color GetNormalColor()
    {
        if (selected)
            return selectedColor;
        return normalColor;
    }
}
