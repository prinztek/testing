using System.Collections.Generic;
using UnityEngine;

public class PlayerDataHandler : MonoBehaviour
{
    public CharacterStats characterStats;
    public PlayerInventory inventory;

    void Start()
    {
        LoadPlayerFromData(GameManager.Instance.playerData);
    }

    // From JSON file to Game
    public void LoadPlayerFromData(JSONPlayerData data)
    {
        Debug.Log(data);

        // Set gold via property -> triggers OnGoldChanged
        inventory.Gold = data.gold;

        // Build inventory via property -> triggers OnInventoryChanged
        var loadedItems = new List<InventorySlot>();
        foreach (var (key, value) in data.items)
        {
            var item = GameManager.Instance.itemDatabase.GetItemById(key);

            if (item != null) loadedItems.Add(new InventorySlot(item, value));
        }
        inventory.OwnedItems = loadedItems;

        // Equip weapons
        if (!string.IsNullOrEmpty(data.equippedMeleeWeaponId))
        {
            var melee = GameManager.Instance.itemDatabase.GetItemById(data.equippedMeleeWeaponId);
            if (melee != null) characterStats.EquipMeleeWeapon(melee);
        }

        if (!string.IsNullOrEmpty(data.equippedRangedWeaponId))
        {
            var ranged = GameManager.Instance.itemDatabase.GetItemById(data.equippedRangedWeaponId);
            if (ranged != null) characterStats.EquipRangedWeapon(ranged);
        }

        // Unlock skills
        foreach (var skillName in data.unlockedSkills)
        {
            if (System.Enum.TryParse(skillName, out SkillType skill))
                characterStats.UnlockSkill(skill);
        }

        // Debug.Log("Player loaded from data. UI events automatically fired.");
    }

    // From Game to JSON file
    public void SavePlayerToData(JSONPlayerData data)
    {
        data.gold = inventory.Gold;
        // Clear JSON list, not Inventory
        data.items.Clear();

        // Clear the items only AFTER adding them to data.items
        foreach (var slot in inventory.OwnedItems)
        {
            data.items[slot.item.itemName] = slot.quantity;
        }

        // DebugPrintDict("AFTER SAVE: data.items", data.items);

        data.equippedMeleeWeaponId = characterStats.equippedMeleeWeapon != null ?
            characterStats.equippedMeleeWeapon.itemName : "";

        data.equippedRangedWeaponId = characterStats.equippedRangedWeapon != null ?
            characterStats.equippedRangedWeapon.itemName : "";

        data.unlockedSkills.Clear();
        foreach (SkillType skill in System.Enum.GetValues(typeof(SkillType)))
        {
            if (characterStats.HasSkill(skill))
                data.unlockedSkills.Add(skill.ToString());
        }

        // Debug.Log("Player saved to data.");
    }
    private void DebugPrintDict(string title, Dictionary<string, int> dict)
    {
        Debug.Log($"----- {title} -----");

        if (dict == null)
        {
            Debug.Log("Dictionary = NULL");
            return;
        }

        if (dict.Count == 0)
        {
            Debug.Log("Dictionary EMPTY");
            return;
        }

        foreach (var kv in dict)
            Debug.Log($"JSON -> Key: {kv.Key} | Qty: {kv.Value}");
    }


}
