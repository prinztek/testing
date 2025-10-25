using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    [SerializeField] private CinemachineCamera[] _allCinemachineVirtualCamera;

    [Header("Controls for lerping and Y Damping during player jump/fall")]
    [SerializeField] public float _fallPanAmount = 0.25f;
    [SerializeField] public float _fallPanTime = 0.35f;
    [SerializeField] public float _fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;
    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer _positionComposer;
    private float _normYPanAmount;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Find the active CinemachineVirtualCamera
        for (int i = 0; i < _allCinemachineVirtualCamera.Length; i++)
        {
            if (_allCinemachineVirtualCamera[i].enabled)
            {
                // set the current active camera
                _currentCamera = _allCinemachineVirtualCamera[i];

                // Get the Position Composer component (new replacement for FramingTransposer)
                _positionComposer = _currentCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
                if (_positionComposer == null)
                {
                    Debug.LogError("Position Composer not found on active CinemachineCamera!");
                    return;
                }
                break;
            }
        }

        // set the YDamping amount so it's based on the inspector value
        _normYPanAmount = _positionComposer.Damping.y;

    }
    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        // grab the starting damping amount
        float startDampAmount = _positionComposer.Damping.y;
        float endDampAmount = 0f;

        // determine the end damping amount 
        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        // lerp the pan amount
        float elapsedTime = 0f;
        while (elapsedTime < _fallPanTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / _fallPanTime);
            _positionComposer.Damping.y = lerpedPanAmount;
            yield return null;
        }
        IsLerpingYDamping = false;
    }
}
