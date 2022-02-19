using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleBar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private const int gap = 20;

    private Vector2 offset;
    RectTransform p;

    private void Start()
    {
        p = transform.parent.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = eventData.position - (Vector2)p.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mp = eventData.position;
        mp.x = Mathf.Clamp(mp.x, gap, Screen.width - gap);
        mp.y = Mathf.Clamp(mp.y, gap, Screen.height - gap);

        p.position = mp - offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
