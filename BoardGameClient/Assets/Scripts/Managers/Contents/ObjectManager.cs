using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager 
{

    private Dictionary<int, Player> _players = new Dictionary<int, Player>();
    private Dictionary<int, TableObject> _tableObjects = new Dictionary<int, TableObject>();


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
            TableObject tableObject = Object.Instantiate(Managers.Instance.Resource.Get<GameObject>( info.Name)).GetComponent<TableObject>(); // Object List Popup 부분에서 Resource Load를
            tableObject.Id = info.ObjectId;
            tableObject.SetMoveInfo(Utils.Dim3Info2Vector3(info.Pos), Utils.Dim3Info2Vector3(info.Angle), true);
            _tableObjects.Add(info.ObjectId, tableObject);
        }
        else if(objectType == GameObjectType.TableObjectSet)
        {
            TableObjectSet set = Object.Instantiate(Managers.Instance.Resource.Get<GameObject>("Prefabs/TableObjectSet")).GetComponent<TableObjectSet>();
            set.Id = info.ObjectId;
            set.SetMoveInfo(Utils.Dim3Info2Vector3(info.Pos), Utils.Dim3Info2Vector3(info.Angle), true);
            _tableObjects.Add(info.ObjectId, set);
        }
        else if(objectType == GameObjectType.Preset)
        {
            Preset preset = Object.Instantiate(Managers.Instance.Resource.Get<GameObject>("Prefabs/Preset")).GetComponent<Preset>();
            preset.presetName = info.Name;
            preset.SettingPreset();
            preset.Id = info.ObjectId;
            preset.SetMoveInfo(Utils.Dim3Info2Vector3(info.Pos), Utils.Dim3Info2Vector3(info.Angle), true);
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
