using UnityEngine;

public class PlayerDataHandler : MonoBehaviour
{
    public CharacterStats stats;
    public PlayerInventory inventory;

    void Start()
    {
        LoadPlayerFromData(GameManager.Instance.playerData);
    }

    public void LoadPlayerFromData(JSONPlayerData data)
    {
        stats.gold = data.gold;

        inventory.ownedItems.Clear();
        foreach (var itemName in data.ownedItemIds)
        {
            GameItem item = ItemDatabase.GetItemById(itemName);
            inventory.ownedItems.Add(new InventorySlot(item, 1));
        }
    }

    public void SavePlayerToData(JSONPlayerData data)
    {
        data.gold = stats.gold;

        data.ownedItemIds.Clear();
        foreach (var slot in inventory.ownedItems)
        {
            data.ownedItemIds.Add(slot.item.itemName);
        }
    }
}
