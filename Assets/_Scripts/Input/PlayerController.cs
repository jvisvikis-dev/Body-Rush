using Unity.Android.Gradle.Manifest;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines.Interpolators;

public class PlayerController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private CinemachineCamera cam;
    [Header("MoveSettings")]
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float groundCheckDistance = 1.2f;
    [Header("LookSettings")]
    [SerializeField] private float maxLookAngle = 90f;
    [SerializeField] private float turnSpeed = 0.5f;
    [SerializeField] private bool shouldFaceMoveDirection = true;


    private Controls _inputActions;
    private Vector3 velocity = Vector3.zero;
    private bool allowedMovement = true;


    private void Awake()
    {
        _inputActions = new Controls();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions.Player.Jump.performed += Jump;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (allowedMovement)
        {
            HandleMovement();
            HandleRotation();
        }
    }
    private Vector3 GetPlayerMovement()
    {
        Vector2 values = _inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 direction = new Vector3(values.x, 0, values.y);
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;
        forward.y = 0;
        right.y = 0;    
        forward = forward.normalized;
        right = right.normalized;
        Vector3 move = direction.z * forward + right * direction.x;
        move = Vector3.ClampMagnitude(move, 1f);
        return move;
    }

    private void HandleMovement()
    {
        bool isGrounded = IsGrounded();
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        Vector3 move = GetPlayerMovement();
        controller.Move(move * Time.deltaTime * speed);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


    }

    private void HandleRotation()
    {
        Vector3 move = GetPlayerMovement();
        if (shouldFaceMoveDirection && move.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(move,Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {

        bool isGrounded = IsGrounded();
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.03f, Vector3.down, groundCheckDistance);
    }

    public void ToggleAllowedMovement()
    {
        allowedMovement = !allowedMovement;
    }

}
