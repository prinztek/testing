using UnityEngine;
using UnityEngine.UI;  // For handling Button component

public class TabGroupButton : MonoBehaviour
{
    public TabGroup tabGroup;  // Reference to the TabGroup
    public Button button;  // The Button component of the tab button
    private Image buttonImage;  // The Image component to change colors
    public Color selectedColor = Color.blue;  // Color when the tab is selected
    public Color deselectedColor = Color.gray;  // Color when the tab is deselected

    void Start()
    {
        tabGroup = GetComponentInParent<TabGroup>();
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();  // Ensure this finds the Image component

        if (tabGroup == null)
        {
            Debug.LogError("TabGroupButton must be a child of a TabGroup.");
            return;
        }

        // Subscribe this button to the TabGroup
        tabGroup.Subscribe(this);

        // Add listener for button click
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }

        // Ensure buttonImage is not null before accessing it
        if (buttonImage == null)
        {
            Debug.LogError("Button Image component is missing from TabGroupButton.");
            return;
        }

        // If this is the default tab, visually mark it as selected
        if (tabGroup.selectedTab == this)
        {
            Select();
        }
    }

    // When the tab button is selected
    public void Select()
    {
        buttonImage.color = selectedColor;  // Change color of the selected tab
    }

    // When the tab button is deselected
    public void Deselect()
    {
        buttonImage.color = deselectedColor;  // Revert color when deselected
    }

    // When the tab button is clicked
    private void OnClick()
    {
        tabGroup.OnTabSelected(this);  // Notify TabGroup that this button is selected
    }

    // Hover effect when the mouse enters the button area
    public void OnTabEntered()
    {
        buttonImage.color = Color.yellow;  // Change color to yellow on hover
    }

    // Hover effect when the mouse exits the button area
    public void OnTabExited()
    {
        if (tabGroup.selectedTab != this)  // Don't revert color if the tab is selected
        {
            buttonImage.color = deselectedColor;
        }
    }
}
