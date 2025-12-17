using UnityEngine;
using TMPro;
public class LessonBlockHeaderUI : MonoBehaviour
{
    public TextMeshProUGUI topicText;
    // public TextMeshProUGUI descriptionText;
    public void Setup(LessonData data)
    {
        // titleText.text = data.title;
        topicText.text = data.topic;
        // descriptionText.text = data.description;
    }

    // for now, only use the topic text (specific math module eg. Permutation and Its Conditions)
}

