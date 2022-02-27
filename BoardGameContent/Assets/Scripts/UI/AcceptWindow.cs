using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptWindow : MonoBehaviour
{
    private System.Action _acceptAction;
    private System.Action _cancleAction;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Open(System.Action acceptAction, System.Action cancleAction)
    {

        gameObject.SetActive(true);
        _acceptAction = acceptAction;
        _cancleAction = cancleAction;
    }


    // AcceptWindow -> Accept
    public void Accept()
    {
        if (_acceptAction != null)
            _acceptAction.Invoke();
        gameObject.SetActive(false);
    }

    // AcceptWindow -> Cancle
    public void Cancle()
    {
        if (_cancleAction != null)
            _cancleAction.Invoke();
        gameObject.SetActive(false);
    }
}
