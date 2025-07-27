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

    void OnEnable()
    {
        playerInventory.OnInventoryChanged += RefreshUI;
        RefreshUI();
    }

    public void RefreshUI()
    {
        // Clear previous buttons
        foreach (Transform child in itemListParent)
        {
            Destroy(child.gameObject);
        }

        // Generate a button for each item
        foreach (GameItem item in playerInventory.ownedItems)
        {
            GameObject btnGO = Instantiate(itemButtonPrefab, itemListParent);

            // Explicitly find the icon and label in the prefab
            Transform iconTransform = btnGO.transform.Find("Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                    iconImage.sprite = item.icon;
            }

            Transform labelTransform = btnGO.transform.Find("Label");
            if (labelTransform != null)
            {
                TextMeshProUGUI labelText = labelTransform.GetComponent<TextMeshProUGUI>();
                if (labelText != null)
                    labelText.text = item.itemName;
            }

            // Add button click listener
            Button btn = btnGO.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                if (item.itemType == ItemType.Consumable)
                    playerInventory.UseItem(item);
                else
                    playerInventory.Equip(item);

                RefreshUI();
            });
        }

        // Show equipped weapons
        equippedMeleeText.text = "Melee: " +
            (characterStats.equippedMeleeWeapon ? characterStats.equippedMeleeWeapon.itemName : "Fist");

        equippedRangedText.text = "Ranged: " +
            (characterStats.equippedRangedWeapon ? characterStats.equippedRangedWeapon.itemName : "None");
    }


    void OnDisable()
    {
        playerInventory.OnInventoryChanged -= RefreshUI;
    }
}
