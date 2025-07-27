using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance;
    public GameObject damageTextPrefab;
    public Canvas worldCanvas; // Assign in inspector

    void Awake()
    {
        Instance = this;

        // if (worldCanvas == null)
        //     Debug.LogError("[DamageTextSpawner] worldCanvas is NOT assigned!");
        // else
        //     Debug.Log("[DamageTextSpawner] worldCanvas assigned: " + worldCanvas.name);
    }


    public void ShowDamage(Vector3 worldPosition, int amount, Color color)
    {
        GameObject popup = Instantiate(damageTextPrefab, worldPosition, Quaternion.identity, worldCanvas.transform);
        popup.transform.localScale = Vector3.one;
        DamageTextPopup damageText = popup.GetComponent<DamageTextPopup>();
        damageText.SetText(amount.ToString(), color);
    }


}
