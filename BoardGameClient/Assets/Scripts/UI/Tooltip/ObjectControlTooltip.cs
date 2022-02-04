using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Google.Protobuf.Protocol;

public class ObjectControlTooltip : ListTooltip
{

    public ListTooltipItem deleteItem;
    public ListTooltipItem lockItem;
    public ListTooltipItem pickItem;
    public ListTooltipItem shuffleItem;


    protected override void Init()
    {
        base.Init();

        deleteItem._event = DeleteObject;
        lockItem._event = LockObject;
        pickItem._event = PickObject;
        shuffleItem._event = ShuffleObject;

    }

    protected override void CustomUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeIndex();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetIndexChild().DoEvent();
            Managers.Instance.GetUIManager<UIManager>().CloseTooltip(this);
        }
    }

    public override void Setting(GameObject obj)
    {

        base.Setting(obj);
        TableObject to = targetObject.GetComponent<TableObject>();
        if (to == null)
            return;

        if (to.Selected)
            lockItem.gameObject.SetActive(false);
        else
        {
            lockItem.gameObject.SetActive(true);
            if (to.Lock)
            {
                lockItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Unlock";
            }
            else
            {
                lockItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Lock";
            }
        }

        GameObjectType got = ObjectManager.GetObjectTypeById(to.Id);
        if(got == GameObjectType.TableObjectSet)
        {
            if ((to as TableObjectSet).containedObjects.Count > 0)
            {
                pickItem.gameObject.SetActive(true);
                shuffleItem.gameObject.SetActive(true);
            }
        }
        else
        {
            pickItem.gameObject.SetActive(false);
            shuffleItem.gameObject.SetActive(false);
        }


        curIdx = curIdx % GetMaxIndex();
        SetColor();
    }

    public void DeleteObject()
    {
        Debug.Log("Delete") ;
        TableObject to = targetObject.GetComponent<TableObject>();
        if (to == null)
            return;

        Managers.Instance.GetScene<GameScene>().SendDespawnObject(to.Id);

    }

    public void LockObject()
    {
        Debug.Log("Lock");
        TableObject to = targetObject.GetComponent<TableObject>();
        if (to == null)
            return;

        Managers.Instance.GetScene<GameScene>().SendLockObject(to, !to.Lock);
    }

    public void PickObject()
    {
        Debug.Log("Pick");
        TableObjectSet to = targetObject.GetComponent<TableObjectSet>();
        if (to == null)
            return;

        Managers.Instance.GetScene<GameScene>().SendPickObject(to );

    }

    public void ShuffleObject()
    {
        Debug.Log("Shuffle");
        TableObject to = targetObject.GetComponent<TableObject>();
        if (to == null)
            return;

        Managers.Instance.GetScene<GameScene>().SendShuffleObject(to);
    }
}
