using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<GameItem> ownedItems = new List<GameItem>();

    public GameItem equippedMelee;
    public GameItem equippedRanged;
    public GameItem defaultFist; // Assign in inspector

    void Start()
    {
        EquipMelee(defaultFist); // Start with fist
    }

    public void AddItem(GameItem newItem)
    {
        if (!ownedItems.Contains(newItem))
            ownedItems.Add(newItem);
    }

    public void EquipMelee(GameItem weapon)
    {
        if (weapon.itemType == ItemType.MeleeWeapon)
            equippedMelee = weapon;
    }

    public void EquipRanged(GameItem weapon)
    {
        if (weapon.itemType == ItemType.RangedWeapon)
            equippedRanged = weapon;
    }
}
