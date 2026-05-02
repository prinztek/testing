using Unity.VisualScripting;
using UnityEngine;

public class SkipIntroManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SkipCutScene()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadTutorialLevel();
        }
    }
}
