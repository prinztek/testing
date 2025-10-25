using UnityEngine;
using TMPro;

public class LessonManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform lessonContainer; // parent object for lesson items
    public GameObject lessonBlockPrefab; // prefab for displaying text
    public GameObject lessonBlockHeaderPrefab; // Assign manually in the Inspector

    private LessonData currentLesson;

    public void LoadLesson(string moduleId)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"Modules/{moduleId}");
        if (jsonFile == null)
        {
            // Debug.LogError($"❌ Lesson file not found: {moduleId}");
            return;
        }

        currentLesson = JsonUtility.FromJson<LessonData>(jsonFile.text);
        DisplayLesson();
    }

    void DisplayLesson()
    {

        // Clear previous content
        foreach (Transform child in lessonContainer)
        {
            Destroy(child.gameObject);
        }

        // Generate each lesson block
        // This creates a visual representation of each lesson block in the UI
        // by instantiating a prefab for each block and setting it up with the block data.

        GameObject blockHeaderObj = Instantiate(lessonBlockHeaderPrefab, lessonContainer);
        LessonBlockHeaderUI headerUI = blockHeaderObj.GetComponent<LessonBlockHeaderUI>();
        headerUI.Setup(currentLesson);

        foreach (var block in currentLesson.lesson)
        {
            GameObject blockObj = Instantiate(lessonBlockPrefab, lessonContainer); // Instantiate lesson block prefab
            LessonBlockUI ui = blockObj.GetComponent<LessonBlockUI>(); // Get the LessonBlockUI component
            ui.Setup(block); // Setup the UI with the block data
        }
    }
}
