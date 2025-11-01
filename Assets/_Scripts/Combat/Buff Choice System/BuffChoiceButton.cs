using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffChoiceButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private Button button;

    public void Setup(BuffOption option, System.Action onClick)
    {
        titleText.text = option.name;
        descText.text = option.description;
        icon.sprite = option.icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick());
    }
}
