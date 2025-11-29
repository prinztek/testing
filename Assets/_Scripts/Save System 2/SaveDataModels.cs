// SaveDataModels.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData2
{
    public bool isUnlocked;
    public bool isCompleted;
    public float bestTime;
}

[Serializable]
public class ChapterData2
{
    public LevelData[] levels = new LevelData[8];
}

[Serializable]
public class JSONSaveData2
{
    public ChapterData[] chapters = new ChapterData[3];

    public JSONSaveData2()
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
public class JSONSettingsData2
{
    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public float uiVolume = 1.0f;
    public bool screenShakeEnabled = true;

    public JSONSettingsData2() { }

    public JSONSettingsData2(float master, float music, float sfx, float ui, bool shake)
    {
        masterVolume = master;
        musicVolume = music;
        sfxVolume = sfx;
        uiVolume = ui;
        screenShakeEnabled = shake;
    }
}

[Serializable]
public class JSONPlayerData2
{
    public int gold = 0;
    public Dictionary<string, int> items = new Dictionary<string, int>();
    public List<string> unlockedSkills = new List<string>();
    public string equippedMeleeWeaponId = "";
    public string equippedRangedWeaponId = "";
}

[Serializable]
public class JSONUsedMathQuestionData2
{
    public List<int> UsedMathQuestionIds = new List<int>();
}

[Serializable]
public class JSONGlobalData
{
    public int lastUsedSlot = 1;
    public JSONSettingsData2 settings = new JSONSettingsData2();
}
