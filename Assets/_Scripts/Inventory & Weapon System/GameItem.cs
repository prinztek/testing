using UnityEngine;
public enum ItemType
{
    MeleeWeapon,
    RangedWeapon,
    Consumable,
    KeyItem
}


[CreateAssetMenu(fileName = "GameItem", menuName = "Inventory/Game Item")]
public class GameItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;

    // Weapon Stats (only used if itemType is Melee/Ranged)
    public int baseDamage;
    public float attackSpeed;
    public int healAmount; // for potions
    public string description;
}


