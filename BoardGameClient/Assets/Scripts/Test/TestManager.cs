using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Define;

public class TestManager : MonoBehaviour
{
    public string[] games;

    public CameraController cam;

    public Preset preset;

    public BoxCollider target;
    public Vector3 size;

    private void Awake()
    {
    }

    private void Start()
    {
        cam.Init(new Vector3(0, 0, 0));


        DirectoryTest();

        preset.SettingPreset();
    }

    private void Update()
    {
        size = target.bounds.size;
    }

    private void DirectoryTest()
    {

    }

}
