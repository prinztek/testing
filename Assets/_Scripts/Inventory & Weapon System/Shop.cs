using UnityEngine;

public class Shop : MonoBehaviour
{
    public Inventory playerInventory;

    public void BuyItem(GameItem item)
    {
        // Add gold check here later
        playerInventory.AddItem(item);
    }
}

