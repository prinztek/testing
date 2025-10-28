using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private GameObject completedMark;

    private int chapterIndex;
    private int levelIndex;

    public void Setup(int chapter, int level, LevelData data)
    {
        chapterIndex = chapter;
        levelIndex = level;

        // Force button to correct state at runtime
        if (button != null)
        {
            button.interactable = data.isUnlocked;

            // Remove previous listeners and add the correct one
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        if (label != null)
            label.text = (level + 1).ToString();

        if (completedMark != null)
            completedMark.SetActive(data.isCompleted);
    }


    public void OnClick()
    {
        // You can handle transitions here or in GameManager
        GameManager.Instance.LoadLevel(chapterIndex, levelIndex);
    }
}
