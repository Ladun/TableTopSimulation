using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    private CustomData customMesh;

    private void Start()
    {
    }

    public void OpenFileBrowser()
    {
        string path = EditorUtility.OpenFilePanel("Show All images", "", ".txt");
    }
}
