using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Camera Distance 
    public Vector2 distMinMax = new Vector2(2, 10);
    public float distSmoothTime = 0.1f;
    private float targetDist = 5;
    private float curDist = 5;
    private float currentDistVelocity;


    // Camera Angle
    public float mouseSensitivity = 3;
    public float angleSmoothTime = 0.1f;
    private float targetXAngle;
    private float targetYAngle;
    private float currentAngleXVelocity;
    private float currentAngleYVelocity;


    void Update()
    {

        UpdateAngle();
        UpdatePos();
    }

    private void UpdatePos()
    {

        targetDist -= Input.mouseScrollDelta.y;
        targetDist = Mathf.Clamp(targetDist, distMinMax.x, distMinMax.y);
        curDist = Mathf.SmoothDamp(curDist, targetDist, ref currentDistVelocity, distSmoothTime);

        transform.position = - transform.forward * curDist;
    }

    private void UpdateAngle()
    {
        if (!Input.GetMouseButton(1))
            return;

        targetYAngle += Input.GetAxis("Mouse X") * mouseSensitivity;
        targetXAngle -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        targetXAngle = Mathf.Clamp(targetXAngle, -90, 90);

        Vector3 current = transform.localEulerAngles;

        current.x = Mathf.SmoothDampAngle(current.x, targetXAngle, ref currentAngleXVelocity, angleSmoothTime);
        current.y = Mathf.SmoothDampAngle(current.y, targetYAngle, ref currentAngleYVelocity, angleSmoothTime);

        transform.localEulerAngles = current;
    }
}
