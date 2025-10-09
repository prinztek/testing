using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory playerInventory;
    public CharacterStats characterStats;

    [Header("UI Prefabs")]
    public GameObject itemButtonPrefab;
    public Transform itemListParent;

    [Header("Equipped Weapon Texts")]
    public TextMeshProUGUI equippedMeleeText;
    public TextMeshProUGUI equippedRangedText;

    [Header("Right Panel UI")]
    public TextMeshProUGUI descriptionText;
    public Button useInventoryItemButton;

    private GameItem selectedItem;

    private void OnEnable()
    {
        playerInventory.OnInventoryChanged += RefreshUI;
        RefreshUI();
        ClearDetails();
    }

    private void OnDisable()
    {
        playerInventory.OnInventoryChanged -= RefreshUI;
    }

    public void RefreshUI()
    {
        // Clear previous buttons
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        // Create a button for each owned slot
        foreach (InventorySlot slot in playerInventory.ownedItems)
        {
            GameObject btnGO = Instantiate(itemButtonPrefab, itemListParent);

            // --- Icon ---
            Transform iconTransform = btnGO.transform.Find("Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null) iconImage.sprite = slot.item.icon;
            }

            // --- Quantity Text ---
            Transform quantityTransform = btnGO.transform.Find("Quantity");
            if (quantityTransform != null)
            {
                TextMeshProUGUI quantityText = quantityTransform.GetComponent<TextMeshProUGUI>();
                if (quantityText != null)
                {
                    quantityText.text = slot.item.isStackable ? slot.quantity.ToString() : "";
                }
            }

            // --- Button Click (show details) ---
            Button btn = btnGO.GetComponent<Button>();
            btn.onClick.AddListener(() => ShowInventoryItemDetails(slot.item));
        }

        // Update equipped weapon display
        equippedMeleeText.text = "Melee: " +
            (characterStats.equippedMeleeWeapon ? characterStats.equippedMeleeWeapon.itemName : "Fist");

        equippedRangedText.text = "Ranged: " +
            (characterStats.equippedRangedWeapon ? characterStats.equippedRangedWeapon.itemName : "None");
    }

    private void ShowInventoryItemDetails(GameItem item)
    {
        selectedItem = item;

        if (item != null && (item.itemType == ItemType.MeleeWeapon || item.itemType == ItemType.RangedWeapon))
        {
            if (item.itemType == ItemType.MeleeWeapon && characterStats.equippedMeleeWeapon == item)
            {
                descriptionText.text = $"{item.itemName}\n{item.description}\n(Equipped)";
                SetButtonLabel("Unequip");
            }
            else if (item.itemType == ItemType.RangedWeapon && characterStats.equippedRangedWeapon == item)
            {
                descriptionText.text = $"{item.itemName}\n{item.description}\n(Equipped)";
                SetButtonLabel("Unequip");
            }
            else
            {
                descriptionText.text =
                    $"{item.itemName}\n" +
                    $"{item.description}\n" +
                    $"Damage: {item.baseDamage}";
                SetButtonLabel("Equip");
            }
        }
        else if (item.itemType == ItemType.Consumable)
        {
            descriptionText.text =
                $"{item.itemName}\n" +
                $"{item.description}\n" +
                $"Heal Amount: {item.healAmount}";
            SetButtonLabel("Use");
        }
        else
        {
            descriptionText.text = $"{item.itemName}\n{item.description}";
            SetButtonLabel("Use");
        }

        useInventoryItemButton.interactable = true;
        useInventoryItemButton.onClick.RemoveAllListeners();
        useInventoryItemButton.onClick.AddListener(() => UseSelectedItem());
    }

    private void SetButtonLabel(string text)
    {
        TextMeshProUGUI buttonLabel = useInventoryItemButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonLabel != null) buttonLabel.text = text;
    }

    private void UseSelectedItem()
    {
        if (selectedItem == null) return;

        bool wasStackable = selectedItem.isStackable;
        GameItem previouslySelected = selectedItem;

        if (selectedItem.itemType == ItemType.Consumable)
        {
            playerInventory.UseItem(selectedItem);
        }
        else
        {
            playerInventory.Equip(selectedItem);
        }

        RefreshUI();

        // Re-select the item if it's stackable and still exists
        if (wasStackable && playerInventory.HasItem(previouslySelected))
        {
            ShowInventoryItemDetails(previouslySelected);
        }
        else
        {
            ClearDetails();
        }
    }

    private void ClearDetails()
    {
        descriptionText.text = "Select an item to see details";
        useInventoryItemButton.interactable = false;
        useInventoryItemButton.onClick.RemoveAllListeners();
    }
}
