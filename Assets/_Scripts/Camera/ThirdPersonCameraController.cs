using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private CinemachineOrbitalFollow orbital;
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 15f;

    private Controls _inputActions;
    private Vector2 _scrollDelta;

    private float _targetZoom;
    private float _currentZoom;

    private void Awake()
    {
        _inputActions = new Controls();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _targetZoom = _currentZoom = orbital.Radius;
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions.CameraControls.MouseZoom.performed += HandleMouseScroll;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        _inputActions.CameraControls.MouseZoom.performed -= HandleMouseScroll;
    }
    private void Update()
    {
        if(_scrollDelta.y != 0 && orbital)
        {
            _targetZoom = Mathf.Clamp(orbital.Radius-_scrollDelta.y*zoomSpeed,minDistance,maxDistance);
            _scrollDelta = Vector2.zero;
        }
        float bumperDelta = _inputActions.CameraControls.GamepadZoom.ReadValue<float>();
        if (bumperDelta != 0){
            _targetZoom = Mathf.Clamp(orbital.Radius - bumperDelta * zoomSpeed, minDistance, maxDistance);
        }
        _currentZoom = Mathf.Lerp(_currentZoom,_targetZoom,Time.deltaTime*zoomLerpSpeed);
        orbital.Radius = _currentZoom;
    }

    private void HandleMouseScroll(InputAction.CallbackContext context)
    {
        _scrollDelta = context.ReadValue<Vector2>();
    }

}
