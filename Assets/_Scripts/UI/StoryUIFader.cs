using System.Collections;
using UnityEngine;

public class StoryUIFader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        StartCoroutine(StorySequence());
    }

    private IEnumerator StorySequence()
    {
        yield return GameManager.Instance.uiFade.FadeIn();
    }
}
