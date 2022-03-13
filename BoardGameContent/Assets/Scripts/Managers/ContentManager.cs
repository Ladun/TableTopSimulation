using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager : MonoBehaviour
{
    #region Singleton
    private static ContentManager _instance;
    public static ContentManager instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject go = new GameObject("@ContentManager");
                _instance = go.AddComponent<ContentManager>();
            }
            return _instance;
        }
    }
    #endregion

    private List<CustomData> _data = new List<CustomData>();
    public List<CustomData> data
    {
        get { return _data; }
    }

    private int activeDataIdx;

    public CustomData AddObj(string path)
    {
        CustomData cmm = new CustomData(path);
        _data.Add(cmm);

        ActiveData(_data.Count - 1);

        return cmm;
    }

    public void DeleteObj(int idx)
    {
        _data.RemoveAt(idx);
    }

    public void ActiveData(int idx)
    {
        activeDataIdx = idx;
        for (int i = 0; i <_data.Count; i++)
        {
            _data[i].go.SetActive(false);
        }
        _data[idx].go.SetActive(true);
    }

    public void Clear()
    {
        for (int i = 0; i < _data.Count; i++)
        {
            Destroy(_data[i].go);
        }
        _data.Clear();
    }

    public CustomData GetActiveObject()
    {
        return _data[activeDataIdx];
    }
}
