using UnityEngine;

public class FactorialInteractableObject : MonoBehaviour
{
    // BasicInteractableObject - an interactable that opens up a canvas with the mini game
    public GameObject totemGamePrefab; // Reference to your mini game/ puzzle canvas - assign in inspector - the window that pops up when interacting
    public StoneWall stoneWall; // Reference to the stone wall object - or any gate to be lifted - assign in inspector
    private GameObject spawnedPuzzle;

    public void Interact()
    {
        if (spawnedPuzzle == null)
        {
            spawnedPuzzle = Instantiate(totemGamePrefab);

            // Try to get either puzzle manager (RuneGameManager (Permutation and its condition) or (Permutation))
            var factorialTotemPuzzle = spawnedPuzzle.GetComponentInChildren<FactorialTotemGameManager>();

            if (factorialTotemPuzzle != null)
            {
                factorialTotemPuzzle.OnPuzzleSolved.AddListener(OnRuneGameSolved);
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
