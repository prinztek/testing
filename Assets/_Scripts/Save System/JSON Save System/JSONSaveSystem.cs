using System.IO;
using UnityEngine;
using Newtonsoft.Json;

// FILE DATA HANDLER
// SERIALIZE AND WRITE
// READ AND DESERIALIZE
public static class JSONSaveSystem
{
    private static readonly string savePath = Path.Combine(Application.persistentDataPath, "save.json");

    // ==============================
    //           GAME SAVE
    // ==============================

    public static void SaveGame(JSONSaveData data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(savePath, json);
        // Debug.Log($"Game saved to: {savePath}");
    }

    public static JSONSaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonConvert.DeserializeObject<JSONSaveData>(json);
        }

        Debug.Log("No main game save found, creating new save.");
        return new JSONSaveData();
    }


    // ==============================
    //         PLAYER SAVE
    // ==============================

    public static void SavePlayer(JSONPlayerData data)
    {
        string path = Path.Combine(Application.persistentDataPath, "player_save.json");

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(path, json);

        Debug.Log($"Player saved to: {path}");
    }

    public static JSONPlayerData LoadPlayer()
    {
        string path = Path.Combine(Application.persistentDataPath, "player_save.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<JSONPlayerData>(json);
        }

        Debug.Log("No player save found. Returning empty player data.");
        return new JSONPlayerData();
    }


    // ==============================
    //    USED MATH QUESTIONS
    // ==============================

    public static void SaveUsedMathQuestions(JSONUsedMathQuestionData data)
    {
        string path = Path.Combine(Application.persistentDataPath, "used_math_questions.json");

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public static JSONUsedMathQuestionData LoadUsedMathQuestions()
    {
        string path = Path.Combine(Application.persistentDataPath, "used_math_questions.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<JSONUsedMathQuestionData>(json);
        }

        return new JSONUsedMathQuestionData();
    }


    // ==============================
    //          GLOBA SETTINGS NON-SPECIFIC TO SAVE SLOTS
    // ==============================

    public static JSONSettingsGlobalData LoadSettingsGlobal()
    {
        string path = Path.Combine(Application.persistentDataPath, "settings_global.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<JSONSettingsGlobalData>(json);
        }

        return new JSONSettingsGlobalData();
    }

    public static void SaveSettingsGlobal(JSONSettingsGlobalData data)
    {
        string path = Path.Combine(Application.persistentDataPath, "settings_global.json");

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(path, json);
    }


    // Wrapper to combine all 3 into 1 file

    public static void SaveSlot(string profileId)
    {
        // combines individual saves into a single one gameData
        JSONSaveData save = LoadGame();
        JSONPlayerData player = LoadPlayer();
        JSONUsedMathQuestionData questions = LoadUsedMathQuestions();

        GameData wrapper = new GameData
        {
            save = save,
            player = player,
            questions = questions
        };
        string path = Path.Combine(Application.persistentDataPath, $"game_data_{profileId}.json");

        string json = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public static GameData LoadSlot(string profileId)
    {
        string path = Path.Combine(Application.persistentDataPath, $"game_data_{profileId}.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<GameData>(json);
        }

        Debug.LogWarning("Save slot not found: " + profileId);
        return new GameData();
    }

}
