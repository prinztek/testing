using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingItemButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject ownedTextIndicator;


    private CraftableItem item;
    private bool isAlreadyOwned;

    public void Setup(CraftableItem craftable, bool isAlreadyOwned)
    {
        item = craftable;
        this.isAlreadyOwned = isAlreadyOwned;

        if (item != null && item.itemData != null)
        {
            iconImage.sprite = item.itemData.icon;
        }

        ownedTextIndicator.SetActive(isAlreadyOwned);
    }

    public void SetOnClick(System.Action callback)
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => callback?.Invoke());
    }

}
