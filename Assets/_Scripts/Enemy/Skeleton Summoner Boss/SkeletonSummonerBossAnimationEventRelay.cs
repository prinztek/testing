using UnityEngine;

public class SkeletonSummonerBossAnimationEventRelay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private SkeletonSummonerBoss skeletonSummonerBoss;

    void Awake()
    {
        skeletonSummonerBoss = GetComponentInParent<SkeletonSummonerBoss>();
    }
    public void EnableVFX()
    {
        skeletonSummonerBoss?.ShowMeleeAttackVFX();
    }

    public void DisableVFX()
    {
        skeletonSummonerBoss?.HideMeleeAttackVFX();
    }

}
