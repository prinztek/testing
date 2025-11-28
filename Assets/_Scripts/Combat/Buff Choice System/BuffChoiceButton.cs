using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffChoiceButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private Button button;

    private BuffOption myOption;
    private System.Action<BuffOption> onSelected;

    public void Setup(BuffOption option, System.Action<BuffOption> onSelected)
    {
        myOption = option;
        this.onSelected = onSelected;

        titleText.text = option.name;
        descText.text = option.description;
        icon.sprite = option.icon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => this.onSelected?.Invoke(myOption));
    }

}
