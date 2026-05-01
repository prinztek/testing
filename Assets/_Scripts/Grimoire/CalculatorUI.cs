using UnityEngine;
using TMPro;

public class CalculatorUI : MonoBehaviour
{
    public PlayerInventory playerInventory;
    [Header("Confirmation Dialog")]
    [SerializeField] private ConfirmationUI confirmationUI;

    [Header("Display")]
    [SerializeField] private TMP_InputField displayText;
    [SerializeField] private TMP_Text resultText;

    [Header("Cost")]
    [SerializeField] private int calculationCost = 20;

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

    // ==========================
    //        INPUT
    // ==========================
    public void PressKey(string key)
    {
        if (string.IsNullOrEmpty(key))
            return;

        string current = displayText.text;
        bool isOperator = IsOperator(key);

        // Prevent operator as first character
        if (current.Length == 0 && isOperator)
            return;

        // Prevent two operators in a row
        if (current.Length > 0 && isOperator && IsOperator(current[^1].ToString()))
            return;

        // Prevent operator after complete expression (e.g. "3+4" + → cannot add another operator)
        if (isOperator && ExpressionAlreadyComplete(current))
            return;

        // Prevent multiple decimals in the same number
        if (key == "." && HasDecimalInCurrentNumber(current))
            return;

        displayText.text += key;
    }

    public void Clear()
    {
        displayText.text = "";
        // resultText.text = "";
    }

    public void Backspace()
    {
        if (displayText.text.Length > 0)
            displayText.text = displayText.text[..^1];
    }

    // ==========================
    //      CALCULATE
    // ==========================
    public void Calculate()
    {
        if (playerInventory.Gold < calculationCost)
        {
            resultText.text = "Not enough gold!";
            return;
        }

        string expression = displayText.text.Replace(" ", "");

        // Expression cannot end with operator
        if (expression.Length == 0 || IsOperator(expression[^1].ToString()))
        {
            resultText.text = "Incomplete expression!";
            return;
        }

        if (!TryParseExpression(expression, out float a, out float b, out char op))
        {
            resultText.text = "Invalid expression!";
            return;
        }

        confirmationUI.Show(
            title: "Confirm Calculation",
            description: $"Are you sure you want to calculate {a} {op} {b} for {calculationCost} gold?",
            cost: $"{calculationCost} Gold",
            currentGold: $"You have: {playerInventory.Gold} Gold",
            confirmAction: () =>
            {
                PerformCalculation(a, b, op);
            }
        );

    }

    private void PerformCalculation(float a, float b, char op)
    {
        float result;

        switch (op)
        {
            case '+': result = a + b; break;
            case '-': result = a - b; break;
            case 'x':
            case '*': result = a * b; break;
            case '÷':
            case '/':
                if (b == 0)
                {
                    resultText.text = "Cannot divide by 0!";
                    return;
                }
                result = a / b;
                break;

            default:
                resultText.text = "Unknown operator!";
                return;
        }

        playerInventory.DeductGold(calculationCost);
        resultText.text = "Result: " + result;
    }

    // ==========================
    //   SIMPLE EXPRESSION PARSER
    // ==========================
    private bool TryParseExpression(string expr, out float a, out float b, out char op)
    {
        a = b = 0;
        op = '\0';

        char[] operators = { '+', '-', 'x', '*', '÷', '/' };

        int operatorCount = 0;
        int operatorIndex = -1;

        for (int i = 0; i < expr.Length; i++)
        {
            if (System.Array.Exists(operators, o => o == expr[i]))
            {
                operatorCount++;
                operatorIndex = i;
            }
        }

        // Must contain exactly ONE operator
        if (operatorCount != 1)
            return false;

        if (!float.TryParse(expr[..operatorIndex], out a))
            return false;

        if (!float.TryParse(expr[(operatorIndex + 1)..], out b))
            return false;

        op = expr[operatorIndex];
        return true;
    }


    // ==========================
    //      HELPER METHODS
    // ==========================

    private bool IsOperator(string c)
    {
        return c == "+" || c == "-" || c == "x" || c == "*" || c == "÷" || c == "/";
    }

    private bool HasDecimalInCurrentNumber(string text)
    {
        for (int i = text.Length - 1; i >= 0; i--)
        {
            if (IsOperator(text[i].ToString()))
                break;

            if (text[i] == '.')
                return true;
        }
        return false;
    }

    private bool ExpressionAlreadyComplete(string text)
    {
        char[] operators = { '+', '-', 'x', '*', '÷', '/' };

        int operatorIndex = -1;

        for (int i = 0; i < text.Length; i++)
        {
            if (System.Array.Exists(operators, o => o == text[i]))
            {
                operatorIndex = i;
                break;
            }
        }

        // No operator yet → not complete
        if (operatorIndex == -1)
            return false;

        // Operator exists but no second number yet
        if (operatorIndex == text.Length - 1)
            return false;

        // Try parsing both sides
        return
            float.TryParse(text[..operatorIndex], out _) &&
            float.TryParse(text[(operatorIndex + 1)..], out _);
    }


}
