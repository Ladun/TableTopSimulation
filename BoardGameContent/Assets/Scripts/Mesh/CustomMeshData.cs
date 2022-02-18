using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CustomMeshData 
{
    private Mesh mesh;

    public CustomMeshData(string objPath)
    {
        mesh = new Mesh();
        OBJLoader loader = new OBJLoader();
        GameObject go = loader.Load(objPath);

        //Icon appIcon = Icon.ExtractAssociatedIcon();
    }
}
