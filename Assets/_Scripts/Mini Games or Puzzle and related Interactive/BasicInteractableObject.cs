using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class BasicInteractableObject : MonoBehaviour
{
    // BasicInteractableObject - an interactable that opens up a canvas with the mini game
    public GameObject runeGamePrefab; // Reference to your mini game/ puzzle canvas - assign in inspector - the window that pops up when interacting
    public StoneWall stoneWall; // Reference to the stone wall object - or any gate to be lifted - assign in inspector
    public List<StompHazard> stompHazardsToActivate; // List of stomp hazards to activate when puzzle is solved - assign in inspector
    [SerializeField] private float phaseOffset = 0.25f;
    private GameObject spawnedPuzzle;
    public GameObject spotLight;
    public GameObject visualContext;
    public bool isPuzzleSolved = false;

    void Start()
    {
        ActivateWave();
    }

    public void ActivateWave()
    {
        int count = stompHazardsToActivate.Count;
        float totalDuration = phaseOffset * count;

        for (int i = 0; i < count; i++)
        {
            int index = i;

            // Normalized position 0 → 1
            float t = (float)index / (count - 1);

            // Sine ease-in-out (0 → 1 → 0 speed)
            float sine = Mathf.Sin(t * Mathf.PI * 0.5f);

            float delay = sine * totalDuration;

            DOVirtual.DelayedCall(delay, () =>
            {
                stompHazardsToActivate[index].Activate();
            });
        }
    }

    public void ActivateStomp()
    {
        foreach (var hazard in stompHazardsToActivate)
        {
            hazard.Activate();
        }
    }

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
            var permutationAndConditionPuzzle = spawnedPuzzle.GetComponentInChildren<PermutationAndConditionRuneGameManager>();
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

        if (stoneWall != null)
        {
            stoneWall.Lift();
        }

        if (stompHazardsToActivate.Count > 0)
        {
            foreach (var hazard in stompHazardsToActivate)
            {
                hazard.Deactivate();
            }
        }
        CloseCanvas();
    }

    public void CloseCanvas()
    {
        UIManager.Instance.CloseActivePanel();
        // uiCanvas.SetActive(false);
    }

    public void HighlightObject()
    {
        // spotLight.SetActive(true);
        visualContext.SetActive(true);
    }

    public void RemoveHighlightObject()
    {
        // spotLight.SetActive(false);
        visualContext.SetActive(false);
    }
}
