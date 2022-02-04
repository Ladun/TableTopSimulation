using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Define;

public class PresetMaker : MonoBehaviour
{

    public Transform presetObject;
    public string presetName;
        
    public void MakePreset()
    {
        string path = PresetJson.PATH + presetName + ".json";

        PresetJson presetJson = new PresetJson();

        for(int i =0;i < presetObject.childCount; i++)
        {
            Transform t = presetObject.GetChild(i);
            presetJson.positions.Add(t.localPosition);
            presetJson.angles.Add(t.localEulerAngles);
        }

        string json = JsonUtility.ToJson(presetJson);

        File.WriteAllText(path, json);
    }


    
}
