using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Totem : MonoBehaviour
{
    [Header("UI")]
    public Button button;
    public TMP_Text valueText;

    [Header("State")]
    public int currentValue;
    private int maxValue;

    public void Initialize(int maxValue)
    {
        this.maxValue = maxValue;
        currentValue = 0;

        UpdateUI();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        currentValue++;

        if (currentValue > maxValue)
            currentValue = 0;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (valueText != null)
            valueText.text = currentValue.ToString();
    }
}
