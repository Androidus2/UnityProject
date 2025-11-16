using System.Collections.Generic;
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
    float walkSpeed;

    [SerializeField]
    float sprintSpeed;

    [SerializeField]
    float sneakSpeed;

    EnemyController[] enemies;

    float movementSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    void DoMovement()
    {
        Vector3 movement = movementSpeed * Time.deltaTime * new Vector3(moveInput.x, 0, moveInput.y);
        characterController.Move(movement);

        // If we aren't sneaking and have moved, alert the enemies
        if (sneakInput == 0f && moveInput != Vector2.zero)
            foreach(EnemyController enemy in enemies)
                enemy.HearSound(transform.position);
    }
}
