using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ConfirmationUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI currentGoldText;

    public Button confirmButton;
    public Button cancelButton;

    private Action onConfirm;

    public void Show(string title, string description, string cost, string currentGold, Action confirmAction)
    {
        gameObject.SetActive(true);

        titleText.text = title;
        descriptionText.text = description;
        costText.text = cost;
        currentGoldText.text = currentGold;

        onConfirm = confirmAction;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(Confirm);

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(Hide);
    }

    void Confirm()
    {
        onConfirm?.Invoke();
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}