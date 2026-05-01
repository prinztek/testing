using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OutcomeItem : MonoBehaviour
{
    [Header("Selection Visuals")]
    [SerializeField] private Image numeratorSelector;
    [SerializeField] private Image denominatorSelector;

    [Header("Reference")]
    [SerializeField] private SimpleProbabilityManager manager;
    [SerializeField] private ProbabilityTwoEventsManager twoEventManager;
    [Header("Audio Clips")]
    [SerializeField] private AudioClip onSelectSoundClip;
    private RectTransform rectTransform;
    public Vector3 dragScale = new Vector3(1.1f, 1.1f, 1.1f); // scale when dragging
    private Vector3 originalScale;
    public string value = "";

    private bool selectedForNumerator = false;
    private bool selectedForDenominator = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale; // save original size
    }
    public void Start()
    {
        numeratorSelector.enabled = false;
        denominatorSelector.enabled = false;
    }

    // Called by Button component
    public void OnClick()
    {
        if (twoEventManager != null)
        {
            twoEventManager.HandleOutcomeItemClick(this);
        }
        else if (manager != null)
        {
            manager.HandleOutcomeItemClick(this);
        }
        else
        {
            Debug.LogWarning("No probability manager assigned to " + gameObject.name);
        }

        // StartCoroutine(ClickAnimation());

        // Play drag sound
        if (onSelectSoundClip != null)
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(onSelectSoundClip, transform, 0.3f);
        }
    }

    private IEnumerator ClickAnimation()
    {
        rectTransform.localScale = dragScale;
        yield return new WaitForSeconds(0.1f); // adjust timing
        rectTransform.localScale = originalScale;
    }

    // ============================
    // Numerator Toggle
    // ============================

    public void ToggleNumerator(OutcomeSlot slot)
    {
        selectedForNumerator = !selectedForNumerator;

        // selector visual
        numeratorSelector.enabled = selectedForNumerator;

        // add/remove from slot
        if (selectedForNumerator)
            slot.Add(value);
        else
            slot.Remove(value);
    }

    // ============================
    // Denominator Toggle
    // ============================

    public void ToggleDenominator(OutcomeSlot slot)
    {
        selectedForDenominator = !selectedForDenominator;

        // selector visual
        denominatorSelector.enabled = selectedForDenominator;

        // add/remove from slot
        if (selectedForDenominator)
            slot.Add(value);
        else
            slot.Remove(value);
    }

    public void ResetSelection()
    {
        selectedForNumerator = false;
        selectedForDenominator = false;

        if (numeratorSelector != null)
            numeratorSelector.enabled = false;

        if (denominatorSelector != null)
            denominatorSelector.enabled = false;
    }
}