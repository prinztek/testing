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

    [Header("Audio Clips")]
    [SerializeField] private AudioClip onClickSoundClip;

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

        // Play click sound
        if (onClickSoundClip != null)
        {
            SoundFXManager.Instance.playOneShotSoundFXClilp(onClickSoundClip, transform, 0.3f);
        }
    }

    void UpdateUI()
    {
        if (valueText != null)
            valueText.text = currentValue.ToString();
    }
}
