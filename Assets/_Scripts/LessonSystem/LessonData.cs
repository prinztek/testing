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

[Serializable]
public class LessonBlock
{
    public LessonContent[] contents;
}


[Serializable]
public class LessonContent
{
    public LessonContentType type;
    // TEXT
    public string[] lines;
    // IMAGE
    public string imagePath;
}

public enum LessonContentType
{
    Text,
    Image
    // Interactive Objects (future)
}




// Data Models for Math Modules