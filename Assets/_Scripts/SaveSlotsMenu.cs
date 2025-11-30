using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject chapterSelectScreen;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    private SaveSlot[] saveSlots;

    private bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    private void Start()
    {
        // ActivateMenu(isLoadingGame);
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        // optionally ask first for the name and save that saveSlot profileId with the inputted name
        // disable all buttons
        DisableMenuButtons();

        // update the selected profile id to be used for data persistence
        GameManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
        GameManager.Instance.LoadGame();
        // new game
        if (!isLoadingGame)
        {
            // create a new game - which will initialize our data to a clean slate
            GameManager.Instance.NewGame();
            GameManager.Instance.LoadLevel(0, 0); // first level
        }
        else // existing save
        {
            // go to chapter select instead of loading directly into gameplay
            chapterSelectScreen.SetActive(true);
        }

    }

    public void ActivateMenu(bool isLoadingGame)
    {
        this.isLoadingGame = isLoadingGame;

        // load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = GameManager.Instance.GetAllProfilesGameData();

        // loop through each save slot in the UI and set the content appropriately
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
            if (profileData == null & isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
            }

        }

    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
        backButton.interactable = false;
    }

    public void OnNewGameClicked()
    {
        ActivateMenu(false);
    }

    public void OnLoadGameClicked()
    {
        ActivateMenu(true);
    }

}