using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListTooltipItem : MonoBehaviour
{
    public System.Action _event;

    public void DoEvent()
    {
        _event?.Invoke();
    }
}
