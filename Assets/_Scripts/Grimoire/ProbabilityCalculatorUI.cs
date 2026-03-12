using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProbabilityCalculatorUI : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public CharacterStats characterStats;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField inputFavorable;
    [SerializeField] private TMP_InputField inputTotal;

    [Header("Selector Visuals")]
    [SerializeField] private Image selectorFavorable;
    [SerializeField] private Image selectorTotal;

    [Header("Canvas Group")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Buttons")]
    [SerializeField] private Button[] buttons;

    [Header("Result")]
    [SerializeField] private TMP_Text resultText;

    [Header("Cost")]
    [SerializeField] private int calculationCost = 20;

    private bool wasLocked = true;

    private enum SelectedInput { None, Favorable, Total }
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

        bool hasSkill = characterStats.HasSkill(SkillType.ProbabilityEngine);

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
    // INPUT SELECTION
    // ==========================

    public void SelectFavorable()
    {
        currentSelectedInput = SelectedInput.Favorable;
        UpdateSelectionVisuals();
    }

    public void SelectTotal()
    {
        currentSelectedInput = SelectedInput.Total;
        UpdateSelectionVisuals();
    }

    private void UpdateSelectionVisuals()
    {
        if (selectorFavorable != null)
            selectorFavorable.gameObject.SetActive(currentSelectedInput == SelectedInput.Favorable);

        if (selectorTotal != null)
            selectorTotal.gameObject.SetActive(currentSelectedInput == SelectedInput.Total);
    }

    // ==========================
    // KEYPAD INPUT
    // ==========================

    public void PressKey(string key)
    {
        TMP_InputField target =
            currentSelectedInput == SelectedInput.Favorable
            ? inputFavorable
            : inputTotal;

        if (target == null || !target.interactable)
            return;

        target.text += key;

        if (!int.TryParse(target.text, out int value))
        {
            target.text = "";
            return;
        }

        if (currentSelectedInput == SelectedInput.Favorable &&
            int.TryParse(inputTotal.text, out int total) && value > total)
        {
            target.text = total.ToString();
        }
    }

    public void ClearSelectedInput()
    {
        TMP_InputField target =
            currentSelectedInput == SelectedInput.Favorable
            ? inputFavorable
            : inputTotal;

        if (target != null)
            target.text = "";
    }

    public void ClearAll()
    {
        inputFavorable.text = "";
        inputTotal.text = "";
        resultText.text = "";
        currentSelectedInput = SelectedInput.None;

        UpdateSelectionVisuals();
    }

    // ==========================
    // CALCULATE
    // ==========================

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
            resultText.text = "Invalid input!";
            return;
        }

        if (total <= 0 || favorable < 0 || favorable > total)
        {
            resultText.text = "Invalid values!";
            return;
        }

        float probability = (float)favorable / total;

        playerInventory.DeductGold(calculationCost);

        resultText.text =
            $"Result: {probability:0.###} ({probability * 100f:0.##}%)";
    }

    // ==========================
    // UI LOCK
    // ==========================

    private void UpdateUIAccess()
    {
        bool hasSkill =
            characterStats != null &&
            characterStats.HasSkill(SkillType.ProbabilityEngine);

        SetLockedState(!hasSkill);

        resultText.text = hasSkill
            ? ""
            : "Requires Probability Engine skill";
    }

    private void SetLockedState(bool locked)
    {
        if (canvasGroup == null) return;

        canvasGroup.interactable = !locked;
        canvasGroup.blocksRaycasts = !locked;
        canvasGroup.alpha = locked ? 0.6f : 1f;
    }
}