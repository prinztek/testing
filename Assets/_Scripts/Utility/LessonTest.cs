using UnityEngine;

public class LessonTest : MonoBehaviour
{
    public LessonBlockUI lessonUI;

    void Start()
    {
        LessonData lesson = LessonLoader.LoadLessonFromResources(
            "Modules/Math10_Q3_M05"
        );

        // lessonUI.BuildLesson(lesson);

    }

}
