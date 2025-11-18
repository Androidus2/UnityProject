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

    EnemyController[] enemies;
    float movementSpeed;
    Vector3 moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        sneakAction = InputSystem.actions.FindAction("Sneak");

        characterController = GetComponent<CharacterController>();

        // Find all enemies in the scene
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        // Give them a reference to the player through code so we don't have to do it in the inspector
        foreach (EnemyController enemy in enemies)
            enemy.SetPlayer(transform);
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
        sneakInput = sneakAction.ReadValue<float>();
    }

    void UpdateMovementSpeed()
    {
        // If the player presses both sneak and sprint, give priority to sneaking so he doesn't accidentally make a sound
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
        Vector3 movement = movementSpeed * Time.deltaTime * moveDirection;
        characterController.Move(movement);

        // If we aren't sneaking and have moved, alert the enemies
        if (sneakInput == 0f && moveInput != Vector2.zero)
            foreach(EnemyController enemy in enemies)
                enemy.HearSound(transform.position);
    }
}
