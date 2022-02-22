using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CustomData 
{
    public GameObject go;
    public MeshRenderer[] meshRenderers;
    public string path;

    public CustomData(string objPath)
    {
        path = objPath;
        OBJLoader loader = new OBJLoader();
        go = loader.Load(objPath);
        meshRenderers = go.GetComponentsInChildren<MeshRenderer>();

        //Icon appIcon = Icon.ExtractAssociatedIcon();
    }

    public void SetTexture(int idx, Texture2D texture)
    {
        meshRenderers[idx].material.mainTexture = texture;
    }

    public Texture GetTexture(int idx)
    {
        return meshRenderers[idx].material.mainTexture;
    }
}
