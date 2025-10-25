using UnityEngine;
using TMPro;

public class LessonBlockUI : MonoBehaviour
{
    public TextMeshProUGUI headingText;
    public TextMeshProUGUI bodyText;

    public void Setup(LessonBlock data)
    {
        // if (!string.IsNullOrWhiteSpace(data.headingText))
        // {
        //     headingText.text = data.headingText;
        // }
        // else
        // {
        //     headingText.gameObject.SetActive(false);
        // }

        // if (!string.IsNullOrWhiteSpace(data.bodyText))
        // {
        //     bodyText.text = data.bodyText;
        // }
        // else
        // {
        //     bodyText.gameObject.SetActive(false);
        // }

        if (data.heading == "")
        {
            Debug.Log("Empty");
        }

        headingText.text = data.heading;
        bodyText.text = data.text;

        if (data.bullets != null && data.bullets.Length > 0)
        {
            string bulletList = "";
            foreach (string bullet in data.bullets)
                bulletList += "\t• " + bullet + "\n";

            bodyText.text = bulletList;
        }
    }
}
