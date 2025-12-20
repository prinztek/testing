using UnityEngine;

public class AncientBossAnimationEventRelay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private AncientBoss ancientBoss;
    public void ExecuteCrystalBarrage()
    {
        ancientBoss?.TriggerSpikeBarrage();
    }
}
