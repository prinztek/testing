using System;
using System.Collections.Generic;

[Serializable]
public class JSONPlayerData
{
    public int gold; // currently in player inventory
    public List<string> ownedItemIds = new(); // Item IDs = their name // currently in player inventory
    public List<string> unlockedSkills = new(); // currently in character stats
    public string equippedMeleeWeaponId = ""; // currently in character stats
    public string equippedRangedWeaponId = ""; // currently in character stats
}
