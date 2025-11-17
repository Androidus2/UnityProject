using UnityEngine;
using UnityEngine.InputSystem;

public class PopUpPanelI : MonoBehaviour
{
    public GameObject Panel;
    InputAction inventoryButton;

    void Awake()
    {
        inventoryButton = InputSystem.actions.FindAction("Inventory");
        inventoryButton.performed += ctx => TogglePanel();
    }

    void OnEnable()
    {
        inventoryButton.Enable();
    }

    void OnDisable()
    {
        inventoryButton.Disable();
    }

    void TogglePanel()
    {
        Panel.SetActive(!Panel.activeSelf);
    }
}
