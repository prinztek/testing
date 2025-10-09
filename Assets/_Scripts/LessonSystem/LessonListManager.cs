using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LessonListManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform buttonContainer;        // Parent object for buttons
    public GameObject lessonButtonPrefab;    // Button prefab
    public LessonManager lessonManager;      // Reference to right-page LessonManager

    void Start()
    {
        GenerateLessonButtons();
    }

    void GenerateLessonButtons()
    {
        TextAsset[] lessonFiles = Resources.LoadAll<TextAsset>("Modules");
        lessonFiles = lessonFiles.OrderBy(f => f.name).ToArray();  // Add this line

        foreach (TextAsset file in lessonFiles)
        {
            // Parse title from JSON
            LessonData data = JsonUtility.FromJson<LessonData>(file.text);

            GameObject newButton = Instantiate(lessonButtonPrefab, buttonContainer);
            TextMeshProUGUI label = newButton.GetComponentInChildren<TextMeshProUGUI>();
            label.text = data.title;

            string moduleId = data.id; // needed for button click
            newButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnLessonButtonClicked(moduleId);
            });
        }
    }

    void OnLessonButtonClicked(string moduleId)
    {
        Debug.Log($"📘 Selected: {moduleId}");
        lessonManager.LoadLesson(moduleId);
    }
}
