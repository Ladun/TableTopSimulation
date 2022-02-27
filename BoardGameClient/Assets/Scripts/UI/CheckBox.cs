using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBox : MonoBehaviour
{
    private bool _checked;
    public bool IsChecked { get { return _checked; } }

    public Color normalColor;
    public Color checkedColor;


    private void Awake()
    {
        GetComponent<Image>().color = normalColor;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            _checked = !_checked;
            GetComponent<Image>().color = _checked ? checkedColor : normalColor;
        });

        
    }

}
