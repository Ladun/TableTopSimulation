using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Define;

public class Preset : TableObject
{
    [Header("Preset")]
    public string presetName;
    public Transform presetPoint;

    public override void Init()
    {
        eventDict.Add(TableObjectEventType.Over, OverEvent);
        eventDict.Add(TableObjectEventType.Select, SelectEvent);
        eventDict.Add(TableObjectEventType.Lock, LockEvent);
    }

    public void SettingPreset()
    {
        string path = presetName + ".json";
        TextAsset jsonAsset = Managers.Instance.Resource.Get<TextAsset>(presetName);
        if (jsonAsset == null)
            return;
        string json = jsonAsset.text;

        PresetJson presetJson = JsonUtility.FromJson<PresetJson>(json);

        if (presetJson.positions.Count < 1)
            return;

        Vector3 min = presetJson.positions[0];
        Vector3 max = presetJson.positions[0];
        for (int i = 0; i < presetJson.positions.Count; i++)
        {
            Transform t;
            if (i < transform.childCount)
            {
                t = transform.GetChild(i);
            }
            else
            {
                t = Instantiate(presetPoint);
                t.SetParent(transform);
            }
            t.localPosition = presetJson.positions[i];
            t.localEulerAngles = presetJson.angles[i];

            min = Utils.MinEach(min, presetJson.positions[i]);
            max = Utils.MaxEach(max, presetJson.positions[i]);
        }

        Vector3 size = max - min;
        Vector3 center = min + size / 2;

        GetComponent<BoxCollider>().size = size;
        GetComponent<BoxCollider>().center = center;

        outline.UpdateRendererCache();
    }

    public Transform GetNear(Vector3 point)
    {
        if (transform.childCount < 1)
            return null;
        point = new Vector3(point.x, 0, point.z);


        float min = Vector3.Distance(point, new Vector3(transform.GetChild(0).position.x, 0, transform.GetChild(0).position.z));
        int idx = 0;

        for (int i = 1; i < transform.childCount; i++)
        {
            Vector3 cPos = transform.GetChild(i).position;
            cPos = new Vector3(cPos.x, 0, cPos.z);
            if(Vector3.Distance(point, cPos) < min)
            {
                min = Vector3.Distance(point, cPos);
                idx = i;
            }
        }

        return transform.GetChild(idx);
    }


}
