using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    InputAction moveAction;
    Vector2 moveInput;

    InputAction sprintAction;
    float sprintInput;

    CharacterController characterController;

    [SerializeField]
    Transform cameraTransform;

    [SerializeField]
    float walkSpeed;

    [SerializeField]
    float sprintSpeed;

    [SerializeField]
    float rotationSpeed;

    float movementSpeed;
    Vector3 moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");

        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
        RotateMoveInputToCameraDirection();
        RotatePlayer();

        UpdateMovementSpeed();

        DoMovement();
    }

    void ReadInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        sprintInput = sprintAction.ReadValue<float>();
    }

    void UpdateMovementSpeed()
    {
        if (sprintInput == 0f)
            movementSpeed = walkSpeed;
        else
            movementSpeed = sprintSpeed;
    }

    void RotateMoveInputToCameraDirection()
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        moveDirection = camForward * moveInput.y + camRight * moveInput.x;
    }

    void RotatePlayer()
    {
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

    }

    void DoMovement()
    {
        Vector3 movement = movementSpeed * Time.deltaTime * moveDirection;
        characterController.Move(movement);
    }
}
