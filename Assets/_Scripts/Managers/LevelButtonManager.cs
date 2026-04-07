using System.Collections.Generic;
using UnityEngine;

public class LevelButtonManager : MonoBehaviour
{
    public static LevelButtonManager Instance;
    private bool isLoading = false;
    private List<LevelButton> buttons = new List<LevelButton>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterButton(LevelButton btn)
    {
        if (!buttons.Contains(btn))
            buttons.Add(btn);
    }

    public void DisableAllButtons()
    {
        isLoading = true;
        foreach (var btn in buttons)
            btn.SetInteractable(false);
    }

    public bool IsLoading() => isLoading;
}