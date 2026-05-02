using UnityEngine;

public class InfoBoard : MonoBehaviour
{

    public GameObject informationCanvas; // Reference to your information canvas - assign in inspector - the window that pops up when interacting
    private GameObject spawnedInformationCanvas;
    public GameObject spotLight;
    public GameObject visualContext;
    public GameObject invisibleWall; // Reference to the invisible wall - assign in inspector (collider object that blocks player from progressing until they interact with the info board)
    public bool isInTutorial = false;

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
        // remove invisible wall if it exists
        if (isInTutorial == true)
        {
            RemoveInvisibleWall();
        }
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

    public void RemoveInvisibleWall()
    {
        // Implement logic to remove invisible wall here
        // For example, you could disable a collider or change the layer of the wall to make it non-solid
        if (invisibleWall != null)
        {
            Debug.Log("Invisible wall removed!");

            // if the wall is disabled already, we don't need to do anything
            if (isInTutorial == true)
            {
                // If in tutorial, we might want to keep the wall for a bit longer or trigger some tutorial-specific behavior
                // For now, we'll just log that we're in the tutorial and not remove the wall immediately
                invisibleWall.SetActive(false);
            }
        }
    }
}
