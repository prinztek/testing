using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public Transform levelButtonParent;

    public TMP_Text chapterNumberTitleText;

    private const int LevelsPerChapter = 8;

    void Start()
    {
        chapterNumberTitleText.text = $"Chapter {GameData.CurrentChapter}";
        GenerateLevelButtons(GameData.CurrentChapter);
    }

    void GenerateLevelButtons(int chapter)
    {
        for (int i = 1; i <= LevelsPerChapter; i++)
        {
            Debug.Log(i);
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonParent);
            Button btn = buttonObj.GetComponent<Button>();
            TMP_Text btnText = buttonObj.GetComponentInChildren<TMP_Text>();

            string levelSceneName = $"Level{chapter}_{i}";
            btnText.text = $"Level {i}";

            btn.onClick.AddListener(() => LoadLevel(levelSceneName));
        }
    }

    public void LoadLevel(string levelSceneName)
    {
        SceneManager.LoadScene(levelSceneName);
    }

    public void BackToChapterSelect()
    {
        SceneManager.LoadScene("ChapterSelect");
    }
}
