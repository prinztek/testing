using UnityEngine;

public class ManualPickupItem : MonoBehaviour
{
    public GameItem itemData;

    [Header("Idle Float")]
    public float floatHeight = 0.15f;
    public float floatSpeed = 2f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPos + Vector3.up * yOffset;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (itemData == null) return;

        if (other.CompareTag("Player") == true)
        {
            Debug.Log("Player collided with pickup item: " + itemData.itemName);
        }

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory == null) return;

        switch (itemData.itemType)
        {
            case ItemType.Gold:
                inventory.AddGold(itemData.goldAmount);
                break;

            case ItemType.Consumable:
            case ItemType.KeyItem:
                inventory.AddItem(itemData);
                break;
        }

        Destroy(gameObject);
    }
}
