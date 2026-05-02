using UnityEngine;
using UnityEngine.UI;  // For Button and Image components
using TMPro;  // For TextMeshPro

public class TabGroupButton : MonoBehaviour
{
    public TabGroup tabGroup;   // Reference to the TabGroup
    public Button button;       // Reference to the Button component
    private TextMeshProUGUI buttonText;    // Reference to the TextMeshProUGUI component inside the button
    public GameObject glow; // assign in Inspector (child Image)
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

        // Auto-find Glow child by name
        glow = transform.Find("Glow")?.gameObject;
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
        if (glow != null)
            glow.SetActive(true);

        if (buttonText != null)
        {
            // optional styling
            // buttonText.color = Color.black;
        }
    }

    public void Deselect()
    {
        if (glow != null)
            glow.SetActive(false);

        if (buttonText != null)
        {
            // optional styling
            // buttonText.color = new Color32(238, 225, 211, 255);
        }
    }

    private void OnClick()
    {
        // Notify the TabGroup that this tab was selected
        tabGroup.OnTabSelected(this);
    }
}
