using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PackageManager
{
    #region Package Data
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
    }
    #endregion

    private string packageDir;
    private const string packageDirName = "CustomPackages";

    private Dictionary<string, StoreData> _packageCodes = new Dictionary<string, StoreData>();
    public Dictionary<string, StoreData> packageCodes { get { return _packageCodes; } }

    public void Init()
    {
        packageDir = Path.Combine(Directory.GetCurrentDirectory(), packageDirName);
        if (!Directory.Exists(packageDir))
        {
            Directory.CreateDirectory(packageDir);
        }

        // Check packages
        string[] packages = Directory.GetDirectories(packageDir);
        for(int i = 0; i < packages.Length; i++)
        {
            if(File.Exists(Path.Combine(packages[i], "package.json")))
            {
                byte[] bytes = Utils.LoadFile(Path.Combine(packages[i], "package.json"));
                StoreData storeData = JsonUtility.FromJson<StoreData>(Encoding.UTF8.GetString(bytes));

                _packageCodes.Add(storeData.packageName + storeData.packageVersion, storeData);
            }
        }

#if UNITY_EDITOR
        Debug.Log("Packages Code: ");
        foreach(var d in _packageCodes)
        {
            Debug.Log(d);
        }
#endif
    }

    public bool HasFile(string fileName)
    {

        return File.Exists(GetPath(fileName));
    }

    public byte[] GetFileByte(string fileName)
    {
        byte[] bytes = File.ReadAllBytes(GetPath(fileName));
        return bytes;
    }

    public void SaveFile(string fileName, byte[] fileBytes)
    {
        File.WriteAllBytes(GetPath(fileName), fileBytes);
    }
    public string GetPath(string fileName)
    {
        return Path.Combine(packageDir, fileName);
    }
}
