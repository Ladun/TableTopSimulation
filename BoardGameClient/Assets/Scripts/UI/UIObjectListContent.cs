using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Protobuf.Protocol;
using static Define;

public class UIObjectListContent : UIListContent<UIObjectListContent.ObjectListStruct>
{
    public struct ObjectListStruct
    {
        public bool isDir;
        public bool isPreset;
        public string packageCode;
        public PackageManager.ObjData item;
        public DirTreeStruct dts;

    }

    public Sprite dirImage;
    public Sprite presetImage;

    private GameObject background;
    private Image objectImage;
    private TextMeshProUGUI objectName;
    private Button createButton;

    public override void Init()
    {
        background = transform.Find("Background").gameObject;
        objectImage = transform.Find("ObjectImage").GetComponent<Image>();
        objectName = transform.Find("ObjectName").GetComponent<TextMeshProUGUI>();
        createButton = transform.Find("CreateButton").GetComponent<Button>();

        createButton.onClick.AddListener(() =>
        {
            // Create
            if (item.isDir)
            {
                Managers.Instance.GetUIManager<GameUIManager>().ChangeDir(item.dts.current);
            }
            else 
            {
                GameObjectType type = GameObjectType.TableObject;
                if (item.isPreset)
                    type = GameObjectType.Preset;

                // TODO: spawn by ObjData
                Managers.Instance.GetScene<GameScene>().SendSpawnObject(item.item.name, item.packageCode, type);
            }
        });

        base.Init();
    }

    public override void Setting(ObjectListStruct item)
    {
        base.Setting(item);


        if (item.isDir)
        {
            background.SetActive(false);
            objectImage.sprite = dirImage;
        }
        else if (item.isPreset)
        {
            background.SetActive(false);
            objectImage.sprite = presetImage;
        }
        else
        {
            background.SetActive(true);
            // TODO: set obj image
            // objectImage.sprite = item.item.image;
        }
        objectName.text = item.item.name;

    }
}
