using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{

    public enum Scene
    {
        Unknown,
        Intro,
        Login,
        Lobby,
        Game,
    }

    [System.Serializable]
    public struct SmoothDampStruct<T>
    {
        public float smoothTime;
        [HideInInspector]public T smoothVelocity;
    }

    [System.Serializable]
    public class PresetJson
    {
        public static string PATH = Application.dataPath + @"\Resources\" + @"Presets\";

        public List<Vector3> positions;
        public List<Vector3> angles;

        public PresetJson()
        {
            positions = new List<Vector3>();
            angles = new List<Vector3>();
        }
    }

    public class DirTreeStruct
    {

        public string current;
        public DirTreeStruct parent;
        private string _root;
        public List<DirTreeStruct> childs;
        public List<DirTreeFileInfo> files;


        public DirTreeStruct(string c, DirTreeStruct p, string root)
        {
            current = c;
            parent = p;
            _root = root;
            childs = new List<DirTreeStruct>();
            files = new List<DirTreeFileInfo>();
        }

        public void PrintAll()
        {
            Debug.Log(GetCurrent());
            foreach (DirTreeFileInfo f in files)
            {
                Debug.Log(GetCurrent() + " File: " + f.name);
            }
            foreach (DirTreeStruct d in childs)
            {
                d.PrintAll();
            }
        }
        public void Print()
        {
            Debug.Log(GetCurrent());
            foreach (DirTreeFileInfo f in files)
            {
                Debug.Log(GetCurrent() + " File: " + f.name);
            }
            foreach (DirTreeStruct d in childs)
            {
                Debug.Log(GetCurrent() + " Directory: " + d.GetCurrent());
            }
        }

        public DirTreeStruct Find(string name)
        {
            if (current.Equals(name) )
                return this;

            if (GetCurrent().Equals(name))
                return this;

            DirTreeStruct _d = null;
            foreach (DirTreeStruct d in childs)
            {
                _d =  d.Find(name);
                if (_d != null)
                    break;
            }
            return _d;
        }

        public string GetCurrent()
        {
            return GetCurrent(_root);
        }

        public string GetCurrent(string withoutPrefix)
        {
            if (!current.Contains(withoutPrefix))
                return current;

            int len = withoutPrefix.Length;

            return current.Substring(len);
        }

        public string GetLastDir()
        {
            string[] dirs = current.Split('\\');
            string n = dirs[dirs.Length - 1];
            if (string.IsNullOrEmpty(n))
                if (dirs.Length > 1)
                    n = dirs[dirs.Length - 2];

            return n;
        }

        public static DirTreeStruct GetFromDirStructArray(List<DirStructArrayContent> list)
        {
            DirTreeStruct dts = null;
            _GetFromDirStruct(list, 0, null, out dts);
            //dts.PrintAll();
            return dts;
        }

        private static int _GetFromDirStruct(List<DirStructArrayContent> list, int index, DirTreeStruct parent, out DirTreeStruct o)
        {
            string cur = list[index].path;
            o = new DirTreeStruct(cur, parent, list[0].path);

            int addedIndex = 1;
            for(int i =0;i < list[index].childCount; i++)
            { 
                DirTreeStruct c = null;
                addedIndex += _GetFromDirStruct(list, index + addedIndex, o, out c) ;
                o.childs.Add(c);
            }

            foreach (DirTreeFileInfo dtfi in list[index].files)
            {
                o.files.Add(dtfi);
            }

            return addedIndex;
        }
    }

    [System.Serializable]
    public class DirTreeFileInfo
    {
        public enum FileType
        {
            Prefab, Json
        }

        public FileType type;
        public string name;

        public DirTreeFileInfo(FileType type, string name)
        {
            this.type = type;
            this.name = name;
        }


    }

    [System.Serializable]
    public class DirStructArrayWrapper
    {
        public List<DirStructArrayContent> list;
    }

    [System.Serializable]
    public class DirStructArrayContent
    {
        public string path;
        public int childCount;
        public List<DirTreeFileInfo> files;
        public int depth;

        public DirStructArrayContent(string path, int depth)
        {
            this.path = path;
            this.depth = depth;
            childCount = 0;
            this.files = new List<DirTreeFileInfo>();
        }
    }
}
