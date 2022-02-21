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

    public void AddObj(string path)
    {
        CustomData cmm = new CustomData(path);
        _data.Add(cmm);

        ActiveData(_data.Count - 1);
    }

    public void ActiveData(int idx)
    {
        for(int i = 0; i <_data.Count; i++)
        {
            _data[i].meshRenderer.gameObject.SetActive(false);
        }

        _data[idx].meshRenderer.gameObject.SetActive(true);
    }
}
