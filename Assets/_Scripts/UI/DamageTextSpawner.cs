using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance { get; private set; }
    public GameObject damageTextPrefab;
    public Canvas worldCanvas; // Assign in inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ShowDamage(Vector3 worldPosition, int amount, Color color)
    {
        GameObject popup = Instantiate(damageTextPrefab, worldPosition, Quaternion.identity, worldCanvas.transform);
        popup.transform.localScale = Vector3.one;
        DamageTextPopup damageText = popup.GetComponent<DamageTextPopup>();
        damageText.SetText(amount.ToString(), color);
    }

    // this should go in to bootsrapper systems prefab
}
