using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator fireAnimator;
    [SerializeField] private GameObject fireVisual;
    [SerializeField] private GameObject fireLight2d;

    private Coroutine fireRoutine;

    private void Awake()
    {
        // Fire starts disabled
        fireVisual.SetActive(false);
    }

    private void Start()
    {
        // Optionally enable fire on start
        // EnableFire();
    }

    public void EnableFire()
    {
        if (fireRoutine != null)
            StopCoroutine(fireRoutine);

        fireVisual.SetActive(true);
        fireLight2d.SetActive(true);
        fireRoutine = StartCoroutine(FireStartRoutine());
    }

    public void DisableFire()
    {
        if (fireRoutine != null)
            StopCoroutine(fireRoutine);

        fireLight2d.SetActive(false);
        fireRoutine = StartCoroutine(FireEndRoutine());
    }

    private IEnumerator FireStartRoutine()
    {
        fireAnimator.Play("start");

        // Wait for start animation to finish
        yield return new WaitForSeconds(
            fireAnimator.GetCurrentAnimatorStateInfo(0).length
        );

        fireAnimator.Play("loop");
    }

    private IEnumerator FireEndRoutine()
    {
        fireAnimator.Play("end");

        // Wait for end animation to finish
        yield return new WaitForSeconds(
            fireAnimator.GetCurrentAnimatorStateInfo(0).length
        );

        fireVisual.SetActive(false);
        fireRoutine = null;
    }
}
