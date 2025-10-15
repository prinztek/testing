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

    // default lessonButton selected
    private Button defaultLessonButton;

    void Start()
    {
        GenerateLessonButtons();
    }

    void GenerateLessonButtons()
    {
        TextAsset[] lessonFiles = Resources.LoadAll<TextAsset>("Modules");
        lessonFiles = lessonFiles.OrderBy(f => f.name).ToArray();

        bool isFirst = true; // Flag for first module

        foreach (TextAsset file in lessonFiles)
        {
            // Parse title from JSON
            LessonData data = JsonUtility.FromJson<LessonData>(file.text);

            GameObject newButton = Instantiate(lessonButtonPrefab, buttonContainer);
            TextMeshProUGUI label = newButton.GetComponentInChildren<TextMeshProUGUI>();
            label.text = data.title;

            string moduleId = data.id;

            Button buttonComponent = newButton.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() =>
            {
                OnLessonButtonClicked(moduleId);
            });

            // Automatically select the first lesson
            if (isFirst)
            {
                isFirst = false;
                defaultLessonButton = buttonComponent; // Store the reference if needed later
                OnLessonButtonClicked(moduleId);       // Load the first lesson by default
            }
        }
    }


    void OnLessonButtonClicked(string moduleId)
    {
        // Debug.Log($"📘 Selected: {moduleId}");
        lessonManager.LoadLesson(moduleId);
    }
}
