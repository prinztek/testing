using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftingUI : MonoBehaviour
{
    public PlayerInventory playerInventory;

    [Header("Confirmation Dialog")]
    [SerializeField] private ConfirmationUI confirmationUI;


    [Header("Crafting Items")]
    public Transform itemListParent;
    public GameObject craftingItemButtonPrefab;
    public CraftableItem[] craftableItems;

    [Header("Right Panel UI")]
    public TextMeshProUGUI descriptionText;
    public Button craftButton;
    public TextMeshProUGUI craftButtonPriceText;

    [Header("Selection Visual")]
    public string selectorObjectName = "Selector"; // Name of the child object in prefab
    // Alternative: public Color normalColor = Color.white;
    // Alternative: public Color selectedColor = new Color(1f, 0.9f, 0.5f, 1f);

    private CraftableItem selectedItem;
    private CraftingItemButton selectedButton;
    private Dictionary<CraftableItem, CraftingItemButton> itemButtonMap = new Dictionary<CraftableItem, CraftingItemButton>();

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
        // If player already exists when UI enables, connect immediately
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
            playerInventory.OnInventoryChanged -= RefreshCraftingList;
        }
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        playerInventory = playerObj.GetComponent<PlayerInventory>();
        playerInventory.OnInventoryChanged += RefreshCraftingList;
        RefreshCraftingList();
    }

    private void RefreshCraftingList()
    {
        // Store currently selected item to restore selection after refresh
        CraftableItem previouslySelected = selectedItem;

        // Clear the item-button mapping
        itemButtonMap.Clear();

        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        CraftableItem firstItem = null;

        foreach (CraftableItem craftable in craftableItems)
        {
            if (firstItem == null) firstItem = craftable;

            // Determine if the item is already owned (only for weapons)
            ItemType itemType = craftable.itemData.itemType;
            bool isWeapon = itemType == ItemType.MeleeWeapon || itemType == ItemType.RangedWeapon;
            bool alreadyOwned = isWeapon && playerInventory != null && playerInventory.HasItem(craftable.itemData);

            GameObject btnGO = Instantiate(craftingItemButtonPrefab, itemListParent);
            CraftingItemButton btn = btnGO.GetComponent<CraftingItemButton>();
            // pass alreadyOwned to button to show "Owned" text if needed
            btn.Setup(craftable, alreadyOwned);
            btn.SetOnClick(() => SelectItem(craftable, btn));

            // Store the button reference
            itemButtonMap[craftable] = btn;
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
            // No items available
            ClearDetails();
        }
    }

    private void SelectItem(CraftableItem item, CraftingItemButton button)
    {
        // Clear previous selection visual
        if (selectedButton != null)
        {
            SetSelectorActive(selectedButton, false);
        }

        // Set new selection
        selectedItem = item;
        selectedButton = button;

        // Apply selection visual
        SetSelectorActive(selectedButton, true);

        // Show details
        ShowCraftableItemDetails(item);
    }

    private void SetSelectorActive(CraftingItemButton button, bool active)
    {
        if (button == null) return;

        // Find the selector child object in the button
        Transform selector = button.transform.Find(selectorObjectName);
        if (selector != null)
        {
            selector.gameObject.SetActive(active);
        }
        else if (active)
        {
            Debug.LogWarning($"[CraftingUI] Selector object '{selectorObjectName}' not found in button prefab.");
        }
    }

    // Alternative method if you want to use color instead of a selector object:
    /*
    private void SetSelectorActive(CraftingItemButton button, bool active)
    {
        if (button == null) return;

        Button unityButton = button.GetComponent<Button>();
        if (unityButton != null)
        {
            Image buttonImage = unityButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = active ? selectedColor : normalColor;
            }
        }
    }
    */

    private void ShowCraftableItemDetails(CraftableItem item)
    {
        if (item == null || item.itemData == null) return;

        ItemType itemType = item.itemData.itemType;
        bool isWeapon = itemType == ItemType.MeleeWeapon || itemType == ItemType.RangedWeapon;
        bool alreadyOwned = isWeapon && playerInventory != null && playerInventory.HasItem(item.itemData);

        descriptionText.text =
            $"{item.itemData.itemName}\n" +
            $"{item.itemData.description}\n" +
            $"Damage: {item.itemData.baseDamage}\n" +
            $"Cost: {item.costInGold}";

        craftButtonPriceText.text = $"{item.costInGold} Gold";

        // Update craft button
        if (alreadyOwned)
        {
            craftButton.interactable = false;
        }
        else
        {
            // Check if player can afford it
            bool canAfford = playerInventory != null && playerInventory.Gold >= item.costInGold;
            craftButton.interactable = canAfford;
        }

        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(() => CraftSelectedItem());
    }

    private void CraftSelectedItem()
    {
        if (selectedItem == null || playerInventory == null) return;

        if (playerInventory.Gold < selectedItem.costInGold)
        {
            Debug.LogWarning("Not enough gold to craft this item.");
            return;
        }

        Debug.Log($"Crafted: {selectedItem.itemData.itemName} for {selectedItem.costInGold} gold.");

        // before every transaction, show a confirmation dialog
        // if yes - proceed with crafting
        // if no - return to crafting menu without doing anything
        confirmationUI.Show(
            title: "Confirm Crafting",
            description: $"Are you sure you want to craft {selectedItem.itemData.itemName} for {selectedItem.costInGold} gold?",
            cost: $"{selectedItem.costInGold} Gold",
            currentGold: $"You have: {playerInventory.Gold} Gold",
            confirmAction: () =>
            {
                // Deduct gold and add item to inventory
                playerInventory.DeductGold(selectedItem.costInGold);
                playerInventory.AddItem(selectedItem.itemData);

                // Refresh to update button states (affordability) while keeping selection
                RefreshCraftingList();
            }
        );

        // playerInventory.DeductGold(selectedItem.costInGold);
        // playerInventory.AddItem(selectedItem.itemData);

        // // Refresh to update button states (affordability) while keeping selection
        // RefreshCraftingList();
    }

    private void ClearDetails()
    {
        selectedItem = null;
        selectedButton = null;
        descriptionText.text = "Select an item to see details";
        craftButtonPriceText.text = "Craft";
        craftButton.interactable = false;
        craftButton.onClick.RemoveAllListeners();
    }

    private void SetOwnedTextActive(CraftingItemButton button, bool active)
    {
        if (button == null) return;

        // Find the selector child object in the button
        Transform ownedTextIndicator = button.transform.Find("Owned Text");
        if (ownedTextIndicator != null)
        {
            ownedTextIndicator.gameObject.SetActive(active);
        }
        else if (active)
        {
            Debug.LogWarning($"[CraftingUI] Owned Text object not found in button prefab.");
        }
    }
}