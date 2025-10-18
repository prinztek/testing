using UnityEngine;
using UnityEngine.UI;  // For handling Button component

public class TabGroupButton : MonoBehaviour
{
    public TabGroup tabGroup;  // Reference to the TabGroup
    public Button button;  // The Button component of the tab button
    private Image buttonImage;  // The Image component to change colors

    // Updated colors
    public Color selectedColor = new Color32(238, 225, 211, 255);  // Color when the tab is selected (EEE1D3)
    public Color deselectedColor = new Color32(231, 139, 80, 255);  // Color when the tab is deselected (E78B50)
    void Awake()
    {
        // Force the color values in case they're overridden in the Inspector
        selectedColor = new Color32(238, 225, 211, 255);
        deselectedColor = new Color32(231, 139, 80, 255);
    }

    void Start()
    {
        tabGroup = GetComponentInParent<TabGroup>();
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if (tabGroup == null)
        {
            Debug.LogError("TabGroupButton must be a child of a TabGroup.");
            return;
        }

        tabGroup.Subscribe(this);

        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }

        if (buttonImage != null)
        {
            buttonImage.color = selectedColor;
        }

        // If this is the selected tab, apply selected color
        if (tabGroup.selectedTab == this)
        {
            Select();
        }
        else
        {
            // Otherwise, apply deselected color
            Deselect();
        }
    }


    // When the tab button is selected
    public void Select()
    {
        buttonImage.color = selectedColor;  // Change color of the selected tab
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.fontStyle = FontStyle.Bold;
        }
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
}
