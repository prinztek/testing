using UnityEngine;
using System.Collections;

public class BuffAcquireVFX : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Animator visualAnimator;
    [SerializeField] private string visualStateName = "Acquire";

    public void Play()
    {
        if (visualAnimator == null) return;

        visualAnimator.Play(visualStateName, 0, 0f);

        float clipLength = GetClipLength();
        Destroy(gameObject, clipLength);
    }

    private float GetClipLength()
    {
        if (visualAnimator.runtimeAnimatorController == null) return 1f;

        foreach (var clip in visualAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == visualStateName)
                return clip.length;
        }

        Debug.LogWarning($"Clip '{visualStateName}' not found. Defaulting to 1s");
        return 1f;
    }
}
