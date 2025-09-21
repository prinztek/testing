using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingItemButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;


    private CraftableItem item;

    public void Setup(CraftableItem craftable)
    {
        item = craftable;

        if (item != null && item.itemData != null)
        {
            iconImage.sprite = item.itemData.icon;

        }
    }

    public void SetOnClick(System.Action callback)
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => callback?.Invoke());
    }

}
