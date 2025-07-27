using UnityEngine;

[CreateAssetMenu(fileName = "DropItem", menuName = "Scriptable Objects/DropItem")]
public class DropItem : ScriptableObject
{
    public GameItem itemData;  // 🔁 Reference to existing GameItem
    public float dropChance;
}
