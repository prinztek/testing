using System.Collections;
using UnityEngine;

public class UI_Fade : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float defaultFadeDuration = 0.4f;
    [SerializeField] private float fastFadeDuration = 0.2f;
    private Coroutine changeAlphaCo;

    public IEnumerator FadeIn()
    {
        yield return FadeEffect(0f, defaultFadeDuration);
    }

    public IEnumerator FadeOut()
    {
        yield return FadeEffect(1f, defaultFadeDuration);
    }

    public IEnumerator FastFadeIn()
    {
        yield return FadeEffect(0f, fastFadeDuration);
    }

    public IEnumerator FastFadeOut()
    {
        yield return FadeEffect(1f, fastFadeDuration);
    }
    private IEnumerator FadeEffect(float targetAlpha, float duration)
    {
        if (changeAlphaCo != null)
            StopCoroutine(changeAlphaCo);

        changeAlphaCo = StartCoroutine(ChangeAlphaCo(targetAlpha, duration));
        yield return changeAlphaCo;
    }

    private IEnumerator ChangeAlphaCo(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha; // get current alpha
        float timePassed = 0f; // how time much time has passed

        while (timePassed < duration)
        {
            timePassed += Time.unscaledDeltaTime; // Changed from Time.deltaTime
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timePassed / duration);
            yield return null; // wait for next frame
        }

        canvasGroup.alpha = targetAlpha;
    }

}
