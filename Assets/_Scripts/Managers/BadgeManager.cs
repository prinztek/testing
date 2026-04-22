using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BadgeManager : MonoBehaviour
{
    [Header("Badge Setup")]
    [SerializeField] public BadgeDatabase badgeDatabase; // assign in inspector
    [SerializeField] private GameObject badgeUIPrefab;     // assign in inspector
    [SerializeField] private Transform badgeCanvas;        // assign in inspector

    private JSONSaveData saveData; // Your save data

    [Header("Audio Clips")]
    [SerializeField] private AudioClip onBadgeUnlockedClip;

    public void Initialize(JSONSaveData data)
    {
        saveData = data;
    }

    // =========================
    // CORE
    // =========================

    public bool IsUnlocked(string badgeId)
    {
        return saveData.unlockedBadges.Contains(badgeId);
    }

    public void UnlockBadge(string badgeId)
    {
        // Already unlocked
        if (IsUnlocked(badgeId)) return;

        // Get badge data from database
        BadgeData badgeData = badgeDatabase.GetBadgeData(badgeId);
        if (badgeData == null)
        {
            Debug.LogWarning($"Badge not found in database: {badgeId}");
            return;
        }

        // Add to save data
        saveData.unlockedBadges.Add(badgeId);
        Debug.Log($"Badge Unlocked: {badgeData.displayName}");

        // Auto-save
        GameManager.Instance.SaveGame();

        // Show UI
        if (badgeUIPrefab != null)
        {
            // GameObject badgeUI = Instantiate(badgeUIPrefab, badgeCanvas);
            // badgeUI.GetComponent<Badge>().Setup(badgeData);
            // Destroy(badgeUI, 3f); // auto-destroy after 3 seconds

            StartCoroutine(ShowBadgeRoutine(badgeData));
        }
    }

    private IEnumerator ShowBadgeRoutine(BadgeData badgeData)
    {
        // Play drag sound
        if (onBadgeUnlockedClip != null)
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(onBadgeUnlockedClip, transform, 0.3f);
        }

        GameObject badgeUI = Instantiate(badgeUIPrefab, badgeCanvas);
        badgeUI.GetComponent<Badge>().Setup(badgeData);
        yield return new WaitForSecondsRealtime(3f);
        Destroy(badgeUI);
    }

    // =========================
    // BADGE TRIGGERS
    // =========================

    public void FirstLevelComplete()
    {
        if (saveData.firstLevelCompleted) return;

        saveData.firstLevelCompleted = true;
        UnlockBadge("FIRST_STEP");
        // UnlockBadge("FIRST_STEP", "First Step");
    }

    public void FirstKill() // first enemy kill, not necessarily first level
    {
        // if (saveData.firstKillDone) return;
        if (saveData.firstKillDone)
        {
            Debug.Log("First Kill already done, skipping badge unlock.");
            return;
        }
        ;

        saveData.firstKillDone = true;
        UnlockBadge("FIRST_KILL");
    }

    public void ChapterStart(int chapterIndex) // 0-based index for chapters unlock every time a chapter starts
    {
        switch (chapterIndex)
        {
            case 0: UnlockBadge("PERMUTATION_START"); break;
            case 1: UnlockBadge("COMBINATION_START"); break;
            case 2: UnlockBadge("PROBABILITY_START"); break;
        }
    }

    public void ChapterComplete(int chapterIndex) // 0-based index for chapters unlock every time a chapter is completed
    {
        switch (chapterIndex)
        {
            case 0: UnlockBadge("PERMUTATION_MASTER"); break;
            case 1: UnlockBadge("COMBINATION_MASTER"); break;
            case 2: UnlockBadge("PROBABILITY_MASTER"); break;
        }

        CheckGameComplete();
    }

    void CheckGameComplete()
    {
        bool c1 = IsUnlocked("PERMUTATION_MASTER");
        bool c2 = IsUnlocked("COMBINATION_MASTER");
        bool c3 = IsUnlocked("PROBABILITY_MASTER");

        if (c1 && c2 && c3)
        {
            UnlockBadge("GAME_COMPLETE");
            // "GAME_COMPLETE", "Arithmos Champion"
        }
    }

}