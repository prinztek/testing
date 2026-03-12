using System;
using UnityEngine;

[System.Serializable]
public class ProbabilityFraction
{
    public OutcomeSlot numeratorSlot;
    public OutcomeSlot denominatorSlot;

    public bool IsCorrect()
    {
        return numeratorSlot.IsCorrect() && denominatorSlot.IsCorrect();
    }

    public void Reset()
    {
        numeratorSlot.ResetSlot();
        denominatorSlot.ResetSlot();
    }
}