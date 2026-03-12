using System;
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


    public string value = "";

    private bool selectedForNumerator = false;
    private bool selectedForDenominator = false;

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