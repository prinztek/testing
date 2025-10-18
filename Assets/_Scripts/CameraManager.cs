using NUnit.Framework;
using Unity.Cinemachine;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // [SerializeField] private CinemachineVirtualCamera[] cinemachineVirtualCamera; // deprecated
    [SerializeField] private CinemachineCamera[] _allCinemachineVirtualCamera; // deprecated
    public static CameraManager Instance { get; private set; }

    [Header("Controls for lerping and Y Damping during player jump/fall")]
    [SerializeField] public float _fallPanAmount = 0.25f;
    [SerializeField] public float _fallPanTime = 0.35f;
    public float _fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; private set; }

    private Coroutine _lerpYDampingCoroutine;
    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer _cinemachinePositionComposer;     // private CinemachineFramingTransposer _framingTransposer; // deprecated

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
        }

        for (int i = 0; i < _allCinemachineVirtualCamera.Length; i++)
        {
            if (_allCinemachineVirtualCamera[i].enabled)
            {
                // set the current active cinemachine virtual camera
                _currentCamera = _allCinemachineVirtualCamera[i];

                // set the framing transposer reference // deprecated
                // _framingTransposer = _currentCamera.GetCinemachineComponent<Cinemachine
                _cinemachinePositionComposer = _currentCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
            }
        }

        // set the YDamping amount so it's based on the inspector value at start
        // _normYPanAmount = _cinemachinePositionComposer.m_YDamping;
    }
}
