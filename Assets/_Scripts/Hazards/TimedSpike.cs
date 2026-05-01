using UnityEngine;
using System.Collections;

public class TimedSpike : MonoBehaviour
{
    [Header("Timing")]
    public float popupDelay = 0.2f;
    public float activeTime = 1.0f;
    public float crossFadeTime = 0.05f;

    [Header("Refs")]
    [SerializeField] private Animator animator;

    private bool isPlayerInside;
    private bool isTriggered;
    private Coroutine spikeCoroutine;

    private static readonly int IDLE = Animator.StringToHash("Spike_Idle");
    private static readonly int POPUP = Animator.StringToHash("Spike_Popup");
    private static readonly int RETRACT = Animator.StringToHash("Spike_Retract");

    void Awake()
    {
        if (!animator)
            animator = GetComponentInChildren<Animator>();

        animator.Play(IDLE, 0, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Enemy")) return;

        isPlayerInside = true;

        // Latch trigger only once per cycle
        if (!isTriggered)
        {
            isTriggered = true;
            spikeCoroutine = StartCoroutine(SpikeRoutine());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Enemy")) return;

        isPlayerInside = false;
    }

    IEnumerator SpikeRoutine()
    {
        // Warning delay (cannot be cancelled)
        yield return new WaitForSeconds(popupDelay);

        animator.CrossFade(POPUP, crossFadeTime);

        // Active window
        yield return new WaitForSeconds(activeTime);

        animator.CrossFade(RETRACT, crossFadeTime);

        yield return new WaitForSeconds(GetClipLength("Spike_Retract"));

        animator.CrossFade(IDLE, crossFadeTime);

        // End of cycle
        isTriggered = false;
        spikeCoroutine = null;

        // Retrigger ONLY after full cycle
        if (isPlayerInside)
        {
            isTriggered = true;
            spikeCoroutine = StartCoroutine(SpikeRoutine());
        }
    }

    float GetClipLength(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        return 0.25f;
    }
}
