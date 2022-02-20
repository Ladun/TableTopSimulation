using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    private CustomData customMesh;

    private void Start()
    {
        print(Application.dataPath);
    }

    public void OpenFileBrowser()
    {
        string path = EditorUtility.OpenFilePanel("Show All images", "", ".txt");
    }
}
