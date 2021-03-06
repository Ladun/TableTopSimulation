using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UIListContentsInfo<P, T>
{
    public P parent;
    public UIListContent<T> prefab;
}


public class UIManager : MonoBehaviour
{
    private List<Transform> uiPopupStack = new List<Transform>();

    public LoadingWindow loadingWindow;

    public List<Tooltip> tooltips = new List<Tooltip>();


    public void Clear()
    {
        // Popup이나 Tooltip이 켜져 있을 때 esc누르면 다 꺼지고, 
        // 다 꺼져있으면 옵션 크기
        CloseAllPopupItem();
        
    }

    #region Popup System

    public Transform GetPopupItem(string n)
    {
        Transform p = uiPopupStack.FindLast(x => x.name.Equals(n));
        return p;
    }

    public void InteractPopupItem(Transform item)
    {
        if (!item.gameObject.activeSelf)
        {
            OpenPopupItem(item);
        }
        else
        {
            ClosePopupItem(item);
        }
    }
    public void OpenPopupItem(Transform item)
    {
        item.gameObject.SetActive(true);
        if (uiPopupStack.Contains(item))
        {
            uiPopupStack.Remove(item);
        }
        uiPopupStack.Add(item);
        item.SetAsLastSibling();
    }

    public void ClosePopupItem(Transform item)
    {
        item.gameObject.SetActive(false);
        Transform findItem = uiPopupStack.FindLast(x => x.name.Equals(item.name));
        if (findItem != null)
        {
            uiPopupStack.Remove(findItem);
        }

    }

    public void CloseAllPopupItem()
    {
        for(int i = uiPopupStack.Count - 1; i >= 0; i--)
        {
            uiPopupStack[i].gameObject.SetActive(false);
            uiPopupStack.RemoveAt(i);
        }
    }

    #endregion

    #region Loading 
    public void OpenLoading(string message, bool clear = true)
    {
        loadingWindow.gameObject.SetActive(true);
        loadingWindow.Setting(message, clear);
    }

    public void CloseLoading()
    {
        loadingWindow.gameObject.SetActive(false);
    }
    #endregion

    #region Tooltip System

    public bool ActiveSelfTooltip(string tooltipName)
    {

        Tooltip t = tooltips.Find(x => x.name.Equals(tooltipName));
        if (t == null)
            return false;
        return t.gameObject.activeSelf;
    }

    public void InteractTooltip(string tooltipName, GameObject target)
    {

        Tooltip t = tooltips.Find(x => x.name.Equals(tooltipName));
        if (t == null)
            return;

        if (t.gameObject.activeSelf)
        {
            if (t.targetObject == target)
                CloseTooltip(t);
            else
                OpenTooltip(t, target);
        }
        else
        {
            OpenTooltip(t, target);
        }
    }

    private void OpenTooltip(Tooltip tooltip, GameObject target)
    {

        tooltip.gameObject.SetActive(true);
        tooltip.Setting( target);
    }

    public void CloseTooltip(Tooltip tooltip)
    {
        tooltip.gameObject.SetActive(false);

    }
    #endregion
}

