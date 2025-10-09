using System;
using UnityEngine;

[Serializable]
public class LessonBlock
{
    public string heading;
    public string text;
    public string[] bullets;
}

[Serializable]
public class LessonData
{
    public string id;
    public string title;
    public string topic;
    public string description;
    public LessonBlock[] lesson;
}

