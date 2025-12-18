using System;

[Serializable]
public class LessonData
{
    public string id;
    public string title;
    public string topic;
    public string description;
    public LessonBlock[] lesson;
}

public enum LessonContentType
{
    Text,
    Image
    // Interactive (later)
}

[Serializable]
public class LessonBlock
{
    public string heading;
    public string[] sections;
    public string[] imagePaths;
}




// Data Models for Math Modules