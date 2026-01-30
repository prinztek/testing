using UnityEngine;

public class VFXAutoDestroy : MonoBehaviour
{
    public float lifetime = 0.5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
