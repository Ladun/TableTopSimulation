using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class TableObject : MonoBehaviour
{
    public int Id;

    public TableObjectSet parentSet;

    protected Outline outline;
    private Rigidbody rigidBody;

    protected Dictionary<TableObjectEventType, System.Action<ObjectEvent, Player>> eventDict = new Dictionary<TableObjectEventType, System.Action<ObjectEvent, Player>>();

    protected Vector3 movePos;
    public SmoothDampStruct<Vector3> moveSmooth;
    protected Vector3 moveAngle;
    public SmoothDampStruct<Vector3> rotateSmooth;

    public Player Owner;

    public bool Over = false;
    public bool Selected {
        get
        {
            return Owner != null;
        }
    }

    public bool Lock;

    public bool animatedMove;

    private Color defaultColor
    {
        get
        {
            if (Lock)
                return Color.gray;
            else
                return Color.white;
        }
    }

    [Header("For UI")]
    public Sprite image;

    protected void BasicSetting()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;

        rigidBody = GetComponent<Rigidbody>();
    }

    public virtual void Init()
    {
        BasicSetting();

        eventDict.Add(TableObjectEventType.Over, OverEvent);
        eventDict.Add(TableObjectEventType.Select, SelectEvent);
        eventDict.Add(TableObjectEventType.Lock, LockEvent);
        eventDict.Add(TableObjectEventType.Merge, MergeEvent);
    }

    private void Update()
    {
        ApplyMovement();   
    }

    public virtual void ApplyMovement()
    {
        // 선택이 되었을 때만 특정 사용자에 의해서 이동하기 때문에
        // Selected 상태에서만 이동 동기화
        if (parentSet == null)
        {
            if (Selected || animatedMove)
            {
                transform.position = Vector3.SmoothDamp(transform.position, movePos, ref moveSmooth.smoothVelocity, moveSmooth.smoothTime);
                transform.eulerAngles = Utils.SmoothDampEuler(transform.eulerAngles, moveAngle, ref rotateSmooth.smoothVelocity, rotateSmooth.smoothTime);
                if (rigidBody)
                {
                    rigidBody.velocity = Vector3.zero;
                    rigidBody.angularVelocity = Vector3.zero;
                }

                if(animatedMove &&
                    Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(movePos.x, 0 ,movePos.z)) < 0.001f &&
                    Vector3.Distance(new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z), new Vector3(moveAngle.x, 0, moveAngle.z)) < 0.001f)
                {
                    animatedMove = false;
                }
            }
        }
    }

    public void SetMoveInfo(Vector3 point, Vector3 angle, bool sync)
    {
        movePos = point;
        moveAngle = angle;
        if (sync)
        {
            transform.position = movePos;
            transform.eulerAngles = moveAngle;
        }

        animatedMove = true;

    }

    public virtual void DoEvent(ObjectEvent e, Player p)
    {
        System.Action<ObjectEvent, Player> eventHandler = null;


        if(eventDict.TryGetValue(e.ObjectEventId, out eventHandler))
        {
            eventHandler.Invoke(e, p);
        }
    }

    public void SetMergeRigidbody()
    {
        rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
    }
    public void ResetRigidbody()
    {
        rigidBody.constraints = RigidbodyConstraints.None;
    }

    public GameObjectType GetGameObjectType()
    {
        return ObjectManager.GetObjectTypeById(Id);
    }

    #region Event Handler

    protected void OverEvent(ObjectEvent e, Player p)
    {


        if (e.ObjectValue == 1) // Over
        {
            if (!outline.enabled)
            {
                Over = true;
                if(!Selected)
                    outline.OutlineColor = defaultColor;
                outline.enabled = true;
            }
        }
        else // Unover
        {

            if (outline.enabled)
            {
                Over = false;
                outline.enabled = false;
            }
        }
    }

    public void SelectEvent(ObjectEvent e, Player p)
    {

        if (e.ObjectValue == 1) // Select
        {
            if (Owner == null)
            {
                if (!Over)
                    OverEvent(new ObjectEvent()
                    {
                        ObjectEventId = TableObjectEventType.Over,
                        ObjectValue = 1
                    }, p);

                Owner = p;
                outline.OutlineColor = p.playerColor;
                if (rigidBody)
                {
                    rigidBody.useGravity = false;
                    rigidBody.velocity = Vector3.zero;
                }
            }
        }
        else // Deselect
        {
            if (Owner == p)
            {
                if (Over)
                    OverEvent(new ObjectEvent()
                    {
                        ObjectEventId = TableObjectEventType.Over,
                        ObjectValue = 0
                    }, p);
                Owner = null;
                outline.OutlineColor = defaultColor;
                if (rigidBody)
                {
                    rigidBody.useGravity = true;
                }
            }
        }
    }

    protected void LockEvent(ObjectEvent e, Player p)
    {

        if (!Selected)
        {
            if (e.ObjectValue == 1) // Lock
            {
                Lock = true;
            }
            else // Unlock
            {
                Lock = false;
            }
        }
    }


    protected void MergeEvent(ObjectEvent e, Player p)
    {
        TableObject to = Managers.Instance.GetScene<GameScene>().objectManager.Find<TableObject>(e.ObjectValue);
        if (to == null)
            return;

        if (GetGameObjectType() == GameObjectType.TableObject) // Self is T.O
        {
            if (to.parentSet == null) // Other is T.O
            {
                TableObjectSet set = Managers.Instance.GetScene<GameScene>().objectManager.Find<TableObjectSet>(e.Flag);

                set.Add(to);
                set.Add(this);
            }
            else // Other is Set
            {
                TableObjectSet set = to.parentSet;
                set.Add(this);
            }
            OverEvent(new ObjectEvent()
            {
                ObjectEventId = TableObjectEventType.Over,
                ObjectValue = 0
            }, p);
            SelectEvent(new ObjectEvent()
            {
                ObjectEventId = TableObjectEventType.Select,
                ObjectValue = 0
            }, p);
        }
        else // Self is Set
        {
            if (to.parentSet != null) // Other is Set
            {
                TableObjectSet set = to.parentSet;
                set.Add(this as TableObjectSet);
            }
        }
    }

    #endregion

}
