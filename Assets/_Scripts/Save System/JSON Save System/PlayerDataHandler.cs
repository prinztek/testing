using UnityEngine;

public class PlayerDataHandler : MonoBehaviour
{
    public CharacterStats characterStats;
    public PlayerInventory inventory;

    void Start()
    {
        LoadPlayerFromData(GameManager.Instance.playerData);
    }

    public void LoadPlayerFromData(JSONPlayerData data)
    {
        inventory.gold = data.gold;

        // Clear and rebuild inventory
        inventory.ownedItems.Clear();
        foreach (var id in data.ownedItemIds)
        {
            var item = GameManager.Instance.itemDatabase.GetItemById(id);
            if (item != null)
                inventory.ownedItems.Add(new InventorySlot(item, 1));
        }

        // Equipped weapons
        if (!string.IsNullOrEmpty(data.equippedMeleeWeaponId))
        {
            var melee = GameManager.Instance.itemDatabase.GetItemById(data.equippedMeleeWeaponId);
            if (melee != null)
                characterStats.EquipMeleeWeapon(melee);
        }

        if (!string.IsNullOrEmpty(data.equippedRangedWeaponId))
        {
            var ranged = GameManager.Instance.itemDatabase.GetItemById(data.equippedRangedWeaponId);
            if (ranged != null)
                characterStats.EquipRangedWeapon(ranged);
        }

        // Skills
        foreach (var skillName in data.unlockedSkills)
        {
            if (System.Enum.TryParse(skillName, out CharacterStats.SkillType skill))
                characterStats.UnlockSkill(skill);
        }

        Debug.Log("✅ Player loaded from data.");
    }

    public void SavePlayerToData(JSONPlayerData data)
    {
        data.gold = inventory.gold;
        data.ownedItemIds.Clear();

        foreach (var slot in inventory.ownedItems)
            data.ownedItemIds.Add(slot.item.itemName);

        data.equippedMeleeWeaponId = characterStats.equippedMeleeWeapon != null ?
            characterStats.equippedMeleeWeapon.itemName : "";

        data.equippedRangedWeaponId = characterStats.equippedRangedWeapon != null ?
            characterStats.equippedRangedWeapon.itemName : "";

        data.unlockedSkills.Clear();
        foreach (CharacterStats.SkillType skill in System.Enum.GetValues(typeof(CharacterStats.SkillType)))
        {
            if (characterStats.HasSkill(skill))
                data.unlockedSkills.Add(skill.ToString());
        }

        Debug.Log("💾 Player saved to data.");
    }

}
