using UnityEngine;
using TMPro;
using TexDrawLib;

public class LessonBlockUI : MonoBehaviour
{
    // public TextMeshProUGUI headingText;
    public TEXDraw bodyText;

    public void Setup(LessonBlock data)
    {
        if (data.sections != null && data.sections.Length > 0)
        {
            string sectionList = "";
            foreach (string section in data.sections)
                sectionList += section + "\n";

            bodyText.text += "\n" + sectionList;
        }
    }
}
