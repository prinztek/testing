using UnityEngine;
using TMPro;

public class LessonManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI topicText;
    public TextMeshProUGUI descriptionText;
    public Transform lessonContainer; // parent object for lesson items
    public GameObject lessonBlockPrefab; // prefab for displaying text

    private LessonData currentLesson;

    public void LoadLesson(string moduleId)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"Modules/{moduleId}");
        if (jsonFile == null)
        {
            Debug.LogError($"❌ Lesson file not found: {moduleId}");
            return;
        }

        currentLesson = JsonUtility.FromJson<LessonData>(jsonFile.text);
        DisplayLesson();
    }

    void DisplayLesson()
    {
        // Clear previous content
        foreach (Transform child in lessonContainer)
            Destroy(child.gameObject);

        // Set basic info
        titleText.text = currentLesson.title;
        topicText.text = currentLesson.topic;
        descriptionText.text = currentLesson.description;

        // Generate each lesson block
        foreach (var block in currentLesson.lesson)
        {
            GameObject blockObj = Instantiate(lessonBlockPrefab, lessonContainer);
            LessonBlockUI ui = blockObj.GetComponent<LessonBlockUI>();
            ui.Setup(block);
        }
    }
}
