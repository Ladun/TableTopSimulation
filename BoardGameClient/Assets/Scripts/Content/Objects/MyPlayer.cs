using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{

    struct MovePacketInfo
    {
        public int id;
        public Vector3 pos;
        public Vector3 angle;

        public MovePacketInfo(int id, Vector3 pos, Vector3 angle)
        {
            this.id = id;
            this.pos = pos;
            this.angle = angle;
        }
    }

    private static float lowerPacketSendDist = 0.005f;
    private static float lowerPacketSendAngle = 1f;

    private static float sendFrequency = 1 / 2f;
    private float sendTime = 0;

    public float cardRotateSensivity = 1;

    private Vector3 lastMousePos;
    private Vector3 targetAngle;
    private Vector3 moveOffset;


    List<ObjectEvent> eventedList = new List<ObjectEvent>();
    List<MovePacketInfo> moveInfoList = new List<MovePacketInfo>();


    protected override void UpdateMovePoint()
    {
        sendTime += Time.deltaTime;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit tableObjectHit;
        RaycastHit tableHit;

        bool rayto = Physics.Raycast(ray, out tableObjectHit, 30, tableObjectLayer);
        bool rayt = Physics.Raycast(ray, out tableHit, 30, tableLayer);

        // TableObject Event ===============
        int tableObjectId = 0;
        if (rayto)
        {
            TableObject newTableObject = tableObjectHit.collider.GetComponent<TableObject>();
            if (tableObject == null || (!tableObject.Selected && tableObject != newTableObject && tableObject != newTableObject.parentSet))
            {
                if(tableObject != null)
                {
                    AddObjectEventPacketInfo(tableObject, TableObjectEventType.Over, 0);
                }
                tableObject = newTableObject;
            }
            if (tableObject)
            {
                if (tableObject.parentSet != null)
                    tableObject = tableObject.parentSet;
                tableObjectId = tableObject.Id;
                // Send: Object Over
                if (!tableObject.Over)
                {
                    // tableObject.Over(this);
                    AddObjectEventPacketInfo(tableObject, TableObjectEventType.Over, 1);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (!tableObject.Selected)
                    {
                        // Send: Object Select
                        //tableObject.Select(this);
                        AddObjectEventPacketInfo(tableObject, TableObjectEventType.Select, 1);
                        targetAngle = tableObject.transform.eulerAngles;
                        Managers.Instance.GetScene<GameScene>().cam.freezePos = true;

                        moveOffset = new Vector3(tableObjectHit.point.x, 0, tableObjectHit.point.z) - new Vector3(tableObject.transform.position.x, 0, tableObject.transform.position.z);
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (tableObject.Selected)
                    {
                        ReleaseTableObject();
                    }
                }
            }
        }
        else
        {
            if (tableObject)
            {
                tableObjectId = tableObject.Id;
                if (!tableObject.Selected) {
                    if (Managers.Instance.GetUIManager<GameUIManager>().ActiveSelfTooltip("ObjectControlTooltip"))
                    {
                        Managers.Instance.GetUIManager<GameUIManager>().InteractTooltip("ObjectControlTooltip", tableObject.gameObject);
                    }
                    // Send: Object Unover
                    //tableObject.Unover(this);
                    AddObjectEventPacketInfo(tableObject, TableObjectEventType.Over, 0);
                    tableObject = null;
                }
                else
                {
                    if (Input.GetMouseButtonUp(0))
                    {

                        ReleaseTableObject();
                    }
                }
            }
        }

        if (eventedList.Count > 0)
        {
            if (tableObjectId != 0)
            {
                C_Interact interactPacket = new C_Interact();
                interactPacket.ObjectId = tableObjectId;
                interactPacket.Events.Add(eventedList);

                Managers.Instance.Network.Send(interactPacket);
            }
            eventedList.Clear();
        }

        // TableObject, Marker Move ===============
        if (rayt)
        {
            Vector3 markerPos = (tableObject == null)? tableHit.point + Vector3.up * distanceFromTable : new Vector3(tableHit.point.x, tableObject.transform.position.y + distanceFromTable, tableHit.point.z);
            // Send Marker Move Packet
            AddMovePacketInfo(Id, marker.transform, markerPos, new Vector3(0, Camera.main.transform.eulerAngles.y, 0));

            // Send Tableobject Move packet
            if (tableObject)
            {
                // Table Object Follow mouse
                if (tableObject.Selected && tableObject.Owner == this)
                {
                    Vector3 targetPos = new Vector3(tableObject.transform.position.x, tableHit.point.y, tableObject.transform.position.z) ;
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        if(ObjectManager.GetObjectTypeById(tableObject.Id) != GameObjectType.TableObjectSet)
                            targetAngle.z += 180;
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.R) && tableObject.Lock)
                        {
                            // 마우스 움직임에 따른 TableObject 각도 변화
                            Vector3 delta = Utils.GetViewPortMousePos() - lastMousePos;

                            targetAngle.y += delta.x * cardRotateSensivity;
                        }
                        else if (!tableObject.Lock)
                        {
                            targetPos = tableHit.point - moveOffset;
                        }
                    }
                    if(tableObject.GetGameObjectType() != GameObjectType.Preset)
                        targetPos.y += distanceFromTable * 2;


                    AddMovePacketInfo(tableObject.Id, tableObject.transform, targetPos, targetAngle);
                }
            }

            if (sendTime > sendFrequency)            
                sendTime -= sendFrequency;

            movePoint = tableHit.point;
            markerAngle = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
        }

        if(moveInfoList.Count > 0)
        {
            C_Move movePacket = new C_Move();

            foreach(MovePacketInfo m in moveInfoList)
            {
                movePacket.ObjectId.Add(m.id);
                movePacket.Pos.Add(Utils.Vector32Dim3Info(m.pos));
                movePacket.Angle.Add(Utils.Vector32Dim3Info(m.angle));
            }
            Managers.Instance.Network.Send(movePacket);
            moveInfoList.Clear();
        }

        // Open Tooltip
        if(tableObject && tableObject.Over && ! tableObject.Selected)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Managers.Instance.GetUIManager<GameUIManager>().InteractTooltip("ObjectControlTooltip", tableObject.gameObject);
            }
        }

        lastMousePos = Utils.GetViewPortMousePos();
    }


    private void AddMovePacketInfo(int id, Transform cur, Vector3 targetPos, Vector3 targetAngle)
    {

        if (Vector3.Distance(cur.position, targetPos) > lowerPacketSendDist || 
            Vector3.Distance(cur.eulerAngles, targetAngle) > lowerPacketSendAngle)
        {
            moveInfoList.Add(new MovePacketInfo(id, targetPos, targetAngle));
        }
        else
        {
            if (sendTime > sendFrequency)
            {
                if (Vector3.Distance(cur.position, targetPos) > Mathf.Epsilon ||
                    Vector3.Distance(cur.eulerAngles, targetAngle) > Mathf.Epsilon)
                {
                    moveInfoList.Add(new MovePacketInfo(id, targetPos, targetAngle));
                }
            }
        }
    }

    private void AddObjectEventPacketInfo(TableObject to, TableObjectEventType eventId, int eventValue, int flag = 0)
    {
        ObjectEvent objectEvent = new ObjectEvent();
        objectEvent.ObjectEventId = eventId;
        objectEvent.ObjectValue = eventValue;
        objectEvent.Flag = flag;
        if(eventId == TableObjectEventType.Over || eventId == TableObjectEventType.Select)
            to.DoEvent(objectEvent, this);
        eventedList.Add(objectEvent);
    }

    private void ReleaseTableObject()
    {

        // Send: Object Deselect
        //tableObject.Deselect(this);
        AddObjectEventPacketInfo(tableObject, TableObjectEventType.Select, 0);
        Managers.Instance.GetScene<GameScene>().cam.freezePos = false;

        int p = tableObject.gameObject.layer;
        // Layer 무효화
        if (tableObject.GetGameObjectType() == GameObjectType.TableObjectSet)
        {
            foreach (TableObject to in (tableObject as TableObjectSet).containedObjects)
                to.gameObject.layer = 0;
        }
        else
        {
            tableObject.gameObject.layer = 0;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit tableObjectHit;
        bool rayto = Physics.Raycast(ray, out tableObjectHit, 30, tableObjectLayer);

        if (rayto)
        {
            TableObject to = tableObjectHit.collider.GetComponent<TableObject>();
            if (to != null)
            {
                GameObjectType targetType = to.GetGameObjectType();

                GameObjectType curType = tableObject.GetGameObjectType();

                if (targetType == GameObjectType.Preset)
                {
                    Preset preset = to as Preset;

                    if (curType == GameObjectType.TableObject)
                    {
                        Transform near = preset.GetNear(tableObject.transform.position);
                        if (near != null)
                        {
                            moveInfoList.Add(new MovePacketInfo(tableObject.Id, near.position + Vector3.up * 0.1f, near.eulerAngles));
                            tableObject.animatedMove = true;
                        }
                    }
                    else if (curType == GameObjectType.TableObjectSet)
                    {
                        float h = 0.05f;
                        TableObjectSet set = tableObject as TableObjectSet;
                        for (int i = 0; i < set.containedObjects.Count && i < preset.transform.childCount; i++)
                        {
                            Transform target = preset.transform.GetChild(i);
                            TableObject targetTo = set.containedObjects[set.containedObjects.Count - 1 - i];

                            AddObjectEventPacketInfo(tableObject, TableObjectEventType.Pick, 0);
                            moveInfoList.Add(new MovePacketInfo(targetTo.Id, target.position + Vector3.up * h, target.eulerAngles));

                            targetTo.animatedMove = true;
                            targetTo.gameObject.layer = p;

                            h += 0.05f;
                        }
                    }
                }
                else
                {
                    if (curType != GameObjectType.Preset)
                    {
                        AddObjectEventPacketInfo(tableObject, TableObjectEventType.Merge, to.Id);
                    }
                }
            }
        }

        // Layer 무효화
        if (tableObject.GetGameObjectType() == GameObjectType.TableObjectSet)
        {
            foreach (TableObject to in (tableObject as TableObjectSet).containedObjects)
                to.gameObject.layer = p;
        }
        else
        {
            tableObject.gameObject.layer = p;
        }
    }

    protected override void MoveMaker()
    {
        Vector3 markerPos = movePoint + Vector3.up * distanceFromTable;
        if (tableObject)
        {
            markerPos.y = tableObject.transform.position.y + distanceFromTable;
        }
        marker.transform.position = markerPos;
        marker.transform.eulerAngles = markerAngle;
    }
}
