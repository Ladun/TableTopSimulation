using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public LayerMask tableMask;

    // Camera Distance 
    public Vector2 distMinMax = new Vector2(2, 10);
    public float distSmoothTime = 0.1f;
    private float targetDist = 5;
    private float curDist = 5;
    private float currentDistVelocity;

    // Camera Position
    public float speed = 5;
    public float posSmoothTime = 0.1f;
    public bool freezePos = true;
    public Vector3 targetPos;
    private Vector3 curPos;
    private Vector3 currentPosVelocity;
    
    //      Move By mouse
    private Vector3 lastMousePos;
    private Vector3 lastPos;

    // Camera Angle
    public float mouseSensitivity = 3;
    public float angleSmoothTime = 0.1f;
    private float targetXAngle;
    private float targetYAngle;
    private float currentAngleXVelocity;
    private float currentAngleYVelocity;

    private bool isInit;
    public void Init(Vector3 playerPos)
    {
        Vector3 current = transform.eulerAngles;
        current.y = Mathf.Atan2(-playerPos.x, -playerPos.z) * Mathf.Rad2Deg;
        targetXAngle = current.x;
        targetYAngle = current.y;
        curPos = targetPos = playerPos;
        transform.eulerAngles = current;

        isInit = true;
    }

    private void Update()
    {
        if (!isInit)
            return;

        UpdateAngle();
        UpdatePos();        
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

    private void UpdatePos()
    {
        targetDist -= Input.mouseScrollDelta.y;
        targetDist = Mathf.Clamp(targetDist, distMinMax.x, distMinMax.y);
        curDist = Mathf.SmoothDamp(curDist, targetDist, ref currentDistVelocity, distSmoothTime);

        if (!freezePos)
        {
            if (Input.GetMouseButtonDown(2))
            {
                lastMousePos = GetMousePos();
                lastPos = curPos;
            }
            if (Input.GetMouseButton(2))
            {
                Vector3 delta = GetMousePos() - lastMousePos;
                float h = -delta.x;
                float v = -delta.y;
                float r = transform.eulerAngles.y * Mathf.Deg2Rad;

                //lastMousePos = Input.mousePosition;
                targetPos = lastPos + new Vector3(v * Mathf.Sin(r) + h * Mathf.Cos(r), 0, v * Mathf.Cos(r) + h * -Mathf.Sin(r)) * speed ;
            }
            else
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                float r = transform.eulerAngles.y * Mathf.Deg2Rad;
                targetPos += new Vector3(v * Mathf.Sin(r) + h * Mathf.Cos(r), 0, v * Mathf.Cos(r) + h * -Mathf.Sin(r)) * speed * Time.deltaTime;
            }

            //targetPos += (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * speed;
        }

        curPos = Vector3.SmoothDamp(curPos, targetPos, ref currentPosVelocity, posSmoothTime);
        transform.position = curPos - transform.forward * curDist;
    }

    public void SetTargetPos(Vector3 targetPos)
    {
        this.targetPos = targetPos;
    }

    private Vector3 GetMousePos()
    {
        return Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    public Vector3 GetUpperTablePosition()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100, tableMask))
        {

            return hit.point;
        }
        return new Vector3(0, 3, 0);
    }
}
