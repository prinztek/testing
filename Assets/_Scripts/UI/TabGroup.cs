using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabGroupButton> tabButtons;  // List of tab buttons (children of TabArea)
    public TabGroupButton selectedTab;  // The currently selected tab button
    public List<GameObject> pages;  // List of pages (children of PageArea)

    void Start()
    {
        if (tabButtons.Count > 0)
        {
            // Automatically select the default tab (MathQuestion in your case)
            OnTabSelected(tabButtons[0]);
        }
    }

    // Subscribe a TabGroupButton to this TabGroup
    public void Subscribe(TabGroupButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabGroupButton>();
        }
        tabButtons.Add(button);
    }

    // When a tab button is selected, show the corresponding page and hide others
    public void OnTabSelected(TabGroupButton button)
    {
        // Deselect previous tab if it's different
        if (selectedTab != null && selectedTab != button)
        {
            selectedTab.Deselect();
        }

        // Always update selectedTab and call Select (even if re-clicking the same tab)
        selectedTab = button;
        selectedTab.Select();

        // Get the index of the clicked tab
        int index = tabButtons.IndexOf(button);

        // 🔥 Always update all pages — show the selected, hide others
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == index);  // Only show the selected page

            // If it's the default (MathQuestion), make sure to reset or re-show the page
            if (i == index && pages[i].activeSelf == false)
            {
                pages[i].SetActive(true);  // Ensure it gets reactivated when clicked again
            }
        }
    }



}
