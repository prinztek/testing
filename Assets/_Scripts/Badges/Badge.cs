using TMPro;
using UnityEngine;
using UnityEngine.UI;

// To display a badge in the UI
public class Badge : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;

    // Setup the UI based on the BadgeData
    public void Setup(BadgeData data)
    {
        nameText.text = string.IsNullOrEmpty(data.displayName) ? "No name." : data.displayName;
        descriptionText.text = string.IsNullOrEmpty(data.description) ? "No description." : data.description;
        iconImage.sprite = data.icon ? data.icon : null; // assign icon
    }
}
