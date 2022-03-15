using Dummiesman;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectManager 
{

    private Dictionary<int, Player> _players = new Dictionary<int, Player>();
    private Dictionary<int, TableObject> _tableObjects = new Dictionary<int, TableObject>();

    private OBJLoader loader = new OBJLoader();



    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public void AddPlayer(Player p, ObjectInfo info)
    {
        Vector3 playerPos = Utils.Dim3Info2Vector3(info.Pos);
        p.Id = info.ObjectId;
        p.transform.position = playerPos;
        p.SetColor(Managers.Instance.GetScene<GameScene>().colors[info.Flag]);
        _players.Add(info.ObjectId, p);
    }

    public void Add(ObjectInfo info)
    {
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);

        if(objectType == GameObjectType.Player)
        {
            Player player = Object.Instantiate(Managers.Instance.Resource.Get<GameObject>("Prefabs/Player")).GetComponent<Player>();
            AddPlayer(player, info);
        }
        else if(objectType == GameObjectType.TableObject)
        {

            PackageManager.StoreData data;
            if(Managers.Instance.Package.packageDict.TryGetValue(info.PackageCode, out data))
            {
                PackageManager.ObjData objData = null;
                for(int i =0;i < data.objData.Length; i++)
                {
                    if (data.objData[i].name.Equals(info.Name))
                    {
                        objData = data.objData[i];
                        break;
                    }
                }

                if (objData == null)
                    return;

                string localPath = Path.Combine(data.GetPackageCode(), objData.objName);

                GameObject go = loader.Load(Managers.Instance.Package.GetPath(localPath));
                if(go == null)
                {
                    // TODO: Error check
                }

                BoxCollider bc = go.AddComponent<BoxCollider>();
                bc.size = new Vector3(objData.sizeX, objData.sizeY, objData.sizeZ);
                go.AddComponent<Rigidbody>();
                go.AddComponent<Outline>();
                TableObject to = go.AddComponent<TableObject>();

                go.layer = 7;

                // Setting textures
                MeshRenderer[] meshRenders = go.GetComponentsInChildren<MeshRenderer>();
                for(int i = 0; i < meshRenders.Length && i < objData.textures.Length; i++)
                {
                    int _i = i;
                    string textureLocalPath = Path.Combine(data.GetPackageCode(), objData.textures[i]);
                    Debug.Log(textureLocalPath);
                    Managers.Instance.StartCoroutine(Utils.CoLoadTexture(Managers.Instance.Package.GetPath(textureLocalPath), 
                        (texture) =>
                        {
                            meshRenders[_i].material.mainTexture = texture;
                        }));
                }

                to.Init();
                to.Id = info.ObjectId;
                to.SetMoveInfo(Utils.Dim3Info2Vector3(info.Pos), Utils.Dim3Info2Vector3(info.Angle), true);
                _tableObjects.Add(info.ObjectId, to);
            }
        }
        else if(objectType == GameObjectType.TableObjectSet)
        {
            TableObjectSet set = Object.Instantiate(Managers.Instance.Resource.Get<GameObject>("Prefabs/TableObjectSet")).GetComponent<TableObjectSet>();
            set.Id = info.ObjectId;
            set.SetMoveInfo(Utils.Dim3Info2Vector3(info.Pos), Utils.Dim3Info2Vector3(info.Angle), true);

            set.Init();
            _tableObjects.Add(info.ObjectId, set);
        }
        else if(objectType == GameObjectType.Preset)
        {
            Preset preset = Object.Instantiate(Managers.Instance.Resource.Get<GameObject>("Prefabs/Preset")).GetComponent<Preset>();
            preset.presetName = info.Name;
            preset.SettingPreset();
            preset.Id = info.ObjectId;
            preset.SetMoveInfo(Utils.Dim3Info2Vector3(info.Pos), Utils.Dim3Info2Vector3(info.Angle), true);

            preset.Init();
            _tableObjects.Add(info.ObjectId, preset);
        }
    }

    public void Remove(int id)  
    {

        GameObjectType objectType = GetObjectTypeById(id);
        switch (objectType)
        {
            case GameObjectType.Player:
                {
                    Player p = null;
                    if (!_players.TryGetValue(id, out p))
                        return;
                    _players.Remove(id);

                    Object.Destroy(p.gameObject);
                    break;
                }
            case GameObjectType.Preset:
            case GameObjectType.TableObject:
                {
                    TableObject to = null;
                    if (!_tableObjects.TryGetValue(id, out to))
                        return;
                    _tableObjects.Remove(id);

                    Object.Destroy(to.gameObject);
                    break;
                }
            case GameObjectType.TableObjectSet:
                {
                    TableObject to = null;
                    if (!_tableObjects.TryGetValue(id, out to))
                        return;
                    _tableObjects.Remove(id);
                    TableObjectSet set = to as TableObjectSet;
                    set.FreeAllObject();
                    Object.Destroy(set.gameObject);

                    break;
                }
        }
    }


    public T Find<T>(int id) where T : MonoBehaviour
    {

        GameObjectType objectType = GetObjectTypeById(id);
        switch (objectType)
        {
            case GameObjectType.Player:
                {
                    Player player = null;
                    if (_players.TryGetValue(id, out player))
                    {
                        return player as T;
                    }
                    break;
                }
            case GameObjectType.TableObject:
            case GameObjectType.TableObjectSet:
            case GameObjectType.Preset:
                {
                    TableObject tableObject = null;
                    if (_tableObjects.TryGetValue(id, out tableObject))
                    {
                        return tableObject as T;
                    }
                    break;
                }
        }
        return null;

    }
}
