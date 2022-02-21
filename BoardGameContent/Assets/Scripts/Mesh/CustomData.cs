using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CustomData 
{
    public MeshRenderer meshRenderer;

    public CustomData(string objPath)
    {
        OBJLoader loader = new OBJLoader();
        GameObject go = loader.Load(objPath);
        meshRenderer = go.GetComponentInChildren<MeshRenderer>();


        //Icon appIcon = Icon.ExtractAssociatedIcon();
    }

    public Texture GetTexture()
    {
        return meshRenderer.material.mainTexture;
    }
}
