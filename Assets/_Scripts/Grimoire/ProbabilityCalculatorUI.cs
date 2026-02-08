using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProbabilityCalculatorUI : MonoBehaviour
{
    public enum InputState
    {
        Favorable,
        Total
    }

    [Header("Player References")]
    public PlayerInventory playerInventory;
    public CharacterStats characterStats;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField inputFavorable;
    [SerializeField] private TMP_InputField inputTotal;

    [Header("Selection Indicators")]
    [SerializeField] private GameObject favorableSelector;
    [SerializeField] private GameObject totalSelector;

    [Header("Buttons")]
    [SerializeField] private Button[] buttons;

    [Header("Result")]
    [SerializeField] private TMP_Text resultText;

    public int calculationCost = 20;

    private InputState currentInputState = InputState.Favorable;
    private bool wasLocked = true;

    // =============================
    // UNITY LIFECYCLE
    // =============================

    private void Awake()
    {
        // Start LOCKED
        inputFavorable.readOnly = true;
        inputTotal.readOnly = true;

        inputFavorable.interactable = false;
        inputTotal.interactable = false;

        if (buttons != null)
        {
            foreach (Button b in buttons)
            {
                if (b != null)
                    b.interactable = false;
            }
        }

        favorableSelector.SetActive(false);
        totalSelector.SetActive(false);

        resultText.text = "Requires Probability Engine skill";
    }

    private void OnEnable()
    {
        GameManager.OnPlayerSpawned += HandlePlayerSpawned;

        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            HandlePlayerSpawned(GameManager.Instance.CurrentPlayer);
        }
    }

    private void OnDisable()
    {
        GameManager.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void Update()
    {
        if (characterStats == null)
            return;

        bool hasSkill = characterStats.HasSkill(SkillType.ProbabilityEngine);

        if (hasSkill && wasLocked)
        {
            UpdateUIAccess();
            wasLocked = false;
        }
    }

    // =============================
    // PLAYER HOOKUP
    // =============================

    private void HandlePlayerSpawned(GameObject playerObj)
    {
        playerInventory = playerObj.GetComponent<PlayerInventory>();
        characterStats = playerObj.GetComponent<CharacterStats>();
        UpdateUIAccess();
    }

    // =============================
    // UI ACCESS CONTROL
    // =============================

    private void UpdateUIAccess()
    {
        bool hasSkill = characterStats != null &&
                        characterStats.HasSkill(SkillType.ProbabilityEngine);

        inputFavorable.interactable = hasSkill;
        inputTotal.interactable = hasSkill;

        if (buttons != null)
        {
            foreach (Button b in buttons)
            {
                if (b != null)
                    b.interactable = hasSkill;
            }
        }

        if (hasSkill)
        {
            inputFavorable.text = "";
            inputTotal.text = "";
            resultText.text = "";
            SelectFavorable();
        }
        else
        {
            favorableSelector.SetActive(false);
            totalSelector.SetActive(false);
            resultText.text = "Requires Probability Engine skill";
        }
    }

    // =============================
    // INPUT SELECTION
    // =============================

    public void SelectFavorable()
    {
        if (!inputFavorable.interactable) return;

        currentInputState = InputState.Favorable;
        favorableSelector.SetActive(true);
        totalSelector.SetActive(false);
    }

    public void SelectTotal()
    {
        if (!inputTotal.interactable) return;

        currentInputState = InputState.Total;
        favorableSelector.SetActive(false);
        totalSelector.SetActive(true);
    }

    // =============================
    // KEYPAD INPUT
    // =============================

    public void PressKey(string key)
    {
        TMP_InputField target =
            currentInputState == InputState.Favorable ? inputFavorable : inputTotal;

        if (!target.interactable)
            return;

        if (target.text.Length >= 3)
            return;

        target.text += key;
    }

    public void Backspace()
    {
        TMP_InputField target =
            currentInputState == InputState.Favorable ? inputFavorable : inputTotal;

        if (!target.interactable || target.text.Length == 0)
            return;

        target.text = target.text[..^1];
    }

    public void ClearAll()
    {
        if (!inputFavorable.interactable)
            return;

        inputFavorable.text = "";
        inputTotal.text = "";
        resultText.text = "";
    }

    // =============================
    // CALCULATION
    // =============================

    public void CalculateProbability()
    {
        if (playerInventory.Gold < calculationCost)
        {
            resultText.text = "Not enough gold!";
            return;
        }

        if (!int.TryParse(inputFavorable.text, out int favorable) ||
            !int.TryParse(inputTotal.text, out int total))
        {
            resultText.text = "Invalid Input";
            return;
        }

        if (total <= 0 || favorable < 0)
        {
            resultText.text = "Invalid Values";
            return;
        }

        if (favorable > total)
        {
            resultText.text = "Favorable > Total";
            return;
        }

        float probability = (float)favorable / total;
        playerInventory.DeductGold(calculationCost);

        resultText.text =
            $"{probability:0.###}  ({probability * 100f:0.##}%)";
    }
}
