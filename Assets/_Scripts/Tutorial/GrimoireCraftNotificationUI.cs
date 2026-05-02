using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GrimoireCraftNotification : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;

    private Coroutine currentRoutine;

    public void Show(string title, string desc)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        titleText.text = title;
        descText.text = desc;

        currentRoutine = StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        gameObject.SetActive(true);

        // ALWAYS reset before animation
        canvasGroup.alpha = 0f;

        // Fade In
        float t = 0;
        while (t < 0.2f)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = t / 0.2f;
            yield return null;
        }

        canvasGroup.alpha = 1;

        // Stay
        yield return new WaitForSecondsRealtime(1.5f);
        // Fade Out
        t = 0;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1 - (t / 0.3f);
            yield return null;
        }

        canvasGroup.alpha = 0;

        gameObject.SetActive(false);
        currentRoutine = null; // cleanup
    }
}