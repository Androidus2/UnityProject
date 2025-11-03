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
    float walkSpeed;

    [SerializeField]
    float sprintSpeed;

    float movementSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");

        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();

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

    void DoMovement()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * Time.deltaTime * movementSpeed;
        characterController.Move(movement);
    }
}
