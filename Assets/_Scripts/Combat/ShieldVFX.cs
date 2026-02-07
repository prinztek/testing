using UnityEngine;
using System.Collections;

public class ShieldVFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator shieldAnimator;
    [SerializeField] private SpriteRenderer shieldVisual;
    private Coroutine shieldRoutine;

    private void Awake()
    {
        // Shield starts disabled
        shieldVisual.enabled = false;
    }

    private void Start()
    {
        // Optionally enable shield on start
        // EnableShield();
    }

    public void EnableShield()
    {
        if (shieldRoutine != null)
            StopCoroutine(shieldRoutine);

        shieldVisual.enabled = true;
        shieldRoutine = StartCoroutine(ShieldStartRoutine());
    }

    public void DisableShield()
    {
        if (shieldRoutine != null)
            StopCoroutine(shieldRoutine);

        shieldVisual.enabled = false;
        shieldRoutine = StartCoroutine(ShieldEndRoutine());
    }

    private IEnumerator ShieldStartRoutine()
    {
        shieldAnimator.Play("start");

        // Wait for start animation to finish
        yield return new WaitForSeconds(
            shieldAnimator.GetCurrentAnimatorStateInfo(0).length
        );

        shieldAnimator.Play("loop");
    }

    private IEnumerator FireStartRoutine()
    {
        shieldAnimator.Play("start");

        // Wait for start animation to finish
        yield return new WaitForSeconds(
            shieldAnimator.GetCurrentAnimatorStateInfo(0).length
        );

        shieldAnimator.Play("loop");
    }

    private IEnumerator ShieldEndRoutine()
    {
        shieldAnimator.Play("end");

        // Wait for end animation to finish
        yield return new WaitForSeconds(
            shieldAnimator.GetCurrentAnimatorStateInfo(0).length
        );

        shieldVisual.enabled = false;
        shieldRoutine = null;
    }
}
