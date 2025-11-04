using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a list of all GameItems in the game.
/// This is used to map item names in saves back to actual GameItem objects.
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<GameItem> allItems = new List<GameItem>();
    private static ItemDatabase instance;

    public static GameItem GetItemById(string id)
    {
        if (instance == null) instance = Resources.Load<ItemDatabase>("GameItemDatabase");
        return instance.allItems.Find(i => i.itemName == id);
    }
}
