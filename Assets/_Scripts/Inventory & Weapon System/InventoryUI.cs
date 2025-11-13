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

        // 🔹 If player already exists when UI enables, connect immediately
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            HandlePlayerSpawned(GameManager.Instance.CurrentPlayer);
        }
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
        if (playerObj == null)
        {
            Debug.LogWarning("[InventoryUI] Player spawn event received, but player is null!");
            return;
        }

        playerInventory = playerObj.GetComponent<PlayerInventory>();
        characterStats = playerObj.GetComponent<CharacterStats>();

        if (playerInventory == null || characterStats == null)
        {
            Debug.LogWarning("[InventoryUI] Player components missing.");
            return;
        }

        playerInventory.OnInventoryChanged += RefreshUI;
        playerInventory.OnGoldChanged += UpdateGoldUI;

        ClearDetails();
        RefreshUI();
        UpdateGoldUI(playerInventory.Gold);

        Debug.Log("✅ InventoryUI connected to player inventory.");
    }

    public void RefreshUI()
    {
        if (playerInventory == null || characterStats == null) return;

        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        foreach (InventorySlot slot in playerInventory.OwnedItems)
        {
            GameObject btnGO = Instantiate(itemButtonPrefab, itemListParent);

            Transform iconTransform = btnGO.transform.Find("Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null) iconImage.sprite = slot.item.icon;
            }

            Transform quantityTransform = btnGO.transform.Find("Quantity");
            if (quantityTransform != null)
            {
                TextMeshProUGUI quantityText = quantityTransform.GetComponent<TextMeshProUGUI>();
                if (quantityText != null)
                    quantityText.text = slot.item.isStackable ? slot.quantity.ToString() : "";
            }

            Button btn = btnGO.GetComponent<Button>();
            InventorySlot capturedSlot = slot;
            btn.onClick.AddListener(() => ShowInventoryItemDetails(capturedSlot.item));
        }

        equippedMeleeText.text = "Melee: " +
            (characterStats.equippedMeleeWeapon ? characterStats.equippedMeleeWeapon.itemName : "Fist");

        equippedRangedText.text = "Ranged: " +
            (characterStats.equippedRangedWeapon ? characterStats.equippedRangedWeapon.itemName : "None");
    }

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
        TextMeshProUGUI label = useInventoryItemButton.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = text;
    }

    private void UseSelectedItem(string action)
    {
        if (selectedItem == null || playerInventory == null) return;

        GameItem previouslySelected = selectedItem;
        bool wasStackable = selectedItem.isStackable;

        if (selectedItem.itemType == ItemType.Consumable)
            playerInventory.UseItem(selectedItem);
        else
            playerInventory.Equip(selectedItem);

        RefreshUI();

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

    private void UpdateGoldUI(int gold)
    {
        // Optional: add gold text update here if needed
        Debug.Log($"💰 Gold updated: {gold}");
    }
}
