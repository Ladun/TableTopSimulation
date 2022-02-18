using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager : MonoBehaviour
{
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

    private List<CustomMeshData> meshes = new List<CustomMeshData>();

    public void AddObj(string path)
    {
        CustomMeshData cmm = new CustomMeshData(path);
        meshes.Add(cmm);
    }
}
