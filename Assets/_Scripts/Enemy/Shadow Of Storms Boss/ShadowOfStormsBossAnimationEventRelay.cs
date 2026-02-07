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
}
