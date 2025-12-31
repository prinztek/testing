using UnityEngine;

public class BasicInteractableObject : MonoBehaviour
{
    // BasicInteractableObject - an interactable that opens up a canvas with the mini game
    public GameObject runeGamePrefab; // Reference to your mini game/ puzzle canvas - assign in inspector - the window that pops up when interacting
    public StoneWall stoneWall; // Reference to the stone wall object - or any gate to be lifted - assign in inspector
    private GameObject spawnedPuzzle;

    public void Interact()
    {
        if (spawnedPuzzle == null)
        {
            spawnedPuzzle = Instantiate(runeGamePrefab);

            // Try to get either puzzle manager (RuneGameManager (Permutation and its condition) or (Permutation))
            var permutationAndConditionPuzzle = spawnedPuzzle.GetComponentInChildren<RuneGameManager>();
            var permutationPuzzle = spawnedPuzzle.GetComponentInChildren<PermutationRuneGameManager>();

            if (permutationAndConditionPuzzle != null)
            {
                permutationAndConditionPuzzle.OnPuzzleSolved.AddListener(OnRuneGameSolved);
            }
            else if (permutationPuzzle != null)
            {
                permutationPuzzle.OnPuzzleSolved.AddListener(OnRuneGameSolved);
            }
            else
            {
                Debug.LogWarning("No puzzle manager found on spawned prefab!");
            }
        }

        UIManager.Instance.ShowModal(spawnedPuzzle);
    }
    public void OnRuneGameSolved()
    {
        stoneWall.Lift();
        CloseCanvas();
    }

    public void CloseCanvas()
    {
        UIManager.Instance.CloseActivePanel();
        // uiCanvas.SetActive(false);
    }
}
