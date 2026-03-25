using UnityEngine;

public class BadgeManager : MonoBehaviour
{
    private JSONSaveData saveData;
    [SerializeField] private GameObject badgeUIPrefab; // assign in inspector
    [SerializeField] private Transform badgeCanvas; // assign in inspector

    public void Initialize(JSONSaveData data)
    {
        saveData = data;
    }

    // =========================
    // CORE
    // =========================

    bool IsUnlocked(string id)
    {
        return saveData.unlockedBadges.Contains(id);
    }

    // additional function is to add a badge prefab to display
    void Unlock(string id, string debugName)
    {
        if (IsUnlocked(id)) return;

        saveData.unlockedBadges.Add(id);
        Debug.Log($"Badge Unlocked: {debugName}");

        GameManager.Instance.SaveGame(); // auto-save on unlock

        // Instantiate badge UI (optional)
        if (badgeUIPrefab != null)
        {
            GameObject badgeUI = Instantiate(badgeUIPrefab, badgeCanvas);
            Destroy(badgeUI, 3f); // auto-destroy after 3 seconds
            // badgeUI.GetComponent<BadgeUI>().Setup(debugName); // assuming you have a BadgeUI script to set up the display
        }
    }

    // =========================
    // BADGE TRIGGERS
    // =========================

    public void FirstLevelComplete()
    {
        if (saveData.firstLevelCompleted) return;

        saveData.firstLevelCompleted = true;
        Unlock("FIRST_STEP", "First Step");
    }

    public void FirstKill()
    {
        // if (saveData.firstKillDone) return;
        if (saveData.firstKillDone)
        {
            Debug.Log("First Kill already done, skipping badge unlock.");
            return;
        }
        ;

        saveData.firstKillDone = true;
        Unlock("FIRST_KILL", "First Blood");
    }

    public void ChapterStart(int chapterIndex)
    {
        switch (chapterIndex)
        {
            case 0: Unlock("PERM_START", "Pattern Seeker"); break;
            case 1: Unlock("COMBO_START", "Strategic Thinker"); break;
            case 2: Unlock("PROB_START", "Risk Taker"); break;
        }
    }

    public void ChapterComplete(int chapterIndex)
    {
        switch (chapterIndex)
        {
            case 0: Unlock("PERM_MASTER", "Permutation Master"); break;
            case 1: Unlock("COMBO_MASTER", "Combination Master"); break;
            case 2: Unlock("PROB_MASTER", "Probability Master"); break;
        }

        CheckGameComplete();
    }

    void CheckGameComplete()
    {
        bool c1 = IsUnlocked("PERM_MASTER");
        bool c2 = IsUnlocked("COMBO_MASTER");
        bool c3 = IsUnlocked("PROB_MASTER");

        if (c1 && c2 && c3)
        {
            Unlock("GAME_COMPLETE", "Arithmos Champion");
        }
    }
}