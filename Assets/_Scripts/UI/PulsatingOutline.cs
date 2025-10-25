using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class PulsatingOutline : MonoBehaviour
{
    [Header("Pulse Settings")]
    public Outline targetOutline;
    public float pulseSpeed = 2f;          // how fast the pulse is
    public float minAlpha = 0.3f;          // minimum opacity
    public float maxAlpha = 1f;            // maximum opacity
    public bool useColorShift = true;      // optional shimmer effect

    private Color baseColor;

    void Start()
    {
        if (targetOutline == null)
            targetOutline = GetComponent<Outline>();

        baseColor = targetOutline.effectColor;
    }

    void Update()
    {
        // Create a smooth pulsing alpha (breathing effect)
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);

        Color c = baseColor;

        if (useColorShift)
        {
            // Add a subtle hue shimmer to draw attention gently
            float hueShift = (Mathf.Sin(Time.time * pulseSpeed * 0.3f) + 1f) * 0.05f; // small hue oscillation
            Color.RGBToHSV(baseColor, out float h, out float s, out float v);
            h = (h + hueShift) % 1f;
            c = Color.HSVToRGB(h, s, v);
        }

        // Apply pulsing transparency
        c.a = alpha;
        targetOutline.effectColor = c;
    }
}
