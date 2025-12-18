using UnityEngine;
using Newtonsoft.Json;

public static class LessonLoader
{
    public static LessonData LoadLessonFromResources(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile == null)
        {
            Debug.LogError($"Lesson JSON not found: {fileName}");
            return null;
        }

        return JsonConvert.DeserializeObject<LessonData>(jsonFile.text);
    }
}
