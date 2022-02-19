using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("Button Options")]
    public Image graphic;
    public Color normalColor;
    public Color overColor;
    public Color downColor;

    public System.Action<CustomButton> onClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        graphic.color = downColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        graphic.color = overColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        graphic.color = normalColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        graphic.color = overColor;
    }
}
