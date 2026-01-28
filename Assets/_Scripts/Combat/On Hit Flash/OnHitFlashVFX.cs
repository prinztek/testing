using System.Collections;
using UnityEngine;

public class OnHitFlashVFX : MonoBehaviour
{
    private Material originalMaterial;
    private SpriteRenderer sr;

    [SerializeField] private Material onDamageVfxMat;
    [SerializeField] private Material onBurnVfxMat;
    [SerializeField] private float flashDuration = 0.15f;
    private Coroutine onDamageVfxCoroutine;
    private Coroutine onBurnVfxCoroutine;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
    }

    public void PlayOnDamageVfx()
    {
        if (onDamageVfxCoroutine != null)
        {
            StopCoroutine(onDamageVfxCoroutine);
        }
        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCoroutine());
    }

    public void PlayOnBurnVfx()
    {
        if (onBurnVfxCoroutine != null)
        {
            StopCoroutine(onBurnVfxCoroutine);
        }
        onBurnVfxCoroutine = StartCoroutine(OnBurnVfxCoroutine());
    }

    private IEnumerator OnDamageVfxCoroutine()
    {
        sr.material = onDamageVfxMat;
        yield return new WaitForSeconds(flashDuration);
        sr.material = originalMaterial;
    }

    private IEnumerator OnBurnVfxCoroutine()
    {
        sr.material = onBurnVfxMat;
        yield return new WaitForSeconds(flashDuration);
        sr.material = originalMaterial;
    }
}
