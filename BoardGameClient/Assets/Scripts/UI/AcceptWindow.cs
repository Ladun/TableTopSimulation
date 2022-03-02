using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AcceptWindow : MonoBehaviour
{
    public TextMeshProUGUI notation;

    private System.Action _acceptAction;
    private System.Action _cancleAction;

    public void Setting(string text, System.Action acceptAction, System.Action cancleAction)
    {
        notation.text = text;
        _acceptAction = acceptAction;
        _cancleAction = cancleAction;
    }

    public void Accept()
    {
        if (_acceptAction != null)
            _acceptAction.Invoke();

        transform.gameObject.SetActive(false);
    }
    public void Cancle()
    {
        if (_cancleAction != null)
            _cancleAction.Invoke();

        transform.gameObject.SetActive(false);
    }


}
