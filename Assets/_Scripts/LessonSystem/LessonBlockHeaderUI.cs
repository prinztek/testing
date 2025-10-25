using UnityEngine;
using TMPro;
public class LessonBlockHeaderUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI topicText;
    public TextMeshProUGUI descriptionText;
    public void Setup(LessonData data)
    {
        titleText.text = data.title;
        topicText.text = data.topic;
        descriptionText.text = data.description;
    }
}

