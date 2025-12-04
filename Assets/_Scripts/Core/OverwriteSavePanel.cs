using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class OverwriteSavePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private Action onYes;
    private Action onNo;

    public void Show(string message, Action yesCallback, Action noCallback = null)
    {
        descriptionText.text = message;
        onYes = yesCallback;
        onNo = noCallback;

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => { onYes?.Invoke(); Close(); });

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => { onNo?.Invoke(); Close(); });

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
