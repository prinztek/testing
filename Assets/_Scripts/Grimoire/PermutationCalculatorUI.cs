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

    [Header("Buttons")]
    public Button[] buttons;

    [Header("Result")]
    public TMP_Text resultText;

    [Header("Cost")]
    public int calculationCost = 20;

    private const int MAX_N = 15;
    private bool wasLocked = true;
    [Header("Selection")]
    [SerializeField] private Image selectorN;
    [SerializeField] private Image selectorR;

    private enum SelectedInput
    {
        None,
        N,
        R
    }

    private SelectedInput currentSelectedInput = SelectedInput.None;

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

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        playerInventory = playerObj.GetComponent<PlayerInventory>();
        characterStats = playerObj.GetComponent<CharacterStats>();
        UpdateUIAccess();
    }

    private void Awake()
    {
        LockUI();
    }

    private void Update()
    {
        if (characterStats == null)
            return;

        bool hasSkill = characterStats.HasSkill(SkillType.PermutationEngine);

        if (hasSkill && wasLocked)
        {
            UpdateUIAccess();
            wasLocked = false;
        }
    }

    // ==========================
    //        INPUT
    // ==========================
    public void PressKey(string key)
    {
        if (currentSelectedInput == SelectedInput.None)
            return;

        TMP_InputField target = GetActiveInput();
        if (target == null || !target.interactable)
            return;

        target.text += key;

        // Clamp values immediately
        if (int.TryParse(target.text, out int value))
        {
            if (currentSelectedInput == SelectedInput.N && value > MAX_N)
                target.text = MAX_N.ToString();

            if (currentSelectedInput == SelectedInput.R)
            {
                if (int.TryParse(inputN.text, out int n) && value > n)
                    target.text = n.ToString();
            }
        }
        else
        {
            target.text = "";
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

    public void ClearInputs()
    {
        inputN.text = "";
        inputR.text = "";
        resultText.text = "";
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
    //      CALCULATE
    // ==========================
    public void CalculatePermutation()
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

        long result = Factorial(n) / Factorial(n - r);

        playerInventory.DeductGold(calculationCost);
        resultText.text = $"Result: {result}";
    }

    // ==========================
    //      FACTORIAL
    // ==========================
    private long Factorial(int value)
    {
        long result = 1;
        for (int i = 1; i <= value; i++)
            result *= i;
        return result;
    }

    // ==========================
    //        UI LOCK
    // ==========================
    private void LockUI()
    {
        if (inputN != null) inputN.interactable = false;
        if (inputR != null) inputR.interactable = false;

        foreach (Button button in buttons)
            button.interactable = false;

        resultText.text = "Requires Permutation Engine skill";
    }

    private void UpdateUIAccess()
    {
        bool hasSkill = characterStats != null &&
                        characterStats.HasSkill(SkillType.PermutationEngine);

        if (inputN != null) inputN.interactable = hasSkill;
        if (inputR != null) inputR.interactable = hasSkill;

        foreach (Button button in buttons)
            button.interactable = hasSkill;

        resultText.text = hasSkill ? "" : "Requires Permutation Engine skill";
    }
}
