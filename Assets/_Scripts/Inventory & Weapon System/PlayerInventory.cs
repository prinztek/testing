using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<GameItem> ownedItems = new List<GameItem>();
    [SerializeField] private CharacterStats characterStats;

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

    public void UseItem(GameItem item)
    {
        if (item.itemType == ItemType.Consumable)
        {
            characterStats.Heal(item.healAmount);
            ownedItems.Remove(item);
        }
    }
}
