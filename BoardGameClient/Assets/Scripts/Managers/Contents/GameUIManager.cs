using Google.Protobuf.Collections;
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

    private string curPackageCode;
    private string curDir;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!ActiveSelfTooltip("ObjectControlTooltip"))
                playerListPopup.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (!ActiveSelfTooltip("ObjectControlTooltip"))
                playerListPopup.SetActive(false);
        }
    }

    public void UpdatePlayerList(List<P_PlayerProfile> items)
    {
        playerContent.parent.UpdateItemList(playerContent.prefab, items);
    }


    // Reference : ObjectListButton
    public void InteractObjectPopup(GameObject objectPopup)
    {
        if (objectPopup.activeSelf)
            ClosePopupItem(objectPopup.transform);
        else
            OpenPopupItem(objectPopup.transform);
    }

    public void LoadAllGames(RepeatedField<string> packageCodes)
    {
            
        for(int i = 0; i < packageCodes.Count; i++)
        {

            objectListPopup.AddTab(packageCodes[i]);
        }
    }

    public void ChangeGame(string game)
    {
        curPackageCode = game;
        UpdateObjectList();
    }

    public void ChangeDir(string name)
    {
        curDir = name;
        UpdateObjectList();
    }

    public void UpdateObjectList()
    {
        PackageManager.StoreData storeData = null;
        if (!Managers.Instance.Package.packageDict.TryGetValue(curPackageCode, out storeData))
            return;

        List<UIObjectListContent.ObjectListStruct> items = new List<UIObjectListContent.ObjectListStruct>();

        for (int i = 0; i < storeData.objData.Length; i++)
        {
            items.Add(new UIObjectListContent.ObjectListStruct() { item = storeData.objData[i], packageCode=storeData.GetPackageCode(), isDir = false, isPreset = false });
        }

        objectContent.parent.UpdateItemList(objectContent.prefab, items);
    }

    //public void UpdateObjectList()
    //{
    //    DirTreeStruct curGameDTS = null;
    //    if (!tableObjectDirectoryTree.TryGetValue(curPackageCode, out curGameDTS))
    //        return;

    //    DirTreeStruct curDTS = curGameDTS.Find(curDir);
    //    if (curDTS == null)
    //    {
    //        Debug.LogError("DirTreeStruct Find is null");
    //        return;
    //    }

    //    string cDir = curDTS.GetCurrent(curDir.Substring(0, curDir.IndexOf("Resources") + 10));

    //    List<UIObjectListContent.ObjectListStruct> items = new List<UIObjectListContent.ObjectListStruct>();

    //    if (curDTS.parent != null)
    //        items.Add(new UIObjectListContent.ObjectListStruct() { path = "..", dts = curDTS.parent, isDir = true, isPreset = false });

    //    foreach (DirTreeStruct d in curDTS.childs)
    //    {
    //        items.Add(new UIObjectListContent.ObjectListStruct() { path = d.GetLastDir(), dts = d, isDir = true, isPreset = false });
    //    }

    //    foreach(DirTreeFileInfo f in curDTS.files)
    //    {
    //        string fileName = cDir + "/" + f.name;
    //        if (f.type == DirTreeFileInfo.FileType.Prefab)
    //        {
    //            TableObject to = Managers.Instance.Resource.Get<GameObject>(fileName).GetComponent<TableObject>();
    //            if (to == null)
    //                continue;
    //            items.Add(new UIObjectListContent.ObjectListStruct() { path = fileName, item = to, isDir = false, isPreset = false });
    //        }
    //        else if (f.type == DirTreeFileInfo.FileType.Json)
    //        {
    //            TextAsset pj = Managers.Instance.Resource.Get<TextAsset>(fileName);
    //            if (pj == null)
    //                continue;
    //            items.Add(new UIObjectListContent.ObjectListStruct() { path = fileName, isDir = false, isPreset = true });
    //        }
    //    }

    //    objectContent.parent.UpdateItemList(objectContent.prefab, items);

    //}
}
