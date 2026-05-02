using UnityEngine;

public class FirstLevelLoader : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.Instance.LoadTutorialLevel();
    }

}
