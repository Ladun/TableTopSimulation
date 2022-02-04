using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableObjectSet : TableObject
{
    public enum SortType
    {
         Overlap,
         Horizontal,
         Vertical
    }

    [Header("TableObjectSet Properties")]
    public SortType sortType;
    public List<TableObject> containedObjects = new List<TableObject>();
    public float dist = .1f;
    public float yDist = 0;

    public override void Init()
    {
        eventDict.Add(TableObjectEventType.Over, OverEvent);
        eventDict.Add(TableObjectEventType.Select, SelectEvent);
        eventDict.Add(TableObjectEventType.Lock, LockEvent);
        eventDict.Add(TableObjectEventType.Merge, MergeEvent);
        eventDict.Add(TableObjectEventType.Pick, PickEvent);
        eventDict.Add(TableObjectEventType.Shuffle, ShuffleEvent);

    }

    public override void ApplyMovement()
    {
        if (Selected)
        {
            transform.position = Vector3.SmoothDamp(transform.position, movePos, ref moveSmooth.smoothVelocity, moveSmooth.smoothTime);
            transform.eulerAngles = Utils.SmoothDampEuler(transform.eulerAngles, moveAngle, ref rotateSmooth.smoothVelocity, rotateSmooth.smoothTime);

            UpdateItemPosition();
        }
    }

    public void Add(TableObject  to) {


        containedObjects.Add(to);
        to.parentSet = this;
        to.transform.SetParent(transform);

        to.SetMergeRigidbody();

        UpdateItemPosition();
        outline.UpdateRendererCache();
    }
    public void Add(TableObjectSet otherSet)
    {
        foreach (TableObject to in  otherSet.containedObjects)
        {
            Add(to);
        }
        otherSet.FreeAllObject();
        Managers.Instance.GetScene<GameScene>().SendDespawnObject(otherSet.Id);
    }

    public void Remove(TableObject to)
    {   

        print("Remove : " + to.name);
        containedObjects.Remove(to);
        if (to.parentSet == this)
        {
            to.parentSet = null;
            to.transform.SetParent(null);
            to.ResetRigidbody();
        }

        if(containedObjects.Count <= 1)
            Managers.Instance.GetScene<GameScene>().SendDespawnObject(Id);

        UpdateItemPosition();
    }

    public void FreeAllObject()
    {
        foreach(TableObject to in containedObjects)
        {
            if (to.parentSet == this)
            {
                to.parentSet = null;
                to.transform.SetParent(null);
            }
        }
    }

    private void UpdateItemPosition()
    {
        for (int i = 0; i < containedObjects.Count; i++)
        {
            containedObjects[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        
        switch (sortType)
        {
            case SortType.Overlap:
                {
                    float y = 0;
                    for(int i = 0; i < containedObjects.Count; i++)
                    {
                        float boundHeight = containedObjects[i].GetComponent<BoxCollider>().bounds.size.y;
                        y += boundHeight / 2;

                        containedObjects[i].transform.localPosition = new Vector3(0, y, 0);

                        y += boundHeight / 2 + yDist;
                    }

                    break;
                }
            case SortType.Horizontal:
                {
                    float y = 0;
                    float x = 0;
                    for (int i = 0; i < containedObjects.Count; i++)
                    {
                        float boundHeight = containedObjects[i].GetComponent<BoxCollider>().bounds.size.y;
                        y += boundHeight / 2;

                        containedObjects[i].transform.localPosition = new Vector3(x, y, 0);

                        y += boundHeight / 2 + yDist;
                        x += dist;
                    }
                    break;
                }
            case SortType.Vertical:
                {
                    float y = 0;
                    float z = 0;
                    for (int i = 0; i < containedObjects.Count; i++)
                    {
                        float boundHeight = containedObjects[i].GetComponent<BoxCollider>().bounds.size.y;
                        y += boundHeight / 2;

                        containedObjects[i].transform.localPosition = new Vector3(0, y, z);

                        y += boundHeight / 2 + yDist;
                        z += dist;
                    }
                    break;
                }
        }
    }


    #region Event Handler

    private void PickEvent(ObjectEvent e, Player p)
    {
        SelectEvent(new ObjectEvent()
        {
            ObjectEventId = TableObjectEventType.Select,
            ObjectValue=0
        }, p);
        TableObject to = containedObjects[containedObjects.Count - 1];

        print(to.name);
        Remove(to);

        if (e.Flag == 1)
        {
            to.SelectEvent(new ObjectEvent()
            {
                ObjectEventId = TableObjectEventType.Select,
                ObjectValue = 1
            }, p);
        }
    }

    private void ShuffleEvent(ObjectEvent e, Player p)
    {

        for(int i = 0; i < e.ShuffleIdx.Count; i++)
        {
            TableObject to = containedObjects[i];
            containedObjects[i] = containedObjects[e.ShuffleIdx[i]];
            containedObjects[e.ShuffleIdx[i]] = to;
        }
        UpdateItemPosition();
    }

    #endregion

}
