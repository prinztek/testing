using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class FactorialTotemGameManager : MonoBehaviour
{
    [Header("Factorial")]
    public int factorialNumber;

    [Header("UI")]
    public Totem totemPrefab;
    public RectTransform totemParent;
    public Button submitButton;

    private List<Totem> totems = new();
    public UnityEvent OnPuzzleSolved;
    public bool isSolved = false;

    void Start()
    {
        SpawnTotems();
        submitButton.onClick.AddListener(CheckAnswer);
    }

    void SpawnTotems()
    {
        // edge case if 0!
        if (factorialNumber == 0)
        {
            factorialNumber = 1;
            Totem totem = Instantiate(totemPrefab, totemParent);

            RectTransform rect = totem.GetComponent<RectTransform>();

            totem.Initialize(factorialNumber);
            totems.Add(totem);
        }

        if (factorialNumber > 1)
        {
            for (int i = 0; i < factorialNumber; i++)
            {
                Totem totem = Instantiate(totemPrefab, totemParent);

                RectTransform rect = totem.GetComponent<RectTransform>();

                totem.Initialize(factorialNumber);
                totems.Add(totem);
            }
        }

    }

    void CheckAnswer()
    {
        if (factorialNumber == 0)
        {
            int expected = 1;
            if (totems[0].currentValue != expected)
            {
                Debug.Log("Incorrect factorial input");
                return;
            }
            else
            {
                Debug.Log("Correct! Factorial understood.");
                isSolved = true;
                OnPuzzleSolved?.Invoke();
                return;
            }
        }

        // if factorialNumber is n, expected sequence is n, n-1, n-2, ..., 2, 1
        for (int i = 0; i < totems.Count; i++)
        {
            int expected = factorialNumber - i;

            if (totems[i].currentValue != expected)
            {
                Debug.Log("Incorrect factorial input");
                return;
            }
        }

        Debug.Log("Correct! Factorial understood.");
        // success logic here
        isSolved = true;
        OnPuzzleSolved?.Invoke();
    }
}
