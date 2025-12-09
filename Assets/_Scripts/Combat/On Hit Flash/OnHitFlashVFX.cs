using System.Collections;
using UnityEngine;

public class OnHitFlashVFX : MonoBehaviour
{
    private Material originalMaterial;
    private SpriteRenderer sr;

    [SerializeField] private Material onDamageVfxMat;
    [SerializeField] private float flashDuration = 0.15f;
    private Coroutine onDamageVfxCoroutine;

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

    // Update is called once per frame
    private IEnumerator OnDamageVfxCoroutine()
    {
        sr.material = onDamageVfxMat;
        yield return new WaitForSeconds(flashDuration);
        sr.material = originalMaterial;
    }
}
