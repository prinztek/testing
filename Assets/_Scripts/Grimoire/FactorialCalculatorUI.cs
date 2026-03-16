using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class MathTables
{
    public static readonly long[] Factorials =
    {
        1L,                     // 0!
        1L,                     // 1!
        2L,                     // 2!
        6L,                     // 3!
        24L,                    // 4!
        120L,                   // 5!
        720L,                   // 6!
        5040L,                  // 7!
        40320L,                 // 8!
        362880L,                // 9!
        3628800L,               // 10!
        39916800L,              // 11!
        479001600L,             // 12!
        6227020800L,            // 13!
        87178291200L,           // 14!
        1307674368000L          // 15!
    };

    public static long Factorial(int n)
    {
        if (n < 0 || n >= Factorials.Length)
            return 0;

        return Factorials[n];
    }
}

public class FactorialCalculatorUI : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public CharacterStats characterStats;

    [Header("Input Field")]
    public TMP_InputField input;
    [Header("Result")]
    public TMP_Text resultText;
    [Header("Canvas Group")]
    public CanvasGroup canvasGroup;

    public int calculationCost = 20; // Gold cost for calculation
    private bool wasLocked = true;

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
        UpdateUIAccess();
    }

    private void Awake()
    {
        // Start locked
        SetLockedState(true);

        if (resultText != null)
            resultText.text = "Requires Factorial Engine skill";
    }

    private void Update()
    {
        if (characterStats == null)
            return;

        bool hasSkill = characterStats.HasSkill(SkillType.FactorialEngine);

        if (hasSkill && wasLocked)
        {
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
        else
        {
            Debug.Log("Input limit reached!");
        }
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
            long factorial = MathTables.Factorial(number);

            if (factorial == 0)
            {
                resultText.text = "Out of range (0–15)";
                return;
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

        SetLockedState(!hasSkill);

        if (resultText != null)
            resultText.text = hasSkill ? "" : "Requires Factorial Engine skill";
    }

    private void SetLockedState(bool locked)
    {
        if (canvasGroup == null)
            return;

        canvasGroup.interactable = !locked;
        canvasGroup.blocksRaycasts = !locked;
        canvasGroup.alpha = locked ? 0.6f : 1f; // dim when locked
    }
}
