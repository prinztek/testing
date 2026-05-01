using UnityEngine;

public class ShadowOfStormsBossAnimationEventRelay : MonoBehaviour
{
    [SerializeField] private ShadowOfStormsBoss shadowOfStormsBoss;

    void Awake()
    {
        shadowOfStormsBoss = GetComponentInParent<ShadowOfStormsBoss>();
    }
    public void ShowAttacksVFX()
    {
        // shadowOfStormsBoss?.ShowAttacksVFX();
    }

    public void ShowMeleeAttackVFX()
    {
        shadowOfStormsBoss?.ShowMeleeAttackVFX();
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

    public void TriggerScreenShake()
    {
        shadowOfStormsBoss?.TriggerScreenShake();
    }
}