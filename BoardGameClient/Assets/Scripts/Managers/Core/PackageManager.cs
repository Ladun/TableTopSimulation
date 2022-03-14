using Google.Protobuf;
using Google.Protobuf.Protocol;
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

        public string GetPackageCode()
        {
            return packageName + packageVersion;
        }
    }

    [System.Serializable]
    public class ObjData
    {
        public string objName;
        public string name;
        public string[] textures;

        public float sizeX, sizeY, sizeZ;
    }
    #endregion

    private string packageDir;
    private const string packageDirName = "CustomPackages";
    private const float packageTimeout = 5f;
    private const int byteSegmentSize = 1024;

    private Dictionary<string, StoreData> _packageDict = new Dictionary<string, StoreData>();
    public Dictionary<string, StoreData> packageDict { get { return _packageDict; } }

    public bool LoadPackagesFromServer = false;

    private Dictionary<string, List<byte[]>> _receivedBytesDict = new Dictionary<string, List<byte[]>>();

    public void Init()
    {
        packageDir = Path.Combine(Directory.GetCurrentDirectory(), packageDirName);
        if (!Directory.Exists(packageDir))
        {
            Directory.CreateDirectory(packageDir);
        }

        UpdatePackagesList();

#if UNITY_EDITOR
        Debug.Log("Packages Code: ");
        foreach(var d in _packageDict)
        {
            Debug.Log(d);
        }
#endif
    }

    #region About Package

    public void UpdatePackagesList()
    {
        _packageDict.Clear();
        // Check packages
        string[] packages = Directory.GetDirectories(packageDir);
        for (int i = 0; i < packages.Length; i++)
        {
            if (File.Exists(Path.Combine(packages[i], "package.json")))
            {
                byte[] bytes = Utils.LoadFile(Path.Combine(packages[i], "package.json"));
                StoreData storeData = JsonUtility.FromJson<StoreData>(Encoding.UTF8.GetString(bytes));

                _packageDict.Add(storeData.packageName + storeData.packageVersion, storeData);
            }
        }
    }

    public bool HasPackage(string packageCode)
    {
        return packageDict.ContainsKey(packageCode);
    }

    public IEnumerator RequestPackage(int roomId, List<string> requestCodes, System.Action<string> eachStartAction)
    {
        LoadPackagesFromServer = false;

        for (int i = 0; i < requestCodes.Count; i++)
        {
            if (eachStartAction != null)
            {
                eachStartAction.Invoke(requestCodes[i]);
            }
            C_PackageTransfer packageTransfer = new C_PackageTransfer();
            packageTransfer.SendCode = 0;
            packageTransfer.RoomId = roomId;
            packageTransfer.PackageCode = requestCodes[i];

            Managers.Instance.Network.Send(packageTransfer);
            LoadPackagesFromServer = true;

            //float tmp = 0;
            while (LoadPackagesFromServer)
            {
                //tmp += Time.deltaTime;

                //if (tmp > packageTimeout)
                //    break;
                yield return null;
            }
        }
        UpdatePackagesList();
    }

    public IEnumerator SendPackage(string packageCode, int requesterPlayerId)
    {
        StoreData package;

        string targetDir = Path.Combine(packageDir, packageCode);
        if(packageDict.TryGetValue(packageCode, out package))
        {
            byte[] bytes;

            C_FileTransfer fileTransfer = new C_FileTransfer();
            fileTransfer.PackageName = packageCode;
            fileTransfer.TargetPlayerId = requesterPlayerId;
            for (int i = 0; i < package.objData.Length; i++)
            {
                bytes = File.ReadAllBytes(Path.Combine(targetDir, package.objData[i].objName));
                fileTransfer.Name = package.objData[i].objName;
                SendSegmentFile(fileTransfer, bytes);

                yield return null;

                for (int j = 0; j < package.objData[i].textures.Length; j++)
                {
                    bytes = File.ReadAllBytes(Path.Combine(targetDir, package.objData[i].textures[j]));
                    fileTransfer.Name = package.objData[i].textures[j];
                    SendSegmentFile(fileTransfer, bytes);

                    yield return null;


                }
            }

            bytes = File.ReadAllBytes(Path.Combine(targetDir, "package.json"));
            fileTransfer.Name = "package.json";
            SendSegmentFile(fileTransfer, bytes);

        }

        C_PackageTransfer packageTransfer = new C_PackageTransfer();
        packageTransfer.SendCode = 1;
        packageTransfer.PackageCode = packageCode;
        packageTransfer.RequesterPlayerId = requesterPlayerId;
        Managers.Instance.Network.Send(packageTransfer);
    }

    private void SendSegmentFile(C_FileTransfer fileTransfer, byte[] bytes)
    {
        int segmentCount = Mathf.CeilToInt(bytes.Length / (float)byteSegmentSize);
        for (int boffset = 0; boffset < segmentCount; boffset++)
        {
            int segSize = byteSegmentSize;
            int flag = (boffset + 1);

            if (boffset == segmentCount - 1)
            {
                segSize = bytes.Length - boffset * byteSegmentSize;
                flag = 0;
            }

            fileTransfer.Flag = 1;
            fileTransfer.Flag |= (flag << 1);
            fileTransfer.Filebytes = ByteString.CopyFrom(bytes, boffset * byteSegmentSize, segSize);
            Managers.Instance.Network.Send(fileTransfer);
        }
    }

    #endregion

    #region About file 
    public bool HasFile(string fileName)
    {
        return File.Exists(GetPath(fileName));
    }

    public byte[] GetFileByte(string fileName)
    {
        byte[] bytes = File.ReadAllBytes(GetPath(fileName));
        return bytes;
    }

    public void SaveFile(string packageCode, string fileName, int fileIdx, byte[] fileBytes)
    {
        string receiveKey = Path.Combine(packageCode, fileName);
        string filePath = GetPath(receiveKey);

        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if(fileIdx == 0)
        {
            int totalLen = fileBytes.Length;
            if (_receivedBytesDict.ContainsKey(receiveKey))
            {
                List<byte[]> receivedBytes = _receivedBytesDict[receiveKey];
                for (int i = 0; i < receivedBytes.Count; i++)
                    totalLen += receivedBytes[i].Length;
            }
            byte[] totalFilebytes = new byte[totalLen];
            int offset = 0;
            if (_receivedBytesDict.ContainsKey(receiveKey))
            {
                List<byte[]> receivedBytes = _receivedBytesDict[receiveKey];
                for (int i = 0; i < receivedBytes.Count; i++)
                {
                    System.Buffer.BlockCopy(receivedBytes[i], 0, totalFilebytes, offset, receivedBytes[i].Length);
                    offset += receivedBytes[i].Length;
                }
            }
            System.Buffer.BlockCopy(fileBytes, 0, totalFilebytes, offset, fileBytes.Length);

            File.WriteAllBytes(filePath, totalFilebytes);

            _receivedBytesDict.Remove(receiveKey);
        }
        else
        {
            if (!_receivedBytesDict.ContainsKey(receiveKey))
            {
                _receivedBytesDict.Add(receiveKey, new List<byte[]>());
            }
            _receivedBytesDict[receiveKey].Add(fileBytes);
        }

    }
    public string GetPath(string fileName)
    {
        return Path.Combine(packageDir, fileName);
    }
    #endregion

}
