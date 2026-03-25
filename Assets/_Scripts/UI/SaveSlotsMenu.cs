using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private EnterNamePanel enterNamePanel;
    [SerializeField] private OverwriteSavePanel overwriteSavePanel;
    [SerializeField] private GameObject chapterSelectScreen;
    [SerializeField] private TextMeshProUGUI BannerText; // Load or New Game banner text

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button confirmButton;

    private SaveSlot[] saveSlots;
    private SaveSlot selectedSlot;


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
        selectedSlot = saveSlot;

        // visually highlight UI
        HighlightSelectedSlot(saveSlot);

        // enable confirm button when a slot is selected
        confirmButton.interactable = true;
    }

    private void HighlightSelectedSlot(SaveSlot slot)
    {
        foreach (var s in saveSlots)
            s.SetHighlight(false);

        slot.SetHighlight(true);
    }



    public void OnConfirmButtonClicked()
    {
        if (selectedSlot == null)
            return;

        if (!isLoadingGame) // NEW GAME
        {
            if (selectedSlot.HasData())
            {
                overwriteSavePanel.Show(
                    $"Slot {selectedSlot.GetProfileId()} already has a save.\nOverwrite it?",
                    () => enterNamePanel.Open(OnNameEntered),
                    () => { }
                );
                return;
            }
            // create a new game - which will initialize our data to a clean slate
            enterNamePanel.Open(OnNameEntered);
        }
        else // LOAD GAME
        {
            LoadExistingGame();
        }
    }


    private void OnNameEntered(string name)
    {
        // Tell GameManager which slot to save into
        GameManager.Instance.ChangeSelectedProfileId(selectedSlot.GetProfileId());

        // Create a new GameData with playerName
        GameManager.Instance.NewGame(name);

        // Save immediately
        GameManager.Instance.SaveGame();
        GameManager.Instance.LoadGame2();

        // Load story intro scene (from this scene it will auto load the first level)

        GameManager.Instance.TriggerIntro();

        // Load first level
        // GameManager.Instance.LoadLevel(0, 0);
    }

    private void LoadExistingGame()
    {
        GameManager.Instance.ChangeSelectedProfileId(selectedSlot.GetProfileId());
        GameManager.Instance.LoadGame();
        // go to chapter select instead of loading directly into gameplay
        chapterSelectScreen.SetActive(true);
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        this.isLoadingGame = isLoadingGame;

        BannerText.text = isLoadingGame ? "Load Game" : "New Game";

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
                saveSlot.SetInteractable(false); // from load game, disable empty slots
            }
            else
            {
                saveSlot.SetInteractable(true); // from new game, enable all slots but keep empty ones interactive
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
        confirmButton.interactable = false;
    }

    public void OnNewGameClicked()
    {
        ActivateMenu(false);
    }

    public void OnLoadGameClicked()
    {
        ActivateMenu(true);
    }

    public bool IsEmptySlot()
    {
        return GameManager.Instance.gameData == null;
    }

    private void EnableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(true);
        }
        backButton.interactable = true;
        confirmButton.interactable = true;
    }

}



// overwrite save file bug