using System.Collections;
using UnityEngine;

public class UI_Fade : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1.5f;
    private Coroutine changeAlphaCo;

    public IEnumerator FadeIn()
    {
        yield return FadeEffect(0); // black to transparent
    }
    public IEnumerator FadeOut()
    {
        yield return FadeEffect(1); // transparent to black
    }
    private IEnumerator FadeEffect(float targetAlpha)
    {
        // stop previous fade effect if any
        if (changeAlphaCo != null)
        {
            StopCoroutine(changeAlphaCo);
        }

        // start new fade effect
        changeAlphaCo = StartCoroutine(ChangeAlphaCo(targetAlpha));
        yield return changeAlphaCo;
    }

    private IEnumerator ChangeAlphaCo(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha; // get current alpha
        float timePassed = 0f; // how time much time has passed

        while (timePassed < fadeDuration)
        {
            timePassed += Time.deltaTime; // increase time passed
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timePassed / fadeDuration); // change alpha (fade effect)
            yield return null; // wait for next frame
        }

        canvasGroup.alpha = targetAlpha;
    }
}
