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

            buttonComponent.onClick.AddListener(() =>
            {
                OnLessonButtonClicked(moduleId);
            });

            // Add a colorful border to the button that matches the current MathTopic
            AddBorderIfMatchesTopic(data.title, newButton);

            if (isFirst)
            {
                isFirst = false;
                defaultLessonButton = buttonComponent;
            }
        }
    }

    void OnLessonButtonClicked(string moduleId)
    {
        lessonManager.LoadLesson(moduleId);
    }

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
