using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<GameItem> ownedItems = new List<GameItem>(); // will take a list of GameItem Objects
    [SerializeField] private CharacterStats characterStats;
    public event System.Action OnInventoryChanged; // use to refresh inventory ui

    // Equip item based on its item type melee or range weapon
    public void Equip(GameItem item)
    {
        if (item.itemType == ItemType.MeleeWeapon)
        {
            if (characterStats.equippedMeleeWeapon == item)
            {
                characterStats.UnequipMeleeWeapon();
            }
            else
            {
                characterStats.EquipMeleeWeapon(item);
            }
        }
        else if (item.itemType == ItemType.RangedWeapon)
        {
            if (characterStats.equippedRangedWeapon == item)
            {
                characterStats.UnequipRangedWeapon();
            }
            else
            {
                characterStats.EquipRangedWeapon(item);
            }
        }
    }

    // use for consumable items (health potions)
    public void UseItem(GameItem item)
    {
        if (item.itemType == ItemType.Consumable)
        {
            characterStats.Heal(item.healAmount);
            ownedItems.Remove(item);
            OnInventoryChanged?.Invoke(); // Notify listeners
        }
    }

    public void AddItem(GameItem item)
    {
        ownedItems.Add(item);
        OnInventoryChanged?.Invoke(); // Notify listeners
    }
}
