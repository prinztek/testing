using UnityEngine;

public class InformationSignage : MonoBehaviour
{
    [SerializeField] private GameObject informationCanvasPrefab; // The text to display when the player interacts with the signage
    public GameObject visualContext;
    private GameObject spawnedInformationCanvas;

    public void Interact()
    {
        if (spawnedInformationCanvas == null)
        {
            spawnedInformationCanvas = Instantiate(informationCanvasPrefab);

        }

        // TODO: Show information signage UI
        UIManager.Instance.ShowModal(informationCanvasPrefab);
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
