using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{

    public GameObject targetObject;

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {

    }

    protected virtual void CustomUpdate()
    {

    }

    public  virtual void Setting(GameObject obj)
    {
        this.targetObject = obj;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 w2s = Camera.main.WorldToScreenPoint(targetObject.transform.position);
        transform.position = w2s;

        CustomUpdate();
    }
}
