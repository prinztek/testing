using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombinationCalculatorUI : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public CharacterStats characterStats;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField inputN;
    [SerializeField] private TMP_InputField inputR;

    [Header("Selector Visuals")]
    [SerializeField] private Image selectorN;
    [SerializeField] private Image selectorR;

    [Header("Buttons")]
    [SerializeField] private Button[] buttons;

    [Header("Result")]
    [SerializeField] private TMP_Text resultText;

    [Header("Cost")]
    [SerializeField] private int calculationCost = 20;

    private const int MAX_N = 15;
    private bool wasLocked = true;

    // ==========================
    //      INPUT SELECTION
    // ==========================
    private enum SelectedInput
    {
        None,
        N,
        R
    }

    private SelectedInput currentSelectedInput = SelectedInput.None;

    // ==========================
    //      LIFECYCLE
    // ==========================
    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;

        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
            HandlePlayerSpawned(GameManager.Instance.CurrentPlayer);
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void Awake()
    {
        LockUI();
        UpdateSelectionVisuals();
    }

    private void Update()
    {
        if (characterStats == null)
            return;

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
    //   INPUT FIELD SELECTION
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
        if (selectorN != null)
            selectorN.gameObject.SetActive(currentSelectedInput == SelectedInput.N);

        if (selectorR != null)
            selectorR.gameObject.SetActive(currentSelectedInput == SelectedInput.R);
    }

    // ==========================
    //        KEYPAD INPUT
    // ==========================
    public void PressKey(string key)
    {
        TMP_InputField target = GetActiveInput();
        if (target == null || !target.interactable)
            return;

        target.text += key;

        if (!int.TryParse(target.text, out int value))
        {
            target.text = "";
            return;
        }

        // Clamp N
        if (currentSelectedInput == SelectedInput.N && value > MAX_N)
            target.text = MAX_N.ToString();

        // Clamp R ≤ N
        if (currentSelectedInput == SelectedInput.R &&
            int.TryParse(inputN.text, out int n) &&
            value > n)
        {
            target.text = n.ToString();
        }
    }

    private TMP_InputField GetActiveInput()
    {
        return currentSelectedInput switch
        {
            SelectedInput.N => inputN,
            SelectedInput.R => inputR,
            _ => null
        };
    }

    public void ClearSelectedInput()
    {
        TMP_InputField target = GetActiveInput();
        if (target != null)
            target.text = "";
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
    //        CALCULATE
    // ==========================
    public void CalculateCombination()
    {
        if (playerInventory.Gold < calculationCost)
        {
            resultText.text = "Not enough gold!";
            return;
        }

        if (!int.TryParse(inputN.text, out int n) ||
            !int.TryParse(inputR.text, out int r))
        {
            resultText.text = "Invalid input!";
            return;
        }

        if (n < 0 || r < 0)
        {
            resultText.text = "n and r must be ≥ 0";
            return;
        }

        if (n > MAX_N)
        {
            resultText.text = $"n must be ≤ {MAX_N}";
            return;
        }

        if (r > n)
        {
            resultText.text = "r must be ≤ n";
            return;
        }

        long result =
            MathTables.Factorial(n) /
            (MathTables.Factorial(r) * MathTables.Factorial(n - r));

        playerInventory.DeductGold(calculationCost);
        resultText.text = $"Result: {result}";
    }

    // ==========================
    //        UI LOCK
    // ==========================
    private void LockUI()
    {
        if (inputN != null) inputN.interactable = false;
        if (inputR != null) inputR.interactable = false;

        foreach (Button button in buttons)
            if (button != null) button.interactable = false;

        resultText.text = "Requires Combination Engine skill";
    }

    private void UpdateUIAccess()
    {
        bool hasSkill = characterStats != null &&
                        characterStats.HasSkill(SkillType.CombinationEngine);

        if (inputN != null) inputN.interactable = hasSkill;
        if (inputR != null) inputR.interactable = hasSkill;

        foreach (Button button in buttons)
            if (button != null) button.interactable = hasSkill;

        resultText.text = hasSkill ? "" : "Requires Combination Engine skill";
    }
}
