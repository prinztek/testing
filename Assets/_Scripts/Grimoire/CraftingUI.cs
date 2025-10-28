using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    public CharacterStats characterStats;

    [Header("Crafting Items")]
    public Transform itemListParent;
    public GameObject craftingItemButtonPrefab;
    public CraftableItem[] craftableItems;

    [Header("Right Panel UI")]
    public TextMeshProUGUI descriptionText;
    public Button craftButton;
    public TextMeshProUGUI craftButtonPriceText;

    private CraftableItem selectedItem;

    private void OnEnable()
    {
        RefreshCraftingList();
        ClearDetails();
    }

    private void RefreshCraftingList()
    {
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        foreach (CraftableItem craftable in craftableItems)
        {
            GameObject btnGO = Instantiate(craftingItemButtonPrefab, itemListParent);
            CraftingItemButton btn = btnGO.GetComponent<CraftingItemButton>();
            btn.Setup(craftable);
            btn.SetOnClick(() => ShowCraftableDetails(craftable));
        }
    }

    private void ShowCraftableDetails(CraftableItem item)
    {
        selectedItem = item;

        if (item != null && item.itemData != null)
        {
            descriptionText.text =
                $"{item.itemData.itemName}\n" +
                $"{item.itemData.description}\n" +
                $"Damage: {item.itemData.baseDamage}\n" +
                $"Cost: {item.costInGold}";
        }

        craftButtonPriceText.text = $"{item.costInGold} Gold";
        craftButton.interactable = true;
        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(() => CraftSelectedItem());
    }

    private void CraftSelectedItem()
    {
        if (selectedItem == null) return;
        Debug.Log($"✅ Crafted: {selectedItem.itemData.itemName} for {selectedItem.costInGold} gold.");
        // Add logic here to deduct gold and give the item
        characterStats.DeductGold(selectedItem.costInGold);
    }

    private void ClearDetails()
    {
        descriptionText.text = "Select an item to see details";
        craftButtonPriceText.text = "Craft";
        craftButton.interactable = false;
        craftButton.onClick.RemoveAllListeners();
    }
}
