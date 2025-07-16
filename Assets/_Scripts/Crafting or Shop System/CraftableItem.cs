using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/CraftableItem")]
public class CraftableItem : ScriptableObject
{
    public GameItem itemData;  // 🔁 Reference to existing GameItem
    public int costInGold;     // 💰 Crafting cost
}
