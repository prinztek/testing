using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingItemButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;

    private CraftableItem item;

    public void Setup(CraftableItem craftable)
    {
        item = craftable;

        if (item != null && item.itemData != null)
        {
            iconImage.sprite = item.itemData.icon;
            nameText.text = item.itemData.itemName;
            costText.text = item.costInGold + " Gold";
        }
    }

    public void SetOnClick(System.Action callback)
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => callback?.Invoke());
    }

}
