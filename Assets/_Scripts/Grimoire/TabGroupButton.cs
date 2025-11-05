using UnityEngine;
using UnityEngine.UI;

public class TabGroupButton : MonoBehaviour
{
    public TabGroup tabGroup;
    public Button button;
    private Image buttonImage;

    public Color selectedColor = new Color32(238, 225, 211, 255);
    public Color deselectedColor = new Color32(231, 139, 80, 255);

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        selectedColor = new Color32(238, 225, 211, 255);
        deselectedColor = new Color32(231, 139, 80, 255);
    }

    void Start()
    {
        tabGroup = GetComponentInParent<TabGroup>();
        button = GetComponent<Button>();

        if (tabGroup == null)
        {
            Debug.LogError("TabGroupButton must be a child of a TabGroup.");
            return;
        }

        tabGroup.Subscribe(this);

        if (button != null)
            button.onClick.AddListener(OnClick);

        if (tabGroup.selectedTab == this)
            Select();
        else
            Deselect();
    }

    public void Select()
    {
        if (buttonImage != null)
            buttonImage.color = selectedColor;
    }

    public void Deselect()
    {
        if (buttonImage != null)
            buttonImage.color = deselectedColor;
    }

    private void OnClick()
    {
        tabGroup.OnTabSelected(this);
    }
}
