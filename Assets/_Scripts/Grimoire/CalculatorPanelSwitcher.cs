using UnityEngine;
using UnityEngine.UI;

public class CalculatorPanelSwitcher : MonoBehaviour
{
    [Header("Calculator Panels (Order Matters)")]
    [SerializeField] private GameObject[] calculatorPanels;

    [Header("Navigation Buttons")]
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    private int currentIndex = 0;

    private void Start()
    {
        ShowPanel(0); // Factorial is default
        
        prevButton.onClick.AddListener(ShowPrevious);
        nextButton.onClick.AddListener(ShowNext);
    }

    private void ShowPanel(int index)
    {
        for (int i = 0; i < calculatorPanels.Length; i++)
        {
            calculatorPanels[i].SetActive(i == index);
        }

        currentIndex = index;

        // Optional: disable buttons at ends
        prevButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < calculatorPanels.Length - 1;
    }

    public void ShowNext()
    {
        if (currentIndex < calculatorPanels.Length - 1)
        {
            ShowPanel(currentIndex + 1);
        }
    }

    public void ShowPrevious()
    {
        if (currentIndex > 0)
        {
            ShowPanel(currentIndex - 1);
        }
    }
}
