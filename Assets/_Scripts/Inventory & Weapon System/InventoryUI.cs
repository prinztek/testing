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
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= RefreshUI;
            playerInventory.OnGoldChanged -= UpdateGoldUI;
        }
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        // Assign inventory and character stats
        playerInventory = playerObj.GetComponent<PlayerInventory>();
        characterStats = playerObj.GetComponent<CharacterStats>();

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += RefreshUI;
            playerInventory.OnGoldChanged += UpdateGoldUI;
        }

        ClearDetails();
        RefreshUI();
        UpdateGoldUI(playerInventory?.Gold ?? 0);
    }

    /// <summary>
    /// Update the entire inventory UI
    /// </summary>
    public void RefreshUI()
    {
        if (playerInventory == null) return;

        // Clear previous buttons
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        // Create a button for each owned item
        foreach (InventorySlot slot in playerInventory.OwnedItems)
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
                    quantityText.text = slot.item.isStackable ? slot.quantity.ToString() : "";
            }

            // --- Button Click (show details) ---
            Button btn = btnGO.GetComponent<Button>();
            InventorySlot capturedSlot = slot; // prevent closure issues
            btn.onClick.AddListener(() => ShowInventoryItemDetails(capturedSlot.item));
        }

        // Update equipped weapon display
        equippedMeleeText.text = "Melee: " +
            (characterStats.equippedMeleeWeapon ? characterStats.equippedMeleeWeapon.itemName : "Fist");

        equippedRangedText.text = "Ranged: " +
            (characterStats.equippedRangedWeapon ? characterStats.equippedRangedWeapon.itemName : "None");
    }

    /// <summary>
    /// Show details of the selected inventory item
    /// </summary>
    private void ShowInventoryItemDetails(GameItem item)
    {
        selectedItem = item;

        if (item == null) return;

        string details = $"{item.itemName}\n{item.description}";
        string buttonLabel = "Use";

        if (item.itemType == ItemType.MeleeWeapon)
        {
            if (characterStats.equippedMeleeWeapon == item)
            {
                details += "\n(Equipped)";
                buttonLabel = "Unequip";
            }
            else
            {
                details += $"\nDamage: {item.baseDamage}";
                buttonLabel = "Equip";
            }
        }
        else if (item.itemType == ItemType.RangedWeapon)
        {
            if (characterStats.equippedRangedWeapon == item)
            {
                details += "\n(Equipped)";
                buttonLabel = "Unequip";
            }
            else
            {
                details += $"\nDamage: {item.baseDamage}";
                buttonLabel = "Equip";
            }
        }
        else if (item.itemType == ItemType.Consumable)
        {
            details += $"\nHeal Amount: {item.healAmount}";
            buttonLabel = "Use";
        }

        descriptionText.text = details;

        useInventoryItemButton.interactable = true;
        useInventoryItemButton.onClick.RemoveAllListeners();
        useInventoryItemButton.onClick.AddListener(() => UseSelectedItem(buttonLabel));
        SetButtonLabel(buttonLabel);
    }

    private void SetButtonLabel(string text)
    {
        TextMeshProUGUI buttonLabel = useInventoryItemButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonLabel != null) buttonLabel.text = text;
    }

    private void UseSelectedItem(string action)
    {
        if (selectedItem == null || playerInventory == null) return;

        GameItem previouslySelected = selectedItem;
        bool wasStackable = selectedItem.isStackable;

        if (selectedItem.itemType == ItemType.Consumable)
        {
            playerInventory.UseItem(selectedItem);
        }
        else
        {
            playerInventory.Equip(selectedItem);
        }

        RefreshUI();

        // Re-select the item if it still exists
        if (wasStackable && playerInventory.HasItem(previouslySelected))
            ShowInventoryItemDetails(previouslySelected);
        else
            ClearDetails();
    }

    private void ClearDetails()
    {
        descriptionText.text = "Select an item to see details";
        useInventoryItemButton.interactable = false;
        useInventoryItemButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Updates gold UI. You can expand this if you have a gold text field.
    /// </summary>
    private void UpdateGoldUI(int gold)
    {
        // Example: update a gold text UI if you have one
        // goldText.text = gold.ToString();
    }
}
