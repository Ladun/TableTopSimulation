using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListTooltip : Tooltip
{
    protected int curIdx;
    public Color indexColor;
    public Color unIndexColor;

    protected override void Init()
    {
        SetColor();
    }

    protected override void CustomUpdate()
    {
    }

    public override void Setting(GameObject obj)
    {
        this.targetObject = obj;
    }

    protected void ChangeIndex()
    {
        int maxIdx = GetMaxIndex();
        curIdx = (curIdx + 1) % maxIdx;
        SetColor();
    }

    protected void SetColor()
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            Image pimage = transform.GetChild(i).GetComponent<Image>();
            pimage.color = i == curIdx ? indexColor : unIndexColor;
        }
    }

    protected ListTooltipItem GetIndexChild()
    {
        int c = 0;
        for(int i = 0;i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                if (c == curIdx)
                    return transform.GetChild(i).GetComponent<ListTooltipItem>();
                c++;
            }
        }

        return null;
    }

    protected int GetMaxIndex()
    {
        int r = 0;
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
                r++;
        }
        return r;
    }
}
