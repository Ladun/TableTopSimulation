using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;

    public Sprite idleSprite;
    public Color idleColor;
    public Sprite overSprite;
    public Color overColor;
    public Sprite selectSprite;
    public Color selectColor;

    private TabButton selectedButton;

    public void Subscribe(TabButton button)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }
        tabButtons.Add(button);
    }

    public void OnTabClick(TabButton button)
    {
        if(selectedButton != null)
        {
            selectedButton.Deselect();
        }

        selectedButton = button;
        selectedButton.Select();

        ResetButton();
        button.background.sprite = selectSprite;
        button.background.color = selectColor;

    }

    public void OnTabEnter(TabButton button)
    {
        ResetButton();
        if (selectedButton != button)
        {
            button.background.sprite = overSprite;
            button.background.color = overColor;
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetButton();
    }

    public void ResetButton()
    {
        foreach(TabButton button in tabButtons)
        {
            if (selectedButton != button)
            {
                button.background.sprite = idleSprite;
                button.background.color = idleColor;
            }
        }
    }   
}
