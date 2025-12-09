using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI profileIdText;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Highlight")]
    [SerializeField] private GameObject bottomBorder;
    private bool hasData = false;
    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        // there's no data for this profileId
        if (data == null)
        {
            hasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        // there is data for this profileId
        else
        {
            hasData = true;
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            profileIdText.text = data.playerName;
            // find latest unlocked chapter/level
            int latestChapter = 0;
            int latestLevel = 0;

            for (int c = 0; c < data.save.chapters.Length; c++)
            {
                for (int l = 0; l < data.save.chapters[c].levels.Length; l++)
                {
                    if (data.save.chapters[c].levels[l].isUnlocked)
                    {
                        latestChapter = c;
                        latestLevel = l;
                    }
                }
            }

            progressText.text = $"Chapter {latestChapter + 1}, Level {latestLevel + 1}";
        }
    }

    public string GetProfileId()
    {
        Debug.Log("GetProfileId: " + profileId);
        return profileId;
    }

    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
    }

    public bool HasData()
    {
        return hasData;
    }

    public void SetHighlight(bool on)
    {
        if (bottomBorder != null)
            bottomBorder.SetActive(on);
    }

    // this script only displays whether the save slot is taken or empty
    // or set it to non-interactive for empty slots on load game 
}