using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombinationCalculatorUI : MonoBehaviour
{
    [Header("Confirmation Dialog")]
    [SerializeField] private ConfirmationUI confirmationUI;
    public PlayerInventory playerInventory;
    public CharacterStats characterStats;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField inputN;
    [SerializeField] private TMP_InputField inputR;

    [Header("Selector Visuals")]
    [SerializeField] private Image selectorN;
    [SerializeField] private Image selectorR;

    [Header("Canvas Group")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Buttons")]
    [SerializeField] private Button[] buttons;

    [Header("Result")]
    [SerializeField] private TMP_Text resultText;

    [Header("Cost")]
    [SerializeField] private int calculationCost = 20;

    private const int MAX_N = 15; // extended limit
    private bool wasLocked = true;

    private enum SelectedInput { None, N, R }
    private SelectedInput currentSelectedInput = SelectedInput.None;

    // ==========================
    // LIFECYCLE
    // ==========================
    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;
        if (GameManager.Instance?.CurrentPlayer != null)
            HandlePlayerSpawned(GameManager.Instance.CurrentPlayer);
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void Awake()
    {
        SetLockedState(true);
        UpdateSelectionVisuals();
    }

    private void Update()
    {
        if (characterStats == null) return;

        bool hasSkill = characterStats.HasSkill(SkillType.CombinationEngine);
        if (hasSkill && wasLocked)
        {
            UpdateUIAccess();
            wasLocked = false;
        }
    }

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        playerInventory = playerObj.GetComponent<PlayerInventory>();
        characterStats = playerObj.GetComponent<CharacterStats>();
        UpdateUIAccess();
    }

    // ==========================
    // INPUT SELECTION & KEYPAD
    // ==========================
    public void SelectInputN()
    {
        currentSelectedInput = SelectedInput.N;
        UpdateSelectionVisuals();
    }

    public void SelectInputR()
    {
        currentSelectedInput = SelectedInput.R;
        UpdateSelectionVisuals();
    }

    private void UpdateSelectionVisuals()
    {
        if (selectorN != null) selectorN.gameObject.SetActive(currentSelectedInput == SelectedInput.N);
        if (selectorR != null) selectorR.gameObject.SetActive(currentSelectedInput == SelectedInput.R);
    }

    public void PressKey(string key)
    {
        TMP_InputField target = currentSelectedInput == SelectedInput.N ? inputN : inputR;
        if (target == null || !target.interactable) return;

        target.text += key;

        if (!int.TryParse(target.text, out int value))
        {
            target.text = "";
            return;
        }

        if (currentSelectedInput == SelectedInput.N && value > MAX_N) target.text = MAX_N.ToString();
        if (currentSelectedInput == SelectedInput.R &&
            int.TryParse(inputN.text, out int n) && value > n) target.text = n.ToString();
    }

    public void ClearSelectedInput()
    {
        TMP_InputField target = currentSelectedInput == SelectedInput.N ? inputN : inputR;
        if (target != null) target.text = "";
    }

    public void ClearAll()
    {
        inputN.text = "";
        inputR.text = "";
        resultText.text = "";
        currentSelectedInput = SelectedInput.None;
        UpdateSelectionVisuals();
    }

    // ==========================
    // CALCULATE
    // ==========================
    public void CalculateCombination()
    {
        if (playerInventory.Gold < calculationCost)
        {
            resultText.text = "Not enough gold!";
            return;
        }

        if (!int.TryParse(inputN.text, out int n) || !int.TryParse(inputR.text, out int r))
        {
            resultText.text = "Invalid input!";
            return;
        }

        if (n < 0 || r < 0 || n > MAX_N || r > n)
        {
            resultText.text = $"Invalid range! n: 0–{MAX_N}, r ≤ n";
            return;
        }

        confirmationUI.Show(
            title: "Confirm Calculation",
            description: $"Are you sure you want to calculate C({n}, {r})?",
            cost: $"Cost: {calculationCost} Gold",
            currentGold: $"You have: {playerInventory.Gold} Gold",
            confirmAction: () =>
            {
                long result = MathTables.Factorial(n) / (MathTables.Factorial(r) * MathTables.Factorial(n - r));
                playerInventory.DeductGold(calculationCost);
                resultText.text = $"Result: {result}";
            }
        );

        // long result = MathTables.Factorial(n) / (MathTables.Factorial(r) * MathTables.Factorial(n - r));
        // playerInventory.DeductGold(calculationCost);
        // resultText.text = $"Result: {result}";
    }

    // ==========================
    // UI LOCK
    // ==========================
    private void UpdateUIAccess()
    {
        bool hasSkill = characterStats != null && characterStats.HasSkill(SkillType.CombinationEngine);
        SetLockedState(!hasSkill);
        resultText.text = hasSkill ? "" : "Requires Combination Engine skill";
    }

    private void SetLockedState(bool locked)
    {
        if (canvasGroup == null) return;
        canvasGroup.interactable = !locked;
        canvasGroup.blocksRaycasts = !locked;
        canvasGroup.alpha = locked ? 0.6f : 1f;
    }
}
