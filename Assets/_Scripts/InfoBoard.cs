using UnityEngine;

public class InfoBoard : MonoBehaviour
{

    public GameObject informationCanvas; // Reference to your information canvas - assign in inspector - the window that pops up when interacting
    private GameObject spawnedInformationCanvas;
    public GameObject spotLight;
    public GameObject visualContext;

    public void Interact()
    {
        if (spawnedInformationCanvas == null)
        {
            spawnedInformationCanvas = Instantiate(informationCanvas);

        }
        UIManager.Instance.ShowModal(spawnedInformationCanvas);
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
