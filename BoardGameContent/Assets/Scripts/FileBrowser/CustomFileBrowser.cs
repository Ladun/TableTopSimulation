using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Text.RegularExpressions;

public class CustomFileBrowser : MonoBehaviour
{
    public Button contentPrefab;

    public TMP_InputField currentPathText;
    public TMP_InputField findFilter;

    public Transform contentParent;
    private List<Button> contents;

    private void Start()
    {
        SetPathToHome();
    }

    public void SetPathToHome()
    {
        currentPathText.text = Application.dataPath;
    }

    public void UpdateFileBrowser()
    {
        Regex reg = new Regex(findFilter.text);

        string[] paths = Directory.GetFiles(currentPathText.text);
        int idx = 0;
        for(int i = 0; i < paths.Length; i++)
        {
            if (!string.IsNullOrEmpty(findFilter.text) && !reg.IsMatch(paths[i]))
                continue;

            Button content;
            if (idx < contentParent.childCount)
            {
                content = contentParent.GetChild(idx).GetComponent<Button>();
            }
            else
            {
                content = Instantiate(contentPrefab);
                content.transform.SetParent(contentParent);
                content.onClick.AddListener(() =>
                {
                    string current = content.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
                    if (current.EndsWith(".obj"))
                    {
                        ContentManager.instance.AddObj(currentPathText.text + "/" +current);
                    }

                });
            }

            content.gameObject.SetActive(true);
            SettingContent(content.transform, paths[idx]);
            idx++;
        }
        for (; idx < contentParent.childCount; idx++)
            contentParent.GetChild(idx).gameObject.SetActive(false);
    }

    private void SettingContent(Transform content, string path)
    {
        string[] splited = path.Split('\\');
        content.GetChild(1).GetComponent<TextMeshProUGUI>().text = splited[splited.Length - 1];
    }


}
