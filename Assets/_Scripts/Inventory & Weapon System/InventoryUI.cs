using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory playerInventory;
    public CharacterStats characterStats;

    [Header("UI Prefabs")]
    public GameObject itemButtonPrefab;
    public Transform itemListParent;

    [Header("Equipped Weapon Texts")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI equippedMeleeText;
    public TextMeshProUGUI equippedRangedText;
    public TextMeshProUGUI activeBuffText;
    public TextMeshProUGUI goldText;

    [Header("Right Panel UI")]
    public TextMeshProUGUI descriptionText;
    public Button useInventoryItemButton;

    [Header("Selection Visual")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(1f, 0.9f, 0.5f, 1f); // Light yellow/gold

    private GameItem selectedItem;
    private Button selectedButton;
    private Dictionary<GameItem, Button> itemButtonMap = new Dictionary<GameItem, Button>();

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
    }

    public void RefreshUI()
    {
        if (playerInventory == null || characterStats == null) return;

        // Store currently selected item to restore selection after refresh
        GameItem previouslySelected = selectedItem;

        // Clear the item-button mapping
        itemButtonMap.Clear();

        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        GameItem firstItem = null;

        foreach (InventorySlot slot in playerInventory.OwnedItems)
        {
            if (firstItem == null) firstItem = slot.item;

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
            btn.onClick.AddListener(() => SelectItem(capturedSlot.item, btn));

            // Store the button reference
            itemButtonMap[slot.item] = btn;
        }

        // Restore selection or select first item
        if (previouslySelected != null && itemButtonMap.ContainsKey(previouslySelected))
        {
            // Re-select the previously selected item
            SelectItem(previouslySelected, itemButtonMap[previouslySelected]);
        }
        else if (firstItem != null && itemButtonMap.ContainsKey(firstItem))
        {
            // Auto-select first item if no previous selection
            SelectItem(firstItem, itemButtonMap[firstItem]);
        }
        else
        {
            // No items in inventory
            ClearDetails();
        }

        // Update status displays
        healthText.text = $"Health: {characterStats.CurrentHealth}";
        activeBuffText.text = "Buffs: " + (characterStats.activeBuff != null ? characterStats.activeBuff.buffName : "None");
        equippedMeleeText.text = "Melee: " +
            (characterStats.equippedMeleeWeapon ? characterStats.equippedMeleeWeapon.itemName : "Fist");

        equippedRangedText.text = "Ranged: " +
            (characterStats.equippedRangedWeapon ? characterStats.equippedRangedWeapon.itemName : "None");
        goldText.text = $"Gold: " + playerInventory.Gold;
    }

    private void SelectItem(GameItem item, Button button)
    {
        // // Clear previous selection visual
        // if (selectedButton != null)
        // {
        //     SetButtonColor(selectedButton, normalColor);
        // }

        // Clear previous selection visual
        if (selectedButton != null)
        {
            SetSelectorActive(selectedButton, false);
        }

        // Set new selection
        selectedItem = item;
        selectedButton = button;

        // // Apply selection visual
        // SetButtonColor(selectedButton, selectedColor);

        // Apply selection visual
        SetSelectorActive(selectedButton, true);

        // Show details
        ShowInventoryItemDetails(item);
    }

    private void SetSelectorActive(Button button, bool active)
    {
        if (button == null) return;

        Transform selector = button.transform.Find("Selector");
        if (selector != null)
        {
            selector.gameObject.SetActive(active);
        }
    }

    private void SetButtonColor(Button button, Color color)
    {
        if (button == null) return;

        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.selectedColor = color;
        button.colors = colors;
    }

    private void ShowInventoryItemDetails(GameItem item)
    {
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

        GameItem itemToKeepSelected = selectedItem;

        if (selectedItem.itemType == ItemType.Consumable)
        {
            //  If this is a heath potion, return if the player is already at full health to prevent waste
            if (selectedItem.healAmount > 0 && characterStats.CurrentHealth >= characterStats.maxHealth)
            {
                Debug.Log("Health is already full. Cannot use health potion.");
                return;
            }

            playerInventory.UseItem(selectedItem);
            // If item was consumed and no longer exists, selection will be handled in RefreshUI
        }
        else
        {
            // Equipping/Unequipping - keep the item selected
            playerInventory.Equip(selectedItem);
        }

        // RefreshUI will maintain selection on the same item (toggle behavior)
        RefreshUI();
    }

    private void ClearDetails()
    {
        selectedItem = null;
        selectedButton = null;
        descriptionText.text = "Select an item to see details";
        useInventoryItemButton.interactable = false;
        useInventoryItemButton.onClick.RemoveAllListeners();
    }

    private void UpdateGoldUI(int gold)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {gold}";
        }
    }
}