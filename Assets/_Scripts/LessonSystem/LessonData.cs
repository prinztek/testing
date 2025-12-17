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
    public string heading;
    public string[] sections;
}




// Data Models for Math Modules