using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EnterNamePanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action<string> onNameConfirmed;

    private void Awake()
    {
        // Automatically wire the buttons
        if (confirmButton != null)
            confirmButton.onClick.AddListener(Confirm);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(Cancel);
    }

    private void Start()
    {
        nameInput.shouldHideMobileInput = true;
    }

    public void Open(Action<string> callback)
    {
        onNameConfirmed = callback;
        nameInput.text = ""; // optionally clear previous input
        gameObject.SetActive(true);
        nameInput.Select();  // optional: focus the input field
        nameInput.ActivateInputField();
    }

    private void Confirm()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
            return;

        onNameConfirmed?.Invoke(nameInput.text);
        Close();
    }

    private void Cancel()
    {
        Close();
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
