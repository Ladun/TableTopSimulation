using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Define;

public class SimpleCameraPositionController : MonoBehaviour
{

    public SmoothDampStruct<Vector3> smoothMove;
    public SmoothDampStruct<Vector3> smoothRotate;
    private Vector3 targetPos;
    private Vector3 targetAngle;

    void Start()
    {
        targetPos = transform.position;
        targetAngle = transform.eulerAngles;
    }

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref smoothMove.smoothVelocity, smoothMove.smoothTime);
        transform.eulerAngles = Vector3.SmoothDamp(transform.eulerAngles, targetAngle, ref smoothRotate.smoothVelocity, smoothRotate.smoothTime);
    }

    public void SetTargetPos(Vector3 targetPos, Vector3 targetAngle)
    {
        this.targetPos = targetPos;
        this.targetAngle = targetAngle;
    }
}
