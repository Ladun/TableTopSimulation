using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Define;

public class GameUIManager : UIManager
{

    public UIListContentsInfo<UIGridListViewer, UIObjectListContent.ObjectListStruct> objectContent;
    public UIListContentsInfo<UIGridListViewer, P_PlayerProfile> playerContent;

    public GameObject playerListPopup;

    // Object List
    public ObjectListPopup objectListPopup;

    public string[] games;
    private Dictionary<string, DirTreeStruct> tableObjectDirectoryTree = new Dictionary<string, DirTreeStruct>();

    private string curDir;
    private string curGame;

    private void Awake()
    {
    }

    private void Start()
    {
        LoadAllGames();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!ActiveSelfTooltip("ObjectControlTooltip"))
                playerListPopup.gameObject.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (!ActiveSelfTooltip("ObjectControlTooltip"))
                playerListPopup.gameObject.SetActive(false);
        }
    }

    public void UpdatePlayerList(List<P_PlayerProfile> items)
    {
        playerContent.parent.UpdateItemList(playerContent.prefab, items);
    }

    public void InteractObjectPopup(GameObject objectPopup)
    {
        if (objectPopup.activeSelf)
            ClosePopupItem(objectPopup.transform);
        else
            OpenPopupItem(objectPopup.transform);
    }

    private void LoadAllGames()
    {
            
        for(int i = 0; i < games.Length; i++)
        {
            TextAsset jsonAsset = Resources.Load<TextAsset>(games[i] + @"\DirectoryHierarchy");
            if (jsonAsset == null)
                return;
            string json = jsonAsset.text;
            DirStructArrayWrapper dsa = JsonUtility.FromJson<DirStructArrayWrapper>(json);
            tableObjectDirectoryTree.Add(games[i], DirTreeStruct.GetFromDirStructArray(dsa.list));

            objectListPopup.AddTab(games[i]);
        }
    }

    public void ChangeGame(string game)
    {
        curGame = game;
        DirTreeStruct curGameDTS = null;
        if (!tableObjectDirectoryTree.TryGetValue(curGame, out curGameDTS))
            return;
        curDir = curGameDTS.current;
        UpdateObjectList();
    }

    public void ChangeDir(string name)
    {
        curDir = name;
        UpdateObjectList();
    }

    public void UpdateObjectList()
    {
        DirTreeStruct curGameDTS = null;
        if (!tableObjectDirectoryTree.TryGetValue(curGame, out curGameDTS))
            return;

        DirTreeStruct curDTS = curGameDTS.Find(curDir);
        if (curDTS == null)
        {
            Debug.LogError("DirTreeStruct Find is null");
            return;
        }

        string cDir = curDTS.GetCurrent(curDir.Substring(0, curDir.IndexOf("Resources") + 10));

        List<UIObjectListContent.ObjectListStruct> items = new List<UIObjectListContent.ObjectListStruct>();

        if (curDTS.parent != null)
            items.Add(new UIObjectListContent.ObjectListStruct() { path = "..", dts = curDTS.parent, isDir = true, isPreset = false });

        foreach (DirTreeStruct d in curDTS.childs)
        {
            items.Add(new UIObjectListContent.ObjectListStruct() { path = d.GetLastDir(), dts = d, isDir = true, isPreset = false });
        }

        foreach(DirTreeFileInfo f in curDTS.files)
        {
            string fileName = cDir + "/" + f.name;
            if (f.type == DirTreeFileInfo.FileType.Prefab)
            {
                TableObject to = Managers.Instance.Resource.Get<GameObject>(fileName).GetComponent<TableObject>();
                if (to == null)
                    continue;
                items.Add(new UIObjectListContent.ObjectListStruct() { path = fileName, item = to, isDir = false, isPreset = false });
            }
            else if (f.type == DirTreeFileInfo.FileType.Json)
            {
                TextAsset pj = Managers.Instance.Resource.Get<TextAsset>(fileName);
                if (pj == null)
                    continue;
                items.Add(new UIObjectListContent.ObjectListStruct() { path = fileName, isDir = false, isPreset = true });
            }
        }

        objectContent.parent.UpdateItemList(objectContent.prefab, items);

    }
}
