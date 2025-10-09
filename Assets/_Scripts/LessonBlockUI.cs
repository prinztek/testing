using UnityEngine;
using TMPro;

public class LessonBlockUI : MonoBehaviour
{
    public TextMeshProUGUI headingText;
    public TextMeshProUGUI bodyText;

    public void Setup(LessonBlock data)
    {
        headingText.text = data.heading;
        bodyText.text = data.text;

        if (data.bullets != null && data.bullets.Length > 0)
        {
            string bulletList = "";
            foreach (string bullet in data.bullets)
                bulletList += "• " + bullet + "\n";

            bodyText.text = bulletList;
        }
    }
}
