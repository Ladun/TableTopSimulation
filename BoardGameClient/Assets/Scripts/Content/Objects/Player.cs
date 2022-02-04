using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Define;

public class Player : MonoBehaviour
{
    public int Id;

    public LayerMask tableObjectLayer;
    public LayerMask tableLayer;
    public float cardRotateSensitivity;
    public Color playerColor;

    // Marker
    public Marker marker;
    protected Vector3 movePoint;
    public SmoothDampStruct<Vector3> markerMoveSmooth;
    protected Vector3 markerAngle;
    public SmoothDampStruct<Vector3> markerRotateSmooth;

    // Table Object
    protected TableObject tableObject;

    public float distanceFromTable;

    private void Update()
    {
        UpdateMovePoint();
        MoveMaker();
    }

    protected virtual void UpdateMovePoint()
    {
    }

    protected virtual void MoveMaker()
    {
        Vector3 markerPos = movePoint + Vector3.up * distanceFromTable;
        marker.transform.position = Vector3.SmoothDamp(marker.transform.position, markerPos, ref markerMoveSmooth.smoothVelocity, markerMoveSmooth.smoothTime);

        marker.transform.eulerAngles = Vector3.SmoothDamp(marker.transform.eulerAngles, markerAngle, ref markerRotateSmooth.smoothVelocity, markerRotateSmooth.smoothTime);
        
    }

    public void SetColor(Color c)
    {
        playerColor = c;
        marker.SetColor(c);
    }

    public void SetMoveInfo(Vector3 point, Vector3 angle, bool sync)
    {
        movePoint = point;
        markerAngle = angle;

        if (sync)
        {
            marker.transform.position = movePoint;
            marker.transform.eulerAngles = markerAngle;
        }
    }


}
