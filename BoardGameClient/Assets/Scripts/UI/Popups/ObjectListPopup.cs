using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectListPopup : MonoBehaviour
{
    public TabGroup tabGroup;
    public RectTransform objectView;

    public TabButton buttonPrefab;

    public float verticalSpace;

    public void AddTab(string gameName)
    {
        TabButton tb = Instantiate(buttonPrefab, tabGroup.transform);
        tb.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = gameName;
        tb.group = tabGroup;
        tb.Init();

        tb.onTabSelected.AddListener(() =>
        {
            Managers.Instance.GetUIManager<GameUIManager>().ChangeGame(gameName);
        });


    }

    private void OnEnable()
    {
        StartCoroutine(ApplySize());
    }


    public IEnumerator ApplySize()
    {
        yield return null;
        for (int i = 0; i < tabGroup.transform.childCount; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)tabGroup.transform.GetChild(i));
        }

        tabGroup.GetComponent<FlexibleSizeLayout>().UpdateContentPosition();
        RectTransform tgrt = tabGroup.GetComponent<RectTransform>();
        objectView.sizeDelta = -new Vector3(0, tgrt.sizeDelta.y + verticalSpace);
        objectView.anchoredPosition = objectView.sizeDelta / 2;
    }
}
