using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public TMPro.TextMeshProUGUI fpsText;       // Reference to a UI Text component
    public float refreshRate = 0.5f; // Update interval in seconds

    private int frameCount;
    private float timer;

    void Update()
    {
        frameCount++;
        timer += Time.unscaledDeltaTime;

        if (timer >= refreshRate)
        {
            int fps = Mathf.RoundToInt(frameCount / timer);
            fpsText.text = "FPS: " + fps;

            frameCount = 0;
            timer = 0f;
        }
    }
}
