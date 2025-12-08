// SaveDataModels.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public bool isUnlocked;
    public bool isCompleted;
    public float bestTime;
}

[Serializable]
public class ChapterData
{
    public LevelData[] levels = new LevelData[8];
}

[Serializable]
public class JSONSaveData
{
    public ChapterData[] chapters = new ChapterData[3];

    public JSONSaveData()
    {
        for (int i = 0; i < chapters.Length; i++)
        {
            chapters[i] = new ChapterData();
            for (int j = 0; j < chapters[i].levels.Length; j++)
            {
                chapters[i].levels[j] = new LevelData();
            }
        }
        // unlock first
        chapters[0].levels[0].isUnlocked = true;
    }
}

[Serializable]
public class JSONSettingsGlobalData
{
    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public float uiVolume = 1.0f;
    public float shakeMultiplier = 1.0f;
    public int lastUsedSlot = -1; // this is for multiple save files

    public JSONSettingsGlobalData() { }

    public JSONSettingsGlobalData(float master, float music, float sfx, float ui, float shakeMultiplier)
    {
        masterVolume = master;
        musicVolume = music;
        sfxVolume = sfx;
        uiVolume = ui;
        this.shakeMultiplier = shakeMultiplier;
    }
}

[Serializable]
public class JSONPlayerData
{
    public int gold = 0;
    public Dictionary<string, int> items = new Dictionary<string, int>();
    public List<string> unlockedSkills = new List<string>();
    public string equippedMeleeWeaponId = "";
    public string equippedRangedWeaponId = "";
}

[Serializable]
public class JSONUsedMathQuestionData
{
    public List<int> UsedMathQuestionIds = new List<int>();
}

[Serializable]
public class GameData
{
    public string playerName;
    public long lastUpdated;
    public JSONSaveData save;
    public JSONPlayerData player;
    public JSONUsedMathQuestionData questions;

    public GameData(string playerName)
    {
        this.playerName = playerName;
        lastUpdated = DateTime.Now.ToBinary();

        save = new JSONSaveData();            // auto-unlocks Chapter0 Level0
        player = new JSONPlayerData();
        questions = new JSONUsedMathQuestionData();
    }
}
