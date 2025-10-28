[System.Serializable]
public class LevelData
{
    public bool isUnlocked;
    public bool isCompleted;
    public float bestTime;
}

[System.Serializable]
public class ChapterData
{
    public LevelData[] levels = new LevelData[8];
}

[System.Serializable]
public class JSONSaveData
{
    public ChapterData[] chapters = new ChapterData[3];

    public JSONSaveData()
    {
        // Initialize structure
        for (int i = 0; i < chapters.Length; i++)
        {
            chapters[i] = new ChapterData();
            for (int j = 0; j < chapters[i].levels.Length; j++)
            {
                chapters[i].levels[j] = new LevelData();
            }
        }

        // Unlock the very first level by default
        chapters[0].levels[0].isUnlocked = true;
    }
}
