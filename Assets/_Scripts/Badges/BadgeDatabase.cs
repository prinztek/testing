using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BadgeDatabase", menuName = "Inventory/BadgeDatabase")]
public class BadgeDatabase : ScriptableObject
{
    public List<BadgeData> badgeDataList = new List<BadgeData>();

    // singleton for easy access
    private static BadgeDatabase instance;
    public static BadgeDatabase Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<BadgeDatabase>("BadgeDatabase");
            return instance;
        }
    }

    public BadgeData Get(string id)
    {
        var badge = badgeDataList.Find(i => i.displayName == id);
        if (badge == null)
            Debug.LogWarning($"Badge with ID '{id}' not found in database.");
        return badge;
    }
}