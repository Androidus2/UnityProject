using UnityEngine;
using UnityEngine.InputSystem;

public class PopUpPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject Panel;
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
