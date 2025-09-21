using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Crafting Related")]
    public int gold = 0;
    public List<InventorySlot> ownedItems = new List<InventorySlot>(); // now holds slots
    [SerializeField] private CharacterStats characterStats;
    public event System.Action OnInventoryChanged;

    // Delegate + event for player gold changes
    public delegate void GoldChanged(int gold);
    public event GoldChanged OnGoldChanged;

    // === EQUIP LOGIC (unchanged) ===
    public void Equip(GameItem item)
    {
        if (item.itemType == ItemType.MeleeWeapon)
        {
            if (characterStats.equippedMeleeWeapon == item)
                characterStats.UnequipMeleeWeapon();
            else
                characterStats.EquipMeleeWeapon(item);
        }
        else if (item.itemType == ItemType.RangedWeapon)
        {
            if (characterStats.equippedRangedWeapon == item)
                characterStats.UnequipRangedWeapon();
            else
                characterStats.EquipRangedWeapon(item);
        }
    }

    // === USE ITEMS ===
    public void UseItem(GameItem item)
    {
        InventorySlot slot = ownedItems.Find(s => s.item == item);
        if (slot != null && slot.quantity > 0)
        {
            if (item.itemType == ItemType.Consumable)
            {
                characterStats.Heal(item.healAmount);

                slot.quantity--;

                if (slot.quantity <= 0)
                    ownedItems.Remove(slot);

                OnInventoryChanged?.Invoke();
            }
        }
    }

    // === ADD ITEMS ===
    public void AddItem(GameItem item, int amount = 1)
    {
        // If stackable, add to existing slot
        if (item.isStackable)
        {
            InventorySlot slot = ownedItems.Find(s => s.item == item);
            if (slot != null)
            {
                slot.quantity += amount;
            }
            else
            {
                ownedItems.Add(new InventorySlot(item, amount));
            }
        }
        else
        {
            // Non-stackable (like weapons), always add new entry
            ownedItems.Add(new InventorySlot(item, 1));
        }

        OnInventoryChanged?.Invoke();
    }

    // === GOLD ===
    public void DeductGold(int amount)
    {
        gold -= amount;
        if (gold < 0) gold = 0;
        OnGoldChanged?.Invoke(gold);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }
}
