// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerInventory : MonoBehaviour
// {
//     [Header("Crafting Related")]
//     public int gold = 0;
//     public List<InventorySlot> ownedItems = new List<InventorySlot>(); // now holds slots
//     [SerializeField] private CharacterStats characterStats;
//     public event System.Action OnInventoryChanged;

//     // Delegate + event for player gold changes
//     public delegate void GoldChanged(int gold);
//     public event GoldChanged OnGoldChanged;

//     // === EQUIP LOGIC (unchanged) ===
//     public void Equip(GameItem item)
//     {
//         if (item.itemType == ItemType.MeleeWeapon)
//         {
//             if (characterStats.equippedMeleeWeapon == item)
//                 characterStats.UnequipMeleeWeapon();
//             else
//                 characterStats.EquipMeleeWeapon(item);
//         }
//         else if (item.itemType == ItemType.RangedWeapon)
//         {
//             if (characterStats.equippedRangedWeapon == item)
//                 characterStats.UnequipRangedWeapon();
//             else
//                 characterStats.EquipRangedWeapon(item);
//         }
//     }

//     // === USE ITEMS ===
//     public void UseItem(GameItem item)
//     {
//         InventorySlot slot = ownedItems.Find(s => s.item == item);
//         if (slot != null && slot.quantity > 0)
//         {
//             if (item.itemType == ItemType.Consumable)
//             {
//                 characterStats.Heal(item.healAmount);

//                 slot.quantity--;

//                 if (slot.quantity <= 0)
//                     ownedItems.Remove(slot);

//                 OnInventoryChanged?.Invoke();
//             }
//         }
//     }

//     // === ADD ITEMS ===
//     public void AddItem(GameItem item, int amount = 1)
//     {
//         // If stackable, add to existing slot
//         if (item.isStackable)
//         {
//             InventorySlot slot = ownedItems.Find(s => s.item == item);
//             if (slot != null)
//             {
//                 slot.quantity += amount;
//             }
//             else
//             {
//                 ownedItems.Add(new InventorySlot(item, amount));
//             }
//         }
//         else
//         {
//             // Non-stackable (like weapons), always add new entry
//             ownedItems.Add(new InventorySlot(item, 1));
//         }

//         OnInventoryChanged?.Invoke();
//     }

//     public bool HasItem(GameItem item)
//     {
//         foreach (var slot in ownedItems)
//         {
//             if (slot.item == item && slot.quantity > 0)
//                 return true;
//         }
//         return false;
//     }

//     // === GOLD ===
//     public void DeductGold(int amount)
//     {
//         gold -= amount;
//         if (gold < 0) gold = 0;
//         OnGoldChanged?.Invoke(gold);
//     }

//     public void AddGold(int amount)
//     {
//         gold += amount;
//         OnGoldChanged?.Invoke(gold);
//     }
// }


using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Crafting Related")]
    [SerializeField] private CharacterStats characterStats;

    // Backing fields
    private int _gold;
    private List<InventorySlot> _ownedItems = new List<InventorySlot>();

    // Events
    public event Action OnInventoryChanged;
    public event Action<int> OnGoldChanged;

    // Properties that automatically fire events
    public int Gold
    {
        get => _gold;
        set
        {
            _gold = value;
            OnGoldChanged?.Invoke(_gold);
        }
    }

    public List<InventorySlot> OwnedItems
    {
        get => _ownedItems;
        set
        {
            _ownedItems = value;
            OnInventoryChanged?.Invoke();
        }
    }

    // === ADD ITEMS ===
    public void AddItem(GameItem item, int amount = 1)
    {
        if (item.isStackable)
        {
            var slot = _ownedItems.Find(s => s.item == item);
            if (slot != null) slot.quantity += amount;
            else _ownedItems.Add(new InventorySlot(item, amount));
        }
        else
        {
            _ownedItems.Add(new InventorySlot(item, 1));
        }

        OnInventoryChanged?.Invoke();
    }

    // === USE ITEMS ===
    public void UseItem(GameItem item)
    {
        var slot = _ownedItems.Find(s => s.item == item);
        if (slot != null && slot.quantity > 0)
        {
            if (item.itemType == ItemType.Consumable)
            {
                characterStats.Heal(item.healAmount);

                slot.quantity--;
                if (slot.quantity <= 0) _ownedItems.Remove(slot);

                OnInventoryChanged?.Invoke();
            }
        }
    }

    public bool HasItem(GameItem item)
    {
        foreach (InventorySlot slot in OwnedItems)
        {
            if (slot.item == item && slot.quantity > 0)
                return true;
        }
        return false;
    }


    // === GOLD ===
    public void AddGold(int amount) => Gold += amount;
    public void DeductGold(int amount) => Gold = Mathf.Max(0, Gold - amount);

    // === EQUIP LOGIC ===
    public void Equip(GameItem item)
    {
        if (item.itemType == ItemType.MeleeWeapon)
        {
            if (characterStats.equippedMeleeWeapon == item) characterStats.UnequipMeleeWeapon();
            else characterStats.EquipMeleeWeapon(item);
        }
        else if (item.itemType == ItemType.RangedWeapon)
        {
            if (characterStats.equippedRangedWeapon == item) characterStats.UnequipRangedWeapon();
            else characterStats.EquipRangedWeapon(item);
        }
    }
}
