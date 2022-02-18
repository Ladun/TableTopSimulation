using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    private CustomMeshData customMesh;

    private void Start()
    {
        print(Application.dataPath);

        customMesh = new CustomMeshData(Application.dataPath + "/Meshs/Card.obj");
    }

    public void OpenFileBrowser()
    {
        string path = EditorUtility.OpenFilePanel("Show All images", "", ".txt");
    }
}
