using UnityEngine;
public enum ItemType
{
    MeleeWeapon,
    RangedWeapon,
    Consumable,
    KeyItem,
    Gold
}


[CreateAssetMenu(fileName = "GameItem", menuName = "Inventory/Game Item")]
public class GameItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public string description;

    public bool isStackable; // true for consumables like potions || false for weapons

    // Weapon Stats (only used if itemType is Melee/Ranged)
    public int baseDamage;
    public float attackSpeed;

    // Consumable
    public int healAmount; // for potions

    // Gold
    public int goldAmount;
}

// item drop would use this scriptable object of GameItem
