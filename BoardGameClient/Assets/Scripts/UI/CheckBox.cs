using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBox : MonoBehaviour
{
    private bool _checked;
    public bool IsChecked
    {
        get
        {
            return _checked;
        }
        set
        {
            _checked = value;
            graphic.color = _checked ? checkedColor : normalColor;
        }
    }

    public Color normalColor;
    public Color checkedColor;

    private Image graphic;


    private void Awake()
    {
        graphic = GetComponent<Image>();

        GetComponent<Image>().color = normalColor;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            IsChecked = !_checked;
        });        
    }

}
