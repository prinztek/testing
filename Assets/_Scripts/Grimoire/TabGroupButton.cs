using UnityEngine;
using UnityEngine.UI;  // For Button and Image components
using TMPro;  // For TextMeshPro

public class TabGroupButton : MonoBehaviour
{
    public TabGroup tabGroup;   // Reference to the TabGroup
    public Button button;       // Reference to the Button component
    private TextMeshProUGUI buttonText;    // Reference to the TextMeshProUGUI component inside the button

    void Awake()
    {
        // Ensure button is assigned via the inspector or by GetComponent if needed
        if (button == null)
        {
            button = GetComponent<Button>(); // Try to get Button if not assigned in Inspector
        }

        // Ensure buttonText is assigned by finding it within the button's children (TMP version)
        if (button != null)
        {
            buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("Button component is missing on the GameObject.");
        }

        // Set default colors
        // selectedColor = new Color32(238, 225, 211, 255);
        // deselectedColor = new Color32(231, 139, 80, 255);
    }

    void Start()
    {
        // Ensure tabGroup is assigned properly
        tabGroup = GetComponentInParent<TabGroup>();
        if (tabGroup == null)
        {
            Debug.LogError("TabGroupButton must be a child of a TabGroup.");
            return;
        }

        // Subscribe to the tabGroup
        tabGroup.Subscribe(this);

        // Add onClick listener to the button if it's assigned
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }

        // Ensure we properly set the selected/deselected state at the start
        if (tabGroup.selectedTab == this)
        {
            Select();
        }
        else
        {
            Deselect();
        }
    }

    public void Select()
    {
        // Ensure buttonText exists before trying to change color
        if (buttonText != null)
        {
            // buttonText.color = Color.black;  // Set the TMP text color to violetRed
        }
        else
        {
            Debug.LogWarning("Button Text (TMP) is missing. Make sure the button has a TMP Text component.");
        }
    }

    public void Deselect()
    {
        // Set the button's text to a default color (for deselected state)
        // buttonText.color = new Color32(238, 225, 211, 255);
        // Light Beige / Off-white color for deselected state
        // Alternatively, you could use gray or transparent color:
        // buttonText.color = Color.gray; // Or a semi-transparent color
        // buttonText.color = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent white
    }


    private void OnClick()
    {
        // Notify the TabGroup that this tab was selected
        tabGroup.OnTabSelected(this);
    }
}
