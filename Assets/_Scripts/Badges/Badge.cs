using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "BadgeData")]
public class BadgeData : ScriptableObject
{
    public string displayName; // Used as both ID and name
    public string description; // What the badge represents
    public Sprite icon;
}

// To display a badge in the UI
public class Badge : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;

    // Setup the UI based on the BadgeData
    public void Setup(BadgeData data)
    {
        nameText.text = data.displayName;
        descriptionText.text = string.IsNullOrEmpty(data.description) ? "No description." : data.description;
        iconImage.sprite = data.icon ? data.icon : null; // assign icon
    }
}
