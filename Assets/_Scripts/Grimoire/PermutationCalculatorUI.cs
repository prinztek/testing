using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PermutationCalculatorUI : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public CharacterStats characterStats;

    [Header("Input Fields")]
    public TMP_InputField inputN;
    public TMP_InputField inputR;

    [Header("Selector Visuals")]
    [SerializeField] private Image selectorN;
    [SerializeField] private Image selectorR;

    [Header("Canvas Group")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Buttons")]
    public Button[] buttons;

    [Header("Result")]
    public TMP_Text resultText;

    [Header("Cost")]
    public int calculationCost = 20;

    private const int MAX_N = 15; // Extended to 20
    private bool wasLocked = true;

    private enum SelectedInput { None, N, R }
    private SelectedInput currentSelectedInput = SelectedInput.None;

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

        bool hasSkill = characterStats.HasSkill(SkillType.PermutationEngine);
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
        if (currentSelectedInput == SelectedInput.None) return;
        TMP_InputField target = currentSelectedInput == SelectedInput.N ? inputN : inputR;
        if (target == null || !target.interactable) return;

        target.text += key;

        // Clamp values immediately
        if (int.TryParse(target.text, out int value))
        {
            if (currentSelectedInput == SelectedInput.N && value > MAX_N) target.text = MAX_N.ToString();
            if (currentSelectedInput == SelectedInput.R &&
                int.TryParse(inputN.text, out int n) && value > n) target.text = n.ToString();
        }
        else
        {
            target.text = "";
        }
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
    public void CalculatePermutation()
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

        long result = MathTables.Factorial(n) / MathTables.Factorial(n - r);
        playerInventory.DeductGold(calculationCost);
        resultText.text = $"Result: {result}";
    }

    // ==========================
    // UI LOCK
    // ==========================
    private void UpdateUIAccess()
    {
        bool hasSkill = characterStats != null && characterStats.HasSkill(SkillType.PermutationEngine);
        SetLockedState(!hasSkill);
        resultText.text = hasSkill ? "" : "Requires Permutation Engine skill";
    }

    private void SetLockedState(bool locked)
    {
        if (canvasGroup == null) return;
        canvasGroup.interactable = !locked;
        canvasGroup.blocksRaycasts = !locked;
        canvasGroup.alpha = locked ? 0.6f : 1f;
    }
}
