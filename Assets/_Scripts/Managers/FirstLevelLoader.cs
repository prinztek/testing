using UnityEngine;

public class FirstLevelLoader : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.Instance.LoadLevel(0, 0);
    }

}
