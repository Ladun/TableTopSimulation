using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Define;

public class Utils 
{
    public static Vector3 Dim3Info2Vector3(Dim3Info info)
    {
        return new Vector3(info.X, info.Y, info.Z);
    }
    public static Dim3Info Vector32Dim3Info(Vector3 info)
    {
        Dim3Info dim = new Dim3Info();
        dim.X = info.x;
        dim.Y = info.y;
        dim.Z = info.z;

        return dim;
    }

    public static Vector3 GetViewPortMousePos()
    {
        return Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }
    public static Vector3 SmoothDampEuler(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
    {
        return new Vector3(
          Mathf.SmoothDampAngle(current.x, target.x, ref currentVelocity.x, smoothTime),
          Mathf.SmoothDampAngle(current.y, target.y, ref currentVelocity.y, smoothTime),
          Mathf.SmoothDampAngle(current.z, target.z, ref currentVelocity.z, smoothTime)
        );
    }

    public static DirTreeStruct GetDirectoryTree(string root)
    {

        DirTreeStruct dts = new DirTreeStruct(root, null, root);

        Queue<DirTreeStruct> s = new Queue<DirTreeStruct>();
        s.Enqueue(dts);


        while (s.Count > 0)
        {
            DirTreeStruct p = s.Dequeue();

            string[] dirs = Directory.GetDirectories(p.current);
            foreach (string d in dirs)
            {
                DirTreeStruct child = new DirTreeStruct(d, p, root);
                p.childs.Add(child);
                s.Enqueue(child);
            }

            string[] files = Directory.GetFiles(p.current);
            foreach (string f in files)
            {
                if (!f.EndsWith(".meta"))
                {
                    if (f.EndsWith(".prefab"))
                        p.files.Add(new DirTreeFileInfo(DirTreeFileInfo.FileType.Prefab, f.Substring(f.LastIndexOfAny(new char[] { '\\', '/' }) + 1).Split('.')[0]));
                    else if (f.EndsWith(".json"))
                        p.files.Add(new DirTreeFileInfo(DirTreeFileInfo.FileType.Json, f.Substring(f.LastIndexOfAny(new char[] { '\\', '/' }) + 1).Split('.')[0]));
                }
            }
        }

        return dts;
    }

    public static byte[] LoadFile(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        return data;
    }

    public static string GetNameFromPath(string path)
    {
        string name = path;
        int index = name.LastIndexOf('/');
        if (index >= 0)
            name = name.Substring(index + 1);

        return name;
    }

    public static Vector3 MinEach(Vector3 cur, Vector3 tar)
    {
        if(cur.x > tar.x)
        {
            cur.x = tar.x;
        }
        if (cur.y > tar.y)
        {
            cur.y = tar.y;
        }
        if (cur.z > tar.z)
        {
            cur.z = tar.z;
        }
        return cur;
    }

    public static Vector3 MaxEach(Vector3 cur, Vector3 tar)
    {
        if (cur.x < tar.x)
        {
            cur.x = tar.x;
        }
        if (cur.y <tar.y)
        {
            cur.y = tar.y;
        }
        if (cur.z < tar.z)
        {
            cur.z = tar.z;
        }
        return cur;
    }
}
