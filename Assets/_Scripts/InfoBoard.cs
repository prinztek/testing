using UnityEngine;

public class InfoBoard : MonoBehaviour
{

    public GameObject infoBoardCanvas; // Reference to your information canvas - assign in inspector - the window that pops up when interacting
    private GameObject spawnedPuzzle;
    public GameObject spotLight;
    public GameObject visualContext;

    public void Interact()
    {
        if (spawnedPuzzle == null)
        {
            spawnedPuzzle = Instantiate(infoBoardCanvas);

        }
        UIManager.Instance.ShowModal(spawnedPuzzle);
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
