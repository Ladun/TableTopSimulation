using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PresetMaker))]
public class PresetMakerEditor : Editor
{

    PresetMaker t;
    private void OnEnable()
    {
       t = target as PresetMaker;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (t == null)
            return;

        if(GUILayout.Button("Generate Preset"))
        {
            t.MakePreset();
        }
    }
}
