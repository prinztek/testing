using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireTutorialController : MonoBehaviour
{
    public static GrimoireTutorialController Instance { get; private set; }

    [Header("Steps")]
    public List<TutorialStep> steps = new List<TutorialStep>();

    [Header("UI References")]
    public TooltipUI tooltip;
    public HighlightUI highlight;
    public Image overlay; // optional dim background
    private int currentStepIndex = 0;
    private RectTransform currentTarget;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void Initialize(GrimoireManager grimoire)
    {
        steps = new List<TutorialStep>()
    {
        new TutorialStep {
            target = grimoire.closeTab,
            message = "Close the grimoire and return to the game.",
            offset = new Vector2(-56, 0),
            highlightPosition = new Vector2(0, 0),
        },
        new TutorialStep {
            target = grimoire.questionTab,
            message = "Answer math question to gain a buff, get hints to help you solve them.",
            offset = new Vector2(-56, 0),
            highlightPosition = new Vector2(0, 0),
        },
        new TutorialStep {
            target = grimoire.inventoryTab,
            message = "Check your stats, and inventory.",
            offset = new Vector2(-56, 0),
            highlightPosition = new Vector2(0, 0),
        },
        new TutorialStep {
            target = grimoire.craftingTab,
            message = "Craft potions and weapons in exchange of gold.",
            offset = new Vector2(-56, 0),
            highlightPosition = new Vector2(0, 0),
        },
        new TutorialStep {
            target = grimoire.calculatorTab,
            message = "Open the calculator to help solve math problems quickly.",
            offset = new Vector2(-56, 0),
            highlightPosition = new Vector2(0, 0),
        },
        new TutorialStep {
            target = grimoire.modulesTab,
            message = "Review lessons and concepts to improve your understanding.",
            offset = new Vector2(-56, 0),
            highlightPosition = new Vector2(0, 0),
        }
    };

        StartCoroutine(StartNextFrame());
    }

    IEnumerator StartNextFrame()
    {
        yield return null; // wait 1 frame
        StartTutorial();
    }
    public void StartTutorial()
    {
        currentStepIndex = 0;
        ShowStep(currentStepIndex);

        if (overlay != null)
            overlay.gameObject.SetActive(true);
    }

    void ShowStep(int index)
    {

        // Reset previous highlight FIRST
        if (currentTarget != null)
        {
            RemoveHighlight(currentTarget);
        }

        var step = steps[index];

        currentTarget = step.target;

        // Highlight target
        Highlight(step.target);

        highlight.Show(step.target, step.highlightPosition);

        // Show tooltip 
        tooltip.Show(step.message, step.target, step.offset);
    }

    public void NextStep()
    {
        Debug.Log("Next tutorial step");
        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            EndTutorial();
            return;
        }

        ShowStep(currentStepIndex);
    }

    void EndTutorial()
    {
        if (currentTarget != null)
        {
            RemoveHighlight(currentTarget);
        }

        tooltip.Hide();

        if (overlay != null)
            overlay.gameObject.SetActive(false);

        highlight.Hide();

        GrimoireManager.Instance.canvasGroup.blocksRaycasts = true;

        Debug.Log("Tutorial Finished");
    }

    void Highlight(RectTransform target)
    {
        // Simple highlight: scale up
        target.localScale = Vector3.one * 1.1f;
    }

    void RemoveHighlight(RectTransform target)
    {
        // Simple highlight: scale back to normal
        target.localScale = Vector3.one * 1f;
    }
}