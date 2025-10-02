using UnityEngine;

public static class SaveSystem
{
    private const string SaveKey = "GameSave";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log("💾 Saved game data: " + json);
    }

    public static SaveData Load()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("📂 Loaded game data: " + json);
            return data;
        }

        Debug.Log("⚠️ No save found, creating new SaveData.");
        return new SaveData();
    }

    // ✅ Reset progress (clear answered questions)
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        Debug.Log("🗑️ Cleared all saved progress!");
    }
}
