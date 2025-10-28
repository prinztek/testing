using UnityEngine;
using DG.Tweening;

public class PickupItem : MonoBehaviour
{
    public GameItem itemData;

    [Header("Pickup Settings")]
    public bool autoPickup = true;
    public float flySpeed = 7f;        // Makes the item move faster
    public float magnetDelay = 2f;  // Optional: feels snappier

    [Header("Explosion Settings")]
    public float explodeForce = 1f;

    private Transform player;
    private bool canBeMagnetized = false;
    private bool isFlying = false;
    public RuntimeAnimatorController animationController;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        AssignIcon();

        // Do explosion (initial punch movement)
        Vector2 punch = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 3f)).normalized * explodeForce;
        transform.DOMove(transform.position + (Vector3)punch, 0.25f).SetEase(Ease.OutQuad);

        // Start magnet after delay
        Invoke(nameof(StartAttract), magnetDelay);
    }

    private void Update()
    {
        if (autoPickup && canBeMagnetized && player != null && !isFlying)
        {
            FlyToPlayer();
        }
    }

    private void StartAttract()
    {
        canBeMagnetized = true;
    }

    private void FlyToPlayer()
    {
        isFlying = true;
        transform
            .DOMove(player.position, 0.3f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(GiveItemToPlayer);
    }

    private void AssignIcon()
    {
        if (itemData == null) return;

        Transform iconChild = transform.Find("PickupItemIcon");
        if (iconChild != null)
        {
            SpriteRenderer iconRenderer = iconChild.GetComponent<SpriteRenderer>();
            if (iconRenderer != null)
            {
                iconRenderer.sprite = itemData.icon;
            }

            Animator animator = iconChild.GetComponent<Animator>();
            if (animator != null)
            {
                string controllerPath = $"Animators/{itemData.itemType.ToString().ToLower()}";
                RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(controllerPath);

                if (controller != null)
                {
                    animator.runtimeAnimatorController = controller;
                }
                else
                {
                    Debug.LogWarning($"Animator Controller not found at Resources/{controllerPath}.controller");
                }
            }

        }
    }

    private void GiveItemToPlayer()
    {
        if (itemData != null && player != null)
        {
            if (itemData.itemType == ItemType.Gold)
            {
                PlayerInventory playerInventory = player.GetComponent<PlayerInventory>();
                if (playerInventory != null)
                {
                    playerInventory.AddGold(itemData.goldAmount);
                    Debug.Log($"Picked up {itemData.goldAmount} gold.");
                }
            }

            if (itemData.itemType == ItemType.Consumable)
            {
                PlayerInventory playerInventory = player.GetComponent<PlayerInventory>();
                if (playerInventory != null)
                {
                    playerInventory.AddItem(itemData);
                    Debug.Log($"Picked up {itemData.itemName}.");
                }
            }

            // Future: handle other item types here
        }

        Destroy(gameObject);
    }
}
