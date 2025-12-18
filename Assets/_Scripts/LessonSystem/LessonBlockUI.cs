// using UnityEngine;
// using UnityEngine.UI;
// using TexDrawLib;

// public class LessonBlockUI : MonoBehaviour
// {
//     // public TextMeshProUGUI headingText;
//     public TEXDraw bodyText;

//     // For displaying Images
//     public Transform imageContainer;
//     public GameObject imagePrefab;

//     public void Setup(LessonBlock data)
//     {
//         // text
//         if (data.sections != null && data.sections.Length > 0)
//         {
//             string sectionList = "";
//             foreach (string section in data.sections)
//                 sectionList += section + "\n";

//             bodyText.text += "\n" + sectionList;
//         }

//         // images
//         if (data.imagePaths != null)
//         {
//             foreach (string path in data.imagePaths)
//             {
//                 GameObject imgObj = Instantiate(imagePrefab, imageContainer);
//                 Image img = imgObj.GetComponent<Image>();
//                 img.sprite = Resources.Load<Sprite>(path);
//             }
//         }
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using TexDrawLib;

public class LessonBlockUI : MonoBehaviour
{
    [Header("Content Prefabs")]
    public GameObject textPrefab;
    public GameObject imagePrefab;

    [Header("Content Root")]
    public Transform contentRoot;

    public void Setup(LessonBlock data)
    {
        if (data == null || data.contents == null)
        {
            Debug.LogWarning("LessonBlock has no contents.");
            return;
        }

        foreach (var content in data.contents)
        {
            switch (content.type)
            {
                case LessonContentType.Text:
                    CreateText(content.lines);
                    break;

                case LessonContentType.Image:
                    CreateImage(content);
                    break;
            }
        }
    }

    void CreateText(string[] lines)
    {
        if (lines == null || lines.Length == 0)
            return;

        var obj = Instantiate(textPrefab, contentRoot);
        var tex = obj.GetComponent<TEXDraw>();

        // Join lines cleanly for TEXDraw
        tex.text = string.Join("\n", lines);
    }

    void CreateImage(LessonContent content)
    {
        var obj = Instantiate(imagePrefab, contentRoot);
        var img = obj.GetComponent<Image>();

        Sprite sprite = Resources.Load<Sprite>(content.imagePath);
        if (sprite == null)
        {
            Debug.LogError($"Image not found at path: {content.imagePath}");
            return;
        }

        img.sprite = sprite;
    }
}
