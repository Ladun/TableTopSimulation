using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleSizeLayout : MonoBehaviour
{

    public enum FixedType
    {
        Width
    }

    public FixedType fixedType;
    public Vector2 space;


    public void Add(RectTransform content)
    {
        UpdateContentPosition();
    }

    public void Remove(RectTransform content)
    {
            UpdateContentPosition();
    }

    [ContextMenu("Update Position")]
    public void UpdateContentPosition()
    {


        float x = 0;
        float y = 0;

        float totalHeight = 0;

        RectTransform rt = GetComponent<RectTransform>();
        float maxHeight = 0;
        for(int i = 0; i < transform.childCount; i++)
        {
            RectTransform c = transform.GetChild(i).GetComponent<RectTransform>();

            c.anchorMin = new Vector3(0, 1);
            c.anchorMax = new Vector3(0, 1);

            if(x + c.rect.width > rt.rect.width)
            {
                totalHeight += maxHeight + space.y; ;

                x = 0;
                y -= maxHeight + space.y;

                maxHeight = 0;
            }

            c.anchoredPosition = new Vector3(x, y);

            x += c.rect.width + space.x;
            if(maxHeight == 0 || c.rect.height > maxHeight)
            {
                maxHeight = c.rect.height;
            }
            
        }
        totalHeight += maxHeight;

        rt.sizeDelta = new Vector2(rt.sizeDelta.x, totalHeight);
    }
}
