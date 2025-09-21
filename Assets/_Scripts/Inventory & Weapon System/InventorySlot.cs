[System.Serializable]
public class InventorySlot
{
    public GameItem item;
    public int quantity;

    public InventorySlot(GameItem item, int quantity = 1)
    {
        this.item = item;
        this.quantity = quantity;
    }
}
