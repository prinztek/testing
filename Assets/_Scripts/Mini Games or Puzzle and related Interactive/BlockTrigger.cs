using UnityEngine;

public class BlockTrigger : MonoBehaviour
{
    public enum TriggerType { Top, Bottom }
    public TriggerType triggerType;

    private MarioNumberBlock block;

    void Start()
    {
        // Find the parent block script automatically
        block = GetComponentInParent<MarioNumberBlock>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (triggerType == TriggerType.Top)
            block.HitTop();
        else if (triggerType == TriggerType.Bottom)
            block.HitBottom();
    }
}
