using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGridListViewer : MonoBehaviour
{
    public int contentPerColumn;

    private GridLayoutGroup layoutGroup;


    IEnumerator Start()
    {
        yield return null;
        layoutGroup = GetComponent<GridLayoutGroup>();

        RectTransform selfTransform = GetComponent<RectTransform>();

        float enableSizeX = selfTransform.rect.width - (layoutGroup.padding.left + layoutGroup.padding.right + (contentPerColumn - 1) * layoutGroup.spacing.x);

        layoutGroup.cellSize = new Vector2(enableSizeX / contentPerColumn, layoutGroup.cellSize.y);
    }

    public void UpdateItemList<T>(UIListContent<T> contentPrefab, List<T> items)
    {
        int i;
        for (i = 0; i < items.Count; i++)
        {
            UIListContent<T> content;
            if (i < transform.childCount)
            {
                content = transform.GetChild(i).GetComponent<UIListContent<T>>();
            }
            else
            {
                content = Instantiate(contentPrefab);
                content.transform.SetParent(transform);
                content.transform.localScale = Vector3.one;
            }
            content.gameObject.SetActive(true);
            content.Setting(items[i]);
        }
        for (; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
}
