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
        public string objName;
        public string name;
        public string[] textures;
    }

    public void Save(string targetDir)
    {
        StartCoroutine("CoSave", targetDir);
    }

    private IEnumerator CoSave(string targetDir)
    {
        StoreData[] storeData = new StoreData[ContentManager.instance.data.Count];
        HashSet<string> objSet = new HashSet<string>();

        for (int i = 0; i < ContentManager.instance.data.Count; i++)
        {
            CustomData data = ContentManager.instance.data[i];
            objSet.Add(data.path);

            StoreData d = new StoreData();
            d.objName = Path.GetFileName(data.path);
            d.name = data.go.name;
            d.textures = new string[data.meshRenderers.Length];
            for(int j = 0; j < d.textures.Length; j++)
            {
                d.textures[j] = d.name + $"_{j}.png";

                // Save Textures
                byte[] _bytes = ((Texture2D)data.meshRenderers[j].material.mainTexture).EncodeToPNG();
                SaveFile(Path.Combine(targetDir, d.textures[j]), _bytes);
            }
            storeData[i] = d;
        }

        foreach(string path in objSet)
        {
            // Save objs
            File.Copy(path, Path.Combine(targetDir, Path.GetFileName(path)), true);
        }

        string jsonFile = JsonHelper.ToJson(storeData);
        SaveFile(Path.Combine(targetDir, "package.json"), Encoding.UTF8.GetBytes(jsonFile));

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
        StoreData[] storeData = JsonHelper.FromJson<StoreData>(Encoding.UTF8.GetString(bytes));

        for(int i = 0; i < storeData.Length; i++)
        {
            StoreData d = storeData[i];
            CustomData cdata = ContentManager.instance.AddObj(Path.Combine(targetDir, d.objName));
            cdata.go.name = d.name;

            for(int j = 0; j < d.textures.Length; j++)
            {
                int _j = j;
                yield return StartCoroutine(Utils.CoLoadTexture(Path.Combine(targetDir, d.textures[j]), (texture) =>
                                            {
                                                cdata.SetTexture(_j, texture);
                                            }));
            }

        }

        UIManager.instance.UpdateContentBrowser();
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
