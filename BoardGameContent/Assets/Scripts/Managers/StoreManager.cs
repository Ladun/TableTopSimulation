using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    #region SingleTon
    private static StoreManager _instance;
    public static StoreManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<StoreManager>();
            }
            return _instance;
        }
    }
    #endregion

    [System.Serializable]
    public class StoreData
    {
        public string packageName;
        public string packageVersion;
        public ObjData[] objData;
    }

    [System.Serializable]
    public class ObjData
    {
        public string objName;
        public string name;
        public string[] textures;

        public float sizeX, sizeY, sizeZ;
    }

    public void Save(string packageName, string packageVersion, string targetDir)
    {
        StartCoroutine(CoSave( packageName, packageVersion, targetDir));
    }

    private IEnumerator CoSave(string packageName, string packageVersion, string targetDir)
    {
        string packageDir = Path.Combine(targetDir, $"{packageName}{packageVersion}");
        if (!Directory.Exists(packageDir))
        {
            Directory.CreateDirectory(packageDir);
        }

        // Define store data
        StoreData storeData = new StoreData();
        storeData.packageName = packageName;
        storeData.packageVersion = packageVersion;

        // Construct object data ----
        ObjData[] objData = new ObjData[ContentManager.instance.data.Count];
        HashSet<string> objSet = new HashSet<string>();

        for (int i = 0; i < ContentManager.instance.data.Count; i++)
        {
            CustomData data = ContentManager.instance.data[i];
            objSet.Add(data.path);

            ObjData d = new ObjData();
            d.objName = Path.GetFileName(data.path);
            d.name = data.go.name;
            d.textures = new string[data.meshRenderers.Length];
            for(int j = 0; j < d.textures.Length; j++)
            {
                d.textures[j] = d.name + $"_{j}.png";

                // Save Textures
                byte[] _bytes = ((Texture2D)data.meshRenderers[j].material.mainTexture).EncodeToPNG();
                SaveFile(Path.Combine(packageDir, d.textures[j]), _bytes);
            }
            d.sizeX = data.colliderSize.x;
            d.sizeY = data.colliderSize.y;
            d.sizeZ = data.colliderSize.z;

            objData[i] = d;
        }

        foreach(string path in objSet)
        {
            // Save objs
            File.Copy(path, Path.Combine(packageDir, Path.GetFileName(path)), true);
        }

        storeData.objData = objData;

        string jsonFile = JsonUtility.ToJson(storeData);
        SaveFile(Path.Combine(packageDir, "package.json"), Encoding.UTF8.GetBytes(jsonFile));

        yield return null;
    }

    public void Load(string jsonFilePath)
    {
        StartCoroutine("CoLoad", jsonFilePath);
    }

    private IEnumerator CoLoad(string jsonFilePath)
    {
        ContentManager.instance.Clear();

        string targetDir = Path.GetDirectoryName(jsonFilePath);

        byte[] bytes = LoadFile(jsonFilePath);
        StoreData storeData = JsonUtility.FromJson<StoreData>(Encoding.UTF8.GetString(bytes));

        CustomContentBrowser _ccb = UIManager.instance.contentBrowser;

        for(int i = 0; i < storeData.objData.Length; i++)
        {
            ObjData d = storeData.objData[i];
            _ccb.Add(Path.Combine(targetDir, d.objName));

            CustomData cdata = _ccb.WrappedDatas[_ccb.WrappedDatas.Count - 1].data;
            cdata.go.name = d.name;
            cdata.colliderSize = new Vector3(d.sizeX, d.sizeY, d.sizeZ);

            for(int j = 0; j < d.textures.Length; j++)
            {
                int _j = j;
                yield return StartCoroutine(Utils.CoLoadTexture(Path.Combine(targetDir, d.textures[j]), (texture) =>
                                            {
                                                cdata.SetTexture(_j, texture);
                                            }));
            }

        }

        UIManager.instance.UpdateContentBrowser(storeData.packageName, storeData.packageVersion);
    }

    private void SaveFile(string path, byte[] bytes)
    {
        // TODO: Ienumerator로 yield return할 수 있도록 변경
        FileStream stream = new FileStream(path, FileMode.Create);
        stream.Write(bytes, 0, bytes.Length);
        stream.Close();
    }
    private byte[] LoadFile(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        return data;
    }

}
