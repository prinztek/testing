using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

// FILE DATA HANDLER - Based on Shaped by Rain Studios
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
    //          GLOBAL SETTINGS NON-SPECIFIC TO SAVE SLOTS
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

    public static void SaveSlot(GameData gameData, string profileId)
    {
        // base case - if the profile id is null, return right away
        if (profileId == null)
        {
            return;
        }
        string folderPath = Path.Combine(Application.persistentDataPath, profileId);

        // Create folder if missing
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fullPath = Path.Combine(folderPath, "game_data.json");

        string json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
        File.WriteAllText(fullPath, json);

        Debug.Log($"Saved profile '{profileId}' to: {fullPath}");
    }


    public static GameData LoadSlot(string profileId)
    {
        // base case - if the profile id is null, return right away
        if (profileId == null)
        {
            return null;
        }

        string folderPath = Path.Combine(Application.persistentDataPath, profileId);
        string fullPath = Path.Combine(folderPath, "game_data.json");

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<GameData>(json);
        }

        Debug.LogWarning($"No save found for profile '{profileId}', creating empty GameData.");
        return new GameData();
    }


    // ========================================================================================
    //          MULTIPLE SAVE SLOTS
    // ========================================================================================
    public static Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();
        string dataFileName = "game_data.json";

        // loop over all directory names in the data directory path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(Application.persistentDataPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;

            // defensive programming - check if the data file exists
            // if it doesn't, then this folder isn't a profile and should be skipped
            string fullPath = Path.Combine(Application.persistentDataPath, profileId, dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: "
                    + profileId);
                continue;
            }

            // load the game data for this profile and put it in the dictionary
            GameData profileData = LoadSlot(profileId);
            // defensive programming - ensure the profile data isn't null,
            // because if it is then something went wrong and we should let ourselves know
            if (profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogError("Tried to load profile but something went wrong. ProfileId: " + profileId);
            }
        }

        return profileDictionary;
    }

    public static string GetMostRecentlyUpdatedProfileId()
    {
        string mostRecentProfileId = null;

        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach (KeyValuePair<string, GameData> pair in profilesGameData)
        {
            string profileId = pair.Key;
            GameData gameData = pair.Value;

            // skip this entry if the gamedata is null
            if (gameData == null)
            {
                continue;
            }

            // if this is the first data we've come across that exists, it's the most recent so far
            if (mostRecentProfileId == null)
            {
                mostRecentProfileId = profileId;
            }
            // otherwise, compare to see which date is the most recent
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                // the greatest DateTime value is the most recent
                if (newDateTime > mostRecentDateTime)
                {
                    mostRecentProfileId = profileId;
                }
            }
        }
        return mostRecentProfileId;
    }
}
