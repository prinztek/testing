using UnityEngine;

public class BadgePanel : MonoBehaviour
{
    [SerializeField] public BadgeDatabase badgeDatabase; // all badges
    [SerializeField] private GameObject badgeUIPrefab;     // prefab with Badge script
    [SerializeField] private Transform badgeListParent; // assign in inspector
    private JSONSaveData saveData;

    private void Start()
    {
        PopulatePanel();
    }

    /// <summary>
    /// Shows all chapter buttons and sets their interactability based on save data.
    /// </summary>

    private void PopulatePanel()
    {
        saveData = GameManager.Instance.currentData;

        foreach (var badgeData in badgeDatabase.badgeDataList)
        {
            GameObject badgeGO = Instantiate(badgeUIPrefab, badgeListParent);
            Badge badgeUI = badgeGO.GetComponent<Badge>();

            bool unlocked = saveData.unlockedBadges.Contains(badgeData.displayName);

            // if unlocked, show normally
            // if locked, show faded

            if (unlocked)
            {
                badgeUI.Setup(badgeData);
            }
            else
            {
                // For locked badges, you can choose to show a default "locked" icon or just fade the existing one
                badgeUI.Setup(badgeData); // still setup to show name/description, but you can modify this to show "Locked" instead
                // Optionally, you can add a visual indicator for locked badges here (e.g., gray out the icon)
                badgeUI.GetComponent<CanvasGroup>().alpha = 0.5f; // example of fading out locked badges
            }
        }
    }
}
