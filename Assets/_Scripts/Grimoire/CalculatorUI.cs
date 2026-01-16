using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CalculatorUI : MonoBehaviour
{
    public PlayerInventory playerInventory;


    [Header("Input Fields")]
    public TMP_InputField inputA;
    public TMP_InputField inputB;

    [Header("Result")]
    public TMP_Text resultText;
    // public TMP_Text selectedOperationText;
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
    }
    private TMP_InputField activeInput;

    private enum Operation { None, Add, Sub, Mul, Div }
    private Operation currentOperation = Operation.None;


    // Called when Input A is selected
    public void SelectInputA()
    {
        activeInput = inputA;
    }

    // Called when Input B is selected
    public void SelectInputB()
    {
        activeInput = inputB;
    }


    // ==========================
    //      KEYPAD INPUT
    // ==========================
    public void PressKey(string key)
    {
        if (activeInput == null)
            return;

        activeInput.text += key;
    }

    public void ClearInput()
    {
        if (activeInput == null)
            return;

        activeInput.text = "";
    }

    public void Backspace()
    {
        if (activeInput == null)
            return;

        if (activeInput.text.Length > 0)
            activeInput.text = activeInput.text[..^1];
    }


    // ==========================
    //   OPERATION SELECTION
    // ==========================
    public void SelectAdd() { SetOperation(Operation.Add, "+"); }
    public void SelectSub() { SetOperation(Operation.Sub, "-"); }
    public void SelectMul() { SetOperation(Operation.Mul, "x"); }
    public void SelectDiv() { SetOperation(Operation.Div, "÷"); }

    private void SetOperation(Operation op, string symbol)
    {
        currentOperation = op;
        // selectedOperationText.text = "Operation: " + symbol;
    }


    // ==========================
    //       CALCULATE
    // ==========================
    public void Calculate()
    {
        if (currentOperation == Operation.None)
        {
            resultText.text = "Select an operation!";
            return;
        }

        if (playerInventory.Gold < calculationCost)
        {
            resultText.text = "Not enough gold!";
            return;
        }

        float a, b;

        if (!float.TryParse(inputA.text, out a) ||
            !float.TryParse(inputB.text, out b))
        {
            resultText.text = "Invalid numbers!";
            return;
        }

        float result = 0f;

        switch (currentOperation)
        {
            case Operation.Add: result = a + b; break;
            case Operation.Sub: result = a - b; break;
            case Operation.Mul: result = a * b; break;
            case Operation.Div:
                if (b == 0)
                {
                    resultText.text = "Cannot divide by 0!";
                    return;
                }
                result = a / b;
                break;
        }
        playerInventory.DeductGold(calculationCost);

        resultText.text = "Result: " + result;
    }
}
