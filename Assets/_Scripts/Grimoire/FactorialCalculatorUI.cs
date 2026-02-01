using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactorialCalculatorUI : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public CharacterStats characterStats;

    [Header("Calculate Button")]
    public Button[] buttons;
    [Header("Input Field")]
    public TMP_InputField input;
    [Header("Result")]
    public TMP_Text resultText;

    public int calculationCost = 20; // Gold cost for calculation
    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
        // If player already exists when UI enables, connect immediately
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            HandlePlayerSpawned(GameManager.Instance.CurrentPlayer);
        }
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        playerInventory = playerObj.GetComponent<PlayerInventory>();
        characterStats = playerObj.GetComponent<CharacterStats>();
    }

    private void Awake()
    {
        // Make sure UI starts locked

        if (input != null)
        {
            input.interactable = false;
        }

        if (buttons != null)
        {
            foreach (Button button in buttons)
            {
                if (button != null)
                    button.interactable = false;
            }
        }

        if (resultText != null)
            resultText.text = "Requires Factorial Engine skill";
    }

    private bool wasLocked = true;

    private void Update()
    {
        if (characterStats == null)
            return;

        bool hasSkill = characterStats.HasSkill(SkillType.FactorialEngine);

        if (hasSkill && wasLocked)
        {
            Debug.Log("HASSKILLLLLLLLLLLLLLLLLLLL & WASLLLLLLLLLLLL:" + hasSkill);

            UpdateUIAccess();
            wasLocked = false;
        }
    }

    public void PressKey(string key)
    {
        if (input == null)
            return;

        if (input.text.Length < 2)
        {
            input.text += key;
        }

        Debug.Log("Input limit reached!");
    }

    public void ClearInput()
    {
        if (input == null)
            return;

        input.text = "";
    }

    public void CalculateFactorial()
    {
        if (input == null || resultText == null)
            return;
        if (playerInventory.Gold < calculationCost)
        {
            resultText.text = "Not enough gold!";
            return;
        }

        if (int.TryParse(input.text, out int number) && number >= 0)
        {
            long factorial = 1;
            for (int i = 1; i <= number; i++)
            {
                factorial *= i;
            }
            resultText.text = $"Result: {factorial}";
            playerInventory.DeductGold(calculationCost);
        }
        else
        {
            resultText.text = "Invalid Input";
        }
    }

    private void UpdateUIAccess()
    {
        bool hasSkill = characterStats != null &&
                        characterStats.HasSkill(SkillType.FactorialEngine);

        Debug.Log("INSIDE UPDATEUIACCESS:" + hasSkill);

        // Manually enable/disable the input and button
        if (input != null)
            input.interactable = hasSkill;

        if (buttons != null)
        {
            foreach (Button button in buttons)
            {
                if (button != null)
                    button.interactable = hasSkill;
            }
        }

        // Update the result text if the skill is missing
        if (resultText != null)
            resultText.text = hasSkill ? "" : "Requires Factorial Engine skill";
    }


}
