using System.IO;
using UnityEngine;

public static class JSONSaveSystem
{
    private static readonly string savePath = Application.persistentDataPath + "/save.json";

    public static void SaveGame(JSONSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        // Debug.Log($"Game saved to: {savePath}");
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
            // Debug.Log("No save found, creating new save data.");
            return new JSONSaveData();
        }
    }

    public static void SavePlayer(JSONPlayerData data)
    {
        string path = Path.Combine(Application.persistentDataPath, "player_save.json");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        // Debug.Log($"Game saved to: {path}");

    }

    public static JSONPlayerData LoadPlayer()
    {
        string path = Path.Combine(Application.persistentDataPath, "player_save.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<JSONPlayerData>(json);
        }
        return null;
    }

    // Math Question Used IDs Save/Load
    public static void SaveUsedMathQuestions(JSONUsedMathQuestionData data)
    {
        string path = Path.Combine(Application.persistentDataPath, "used_math_questions.json");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }
    public static JSONUsedMathQuestionData LoadUsedMathQuestions()
    {
        string path = Path.Combine(Application.persistentDataPath, "used_math_questions.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<JSONUsedMathQuestionData>(json);
        }
        return new JSONUsedMathQuestionData();
    }

}
