using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LessonListManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform buttonContainer;
    public GameObject lessonButtonPrefab;
    public LessonManager lessonManager;
    public MathQuestionManager mathQuestionManager;
    private Button defaultLessonButton;
    private Button selectedButton;
    private GameObject selectedSelector;

    void Start()
    {
        GenerateLessonButtons();
    }

    void GenerateLessonButtons()
    {
        TextAsset[] lessonFiles = Resources.LoadAll<TextAsset>("Modules");
        lessonFiles = lessonFiles.OrderBy(f => f.name).ToArray();

        bool isFirst = true;

        foreach (TextAsset file in lessonFiles)
        {
            LessonData data = JsonUtility.FromJson<LessonData>(file.text);

            GameObject newButton = Instantiate(lessonButtonPrefab, buttonContainer);
            TextMeshProUGUI label = newButton.GetComponentInChildren<TextMeshProUGUI>();
            label.text = data.title;

            string moduleId = data.id;
            Button buttonComponent = newButton.GetComponent<Button>();

            // Find Selector (sprite indicator to show this button is selected) child (inactive by default)
            Transform selectedSelector = buttonComponent.transform.Find("Selector");
            if (selectedSelector != null)
            {
                selectedSelector.gameObject.SetActive(false);
            }

            buttonComponent.onClick.AddListener(() =>
            {
                SelectLesson(buttonComponent, selectedSelector?.gameObject, moduleId);
            });

            // Add a colorful border to the button that matches the current topic of the question
            AddBorderIfMatchesTopic(data.title, newButton);

            if (isFirst)
            {
                isFirst = false;
                defaultLessonButton = buttonComponent;
            }
        }

        // click the first button by default to load the first lesson
        if (defaultLessonButton != null)
        {
            defaultLessonButton.onClick.Invoke();
        }
    }

    void OnLessonButtonClicked(string moduleId)
    {
        lessonManager.LoadLesson(moduleId);
    }

    void SelectLesson(Button button, GameObject selector, string moduleId)
    {
        // Turn off previous selection
        if (selectedSelector != null)
            selectedSelector.SetActive(false);

        // Activate new selector
        selectedButton = button;
        selectedSelector = selector;

        if (selectedSelector != null)
            selectedSelector.SetActive(true);

        // Load lesson content
        lessonManager.LoadLesson(moduleId);
    }


    // to give the player a clue about which lesson is relevant to the current math question,
    // we can add a border to the button that matches the current topic of the question.
    // This way, players can easily identify which lesson they should review to find the information they need for solving the problem.    
    void AddBorderIfMatchesTopic(string lessonTitle, GameObject buttonObj)
    {
        // Normalize topic name (replace underscores with spaces)
        string normalizedTopic = mathQuestionManager.GetNormalizedTopicName();

        if (lessonTitle.Equals(normalizedTopic, System.StringComparison.OrdinalIgnoreCase))
        {
            Outline border = buttonObj.GetComponent<Outline>();
            if (border == null)
                border = buttonObj.AddComponent<Outline>();

            // Set base color (green)
            border.effectColor = new Color(0.3f, 0.9f, 0.3f);
            border.effectDistance = new Vector2(0.5f, 0.5f);

            // Add a pulsing effect
            PulsatingOutline pulser = buttonObj.GetComponent<PulsatingOutline>();
            if (pulser == null)
                pulser = buttonObj.AddComponent<PulsatingOutline>();

            pulser.targetOutline = border;
        }
    }
}
