using System.IO;
using UnityEngine;

public static class JSONSaveSystem
{
    private static readonly string savePath = Application.persistentDataPath + "/save.json";

    public static void SaveGame(JSONSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to: {savePath}");
    }

    public static JSONSaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<JSONSaveData>(json);
        }
        else
        {
            Debug.Log("No save found, creating new save data.");
            return new JSONSaveData();
        }
    }
}
