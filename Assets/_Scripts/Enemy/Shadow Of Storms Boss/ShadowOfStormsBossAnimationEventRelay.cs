using UnityEngine;

public class ShadowOfStormsBossAnimationEventRelay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private ShadowOfStormsBoss shadowOfStormsBoss;

    void Awake()
    {
        shadowOfStormsBoss = GetComponentInParent<ShadowOfStormsBoss>();
    }
    public void ShowAttacksVFX()
    {
        shadowOfStormsBoss?.ShowAttacksVFX();
    }

    public void ShowChargeVFX()
    {
        shadowOfStormsBoss?.ShowChargeVFX();
    }

    public void ShowChargeExplosionVFX()
    {
        shadowOfStormsBoss?.ShowChargeExplosionVFX();
    }

    public void ShowChargeBeamVFX()
    {
        shadowOfStormsBoss?.ShowChargeBeamVFX();
    }

    public void GenerateBeam()
    {
        shadowOfStormsBoss?.GenerateBeam();
    }

    public void OnChargeExplosionFinished()
    {
        shadowOfStormsBoss?.OnChargeExplosionFinished();
    }

    public void OnChargeBeamFinished()
    {
        shadowOfStormsBoss?.OnChargeBeamFinished();
    }

    public void OnComboFinished()
    {
        shadowOfStormsBoss?.OnComboFinished();
    }
}