using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    InputAction moveAction;
    Vector2 moveInput;

    InputAction sprintAction;
    float sprintInput;

    InputAction sneakAction;
    float sneakInput;

    CharacterController characterController;

    [SerializeField]
    Transform cameraTransform;

    [SerializeField]
    float walkSpeed;

    [SerializeField]
    float sprintSpeed;

    [SerializeField]
    float sneakSpeed;

    [SerializeField]
    float rotationSpeed;

    [SerializeField]
    float gravity = -9.81f;

    EnemyController[] enemies;
    float movementSpeed;
    Vector3 moveDirection;

    float verticalVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        sneakAction = InputSystem.actions.FindAction("Sneak");

        characterController = GetComponent<CharacterController>();

        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (EnemyController enemy in enemies)
            enemy.SetPlayer(transform);
    }

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
        sneakInput = sneakAction.ReadValue<float>();
    }

    void UpdateMovementSpeed()
    {
        if (sneakInput != 0f)
            movementSpeed = sneakSpeed;
        else if (sprintInput != 0f)
            movementSpeed = sprintSpeed;
        else
            movementSpeed = walkSpeed;
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
        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;       // Keeps grounded consistently
        }

        // Horizontal movement
        Vector3 movement = movementSpeed * Time.deltaTime * moveDirection;

        // Add gravity to movement
        movement.y = verticalVelocity * Time.deltaTime;

        // Move character
        characterController.Move(movement);

        // Alert enemies
        if (sneakInput == 0f && moveInput != Vector2.zero)
            foreach (EnemyController enemy in enemies)
                enemy.HearSound(transform.position);
    }
}
