using UnityEngine;

[CreateAssetMenu(menuName = "BadgeData")]
public class BadgeData : ScriptableObject
{
    public string displayName; // Used as both ID and name
    public string description; // What the badge represents
    public Sprite icon;
}