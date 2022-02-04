using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TabGroup group;

    public Image background;

    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        group?.Subscribe(this);
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        group.OnTabClick(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        group.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        group.OnTabExit(this);

    }

    public void Select()
    {
        if (onTabSelected != null)
            onTabSelected.Invoke();
    }

    public void Deselect()
    {
        if (onTabDeselected != null)
            onTabDeselected.Invoke();
    }
}
