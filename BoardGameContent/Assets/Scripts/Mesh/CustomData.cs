using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CustomData 
{
    public GameObject go;

    public CustomData(string objPath)
    {
        OBJLoader loader = new OBJLoader();
        go = loader.Load(objPath);

        //Icon appIcon = Icon.ExtractAssociatedIcon();
    }
}
