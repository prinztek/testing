using UnityEngine;
using System.Collections;

public class MushroomJumpPad : MonoBehaviour
{
    [Header("Timing")]
    public float crossFadeTime = 0.05f;

    [SerializeField] private float bounceForce = 20f;
    [SerializeField] private Animator animator;

    private Coroutine jumpPadCoroutine;

    private static readonly int IDLE = Animator.StringToHash("Mushroom_Idle");
    private static readonly int PROPEL = Animator.StringToHash("Mushroom_Propel");

    void Awake()
    {
        if (!animator)
            animator = GetComponentInChildren<Animator>();

        animator.Play(IDLE, 0, 0f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        // Restart animation cleanly
        if (jumpPadCoroutine != null)
        {
            StopCoroutine(jumpPadCoroutine);
        }

        jumpPadCoroutine = StartCoroutine(MushroomJumpPadRoutine());

        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        }
    }

    IEnumerator MushroomJumpPadRoutine()
    {
        animator.CrossFade(PROPEL, crossFadeTime);

        yield return new WaitForSeconds(GetClipLength("Mushroom_Propel"));

        animator.CrossFade(IDLE, crossFadeTime);

        jumpPadCoroutine = null;
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
