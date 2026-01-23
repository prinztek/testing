using UnityEngine;
using UnityEngine.EventSystems;

public class RotateObject : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Rotation")]
    public float sensitivity = 1f;
    public bool useLimits = false;
    public float minRotation = -180f;
    public float maxRotation = 180f;

    [Header("Snapping")]
    public bool snapOnRelease = true;
    public float snapAngle = 15f;

    [Header("Momentum")]
    public bool useMomentum = true;
    public float momentumDamping = 8f;

    [Header("Puzzle")]
    public float correctAngle = 90f;
    public float tolerance = 2f;

    private RectTransform rectTransform;

    private float startAngle;
    private float startRotation;
    private float angularVelocity;
    private bool isDragging;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        angularVelocity = 0f;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        startAngle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;
        startRotation = rectTransform.eulerAngles.z;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        float currentAngle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;
        float delta = (currentAngle - startAngle) * sensitivity;

        float targetRotation = startRotation + delta;

        if (useLimits)
            targetRotation = Mathf.Clamp(targetRotation, minRotation, maxRotation);

        angularVelocity = (targetRotation - rectTransform.eulerAngles.z) / Time.deltaTime;
        rectTransform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        if (snapOnRelease)
            SnapRotation();

        CheckPuzzleSolved();
    }

    void Update()
    {
        if (!isDragging && useMomentum)
        {
            if (Mathf.Abs(angularVelocity) > 0.1f)
            {
                float newRot = rectTransform.eulerAngles.z + angularVelocity * Time.deltaTime;

                if (useLimits)
                {
                    newRot = Mathf.Clamp(newRot, minRotation, maxRotation);
                    angularVelocity = 0f;
                }

                rectTransform.rotation = Quaternion.Euler(0f, 0f, newRot);
                angularVelocity = Mathf.Lerp(angularVelocity, 0f, momentumDamping * Time.deltaTime);
            }
        }
    }

    void SnapRotation()
    {
        float z = rectTransform.eulerAngles.z;
        float snapped = Mathf.Round(z / snapAngle) * snapAngle;
        rectTransform.rotation = Quaternion.Euler(0f, 0f, snapped);
    }

    void CheckPuzzleSolved()
    {
        float z = rectTransform.eulerAngles.z;
        if (Mathf.Abs(Mathf.DeltaAngle(z, correctAngle)) <= tolerance)
        {
            OnPuzzleSolved();
        }
    }

    void OnPuzzleSolved()
    {
        Debug.Log("Puzzle Solved!");

        // 🔊 Play sound
        // 📳 Trigger haptics
        // ✨ Glow / animate
        // 🔓 Notify puzzle manager
    }
}
