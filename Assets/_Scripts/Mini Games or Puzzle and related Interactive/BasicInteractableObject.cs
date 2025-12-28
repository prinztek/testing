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
            var puzzle = spawnedPuzzle.GetComponentInChildren<RuneGameManager>();

            puzzle.OnPuzzleSolved.AddListener(OnRuneGameSolved);
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
