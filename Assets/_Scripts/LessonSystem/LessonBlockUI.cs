using UnityEngine;
using UnityEngine.UI;
using TexDrawLib;

public class LessonBlockUI : MonoBehaviour
{
    // public TextMeshProUGUI headingText;
    public TEXDraw bodyText;

    // For displaying Images
    public Transform imageContainer;
    public GameObject imagePrefab;

    public void Setup(LessonBlock data)
    {
        // text
        if (data.sections != null && data.sections.Length > 0)
        {
            string sectionList = "";
            foreach (string section in data.sections)
                sectionList += section + "\n";

            bodyText.text += "\n" + sectionList;
        }

        // images
        if (data.imagePaths != null)
        {
            foreach (string path in data.imagePaths)
            {
                GameObject imgObj = Instantiate(imagePrefab, imageContainer);
                Image img = imgObj.GetComponent<Image>();
                img.sprite = Resources.Load<Sprite>(path);
            }
        }
    }
}
