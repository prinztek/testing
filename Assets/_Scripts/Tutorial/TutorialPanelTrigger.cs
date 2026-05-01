using UnityEngine;

public class TutorialPanelTrigger : MonoBehaviour
{
    [SerializeField] private GameObject panelToShow;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialLevelManager.Instance.ShowPanel(panelToShow);
        }
    }
}