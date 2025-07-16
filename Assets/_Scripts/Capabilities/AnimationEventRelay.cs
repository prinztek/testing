using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    [SerializeField] private Attack attack;
    [SerializeField] private ProjectileLauncher projectileLauncher;

    public void EnableCombo()
    {
        attack?.EnableCombo();
    }

    public void FireProjectile()
    {
        projectileLauncher?.FireProjectile();
    }

}
