using UnityEngine;

public class BasicInteractableObject : MonoBehaviour
{
    // BasicInteractableObject - an interactable that opens up a canvas with the mini game
    public GameObject runeGamePrefab; // Reference to your mini game/ puzzle canvas - assign in inspector - the window that pops up when interacting
    public StoneWall stoneWall; // Reference to the stone wall object - or any gate to be lifted - assign in inspector
    private GameObject spawnedPuzzle;

    public bool isPuzzleSolved = false;
    public void Interact()
    {

        // If already solved, do nothing
        if (isPuzzleSolved == true)
        {
            return;
        }

        if (spawnedPuzzle == null)
        {
            spawnedPuzzle = Instantiate(runeGamePrefab);

            // Try to get either puzzle manager (RuneGameManager (Permutation and its condition) or (Permutation))
            var permutationAndConditionPuzzle = spawnedPuzzle.GetComponentInChildren<RuneGameManager>();
            var factorialTotemPuzzle = spawnedPuzzle.GetComponentInChildren<FactorialTotemGameManager>();
            var permutationPuzzle = spawnedPuzzle.GetComponentInChildren<PermutationRuneGameManager>();

            if (permutationAndConditionPuzzle != null)
            {
                permutationAndConditionPuzzle.OnPuzzleSolved.AddListener(OnPuzzleSolved);
            }
            else if (permutationPuzzle != null)
            {
                permutationPuzzle.OnPuzzleSolved.AddListener(OnPuzzleSolved);
            }
            else if (factorialTotemPuzzle != null)
            {
                factorialTotemPuzzle.OnPuzzleSolved.AddListener(OnPuzzleSolved);
            }
            else
            {
                Debug.LogWarning("No puzzle manager found on spawned prefab!");
            }
        }
        UIManager.Instance.ShowModal(spawnedPuzzle);
    }
    public void OnPuzzleSolved()
    {
        isPuzzleSolved = true;
        stoneWall.Lift();
        CloseCanvas();
    }

    public void CloseCanvas()
    {
        UIManager.Instance.CloseActivePanel();
        // uiCanvas.SetActive(false);
    }
}
