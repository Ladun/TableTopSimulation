using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static Define;
using System.IO;

public class ResourceHierarchyEditorWindow : EditorWindow
{
    string targetDir;


    public DirStructArrayWrapper contents;

    //Window 메뉴에 "My Window" 항목을 추가한다.
    [MenuItem("Window/Resource Hierarchy")]
    static void Init()
    {
        // 생성되어있는 윈도우를 가져온다. 없으면 새로 생성한다. 싱글턴 구조인듯하다.
        ResourceHierarchyEditorWindow window = (ResourceHierarchyEditorWindow)GetWindow(typeof(ResourceHierarchyEditorWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Resources 폴더 기준의 경로를 적습니다.\nSave를 하면 해당 Resources/Target Dirctory/DirectoryHierarchy.json으로 저장됩니다.");
        GUILayout.Space(5);
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        targetDir = EditorGUILayout.TextField("Target Directory", targetDir);

        if(GUILayout.Button("Find Directory Hierarchy"))
        {
            FindDirectoryHierarchy();
        }

        if (contents != null)
        {

            if (contents.list != null && contents.list.Count > 0)
            {
                EditorGUILayout.Space(10);

                GUIStyle guiStyle = new GUIStyle();
                guiStyle.fontSize = 15;
                guiStyle.fontStyle = FontStyle.Bold;
                guiStyle.normal.textColor = Color.white;

                GUIStyle fileGuiStyle = new GUIStyle();
                fileGuiStyle.fontSize = 12;
                Color color;
                ColorUtility.TryParseHtmlString("#FFA600", out color);
                fileGuiStyle.normal.textColor = color;

                foreach (DirStructArrayContent dsac in contents.list)
                {
                    EditorGUI.indentLevel = 1 + dsac.depth * 2;

                    string r = dsac.path.Substring((Application.dataPath + "/Resources/").Length);
                    EditorGUILayout.LabelField(r, guiStyle);

                    EditorGUI.indentLevel += 2;
                    foreach (DirTreeFileInfo dtfi in dsac.files)
                    {
                        EditorGUILayout.LabelField(dtfi.name + "." + dtfi.type.ToString(), fileGuiStyle);
                    }
                    EditorGUI.indentLevel -= 2;
                }
                if (GUILayout.Button("Save as File"))
                {
                    SaveAsFile();
                }
                //if (GUILayout.Button("Test DTS"))
                //{
                //    DirTreeStruct.GetFromDirStructArray(contents.list);
                //}
            }
        }
    }

    private void FindDirectoryHierarchy()
    {
        if (contents == null)
            contents = new DirStructArrayWrapper();

        if (contents.list == null)
            contents.list = new List<DirStructArrayContent>();
        else
            contents.list.Clear();


        string path = Application.dataPath + "/Resources/" + targetDir;

        AddByDFS(path, 0);
    }

    private void AddByDFS(string curDir, int depth)
    {
        DirStructArrayContent dsac = new DirStructArrayContent(curDir, depth);

        string[] dirs = Directory.GetDirectories(curDir);
        dsac.childCount = dirs.Length;

        string[] files = Directory.GetFiles(curDir);
        foreach (string f in files)
        {
            if (!f.EndsWith(".meta"))
            {
                if (f.EndsWith(".prefab"))
                    dsac.files.Add(new DirTreeFileInfo(DirTreeFileInfo.FileType.Prefab, f.Substring(f.LastIndexOfAny(new char[] { '\\', '/' }) + 1).Split('.')[0]));
                else if (f.EndsWith(".json"))
                {
                    string fileName = f.Substring(f.LastIndexOfAny(new char[] { '\\', '/' }) + 1).Split('.')[0];
                    if (!fileName.StartsWith("DirectoryHierarchy"))
                        dsac.files.Add(new DirTreeFileInfo(DirTreeFileInfo.FileType.Json, fileName));
                }
            }
        }
        contents.list.Add(dsac);

        foreach (string d in dirs)
        {
            AddByDFS(d, depth + 1);
        }
    }

    private void SaveAsFile()
    {
        string path = Application.dataPath + "/Resources/" + targetDir + "/DirectoryHierarchy.json";


        string json = JsonUtility.ToJson(contents);

        Debug.Log("Save Json file to " + path);
        Debug.Log(json);

        File.WriteAllText(path, json);
    }
}
