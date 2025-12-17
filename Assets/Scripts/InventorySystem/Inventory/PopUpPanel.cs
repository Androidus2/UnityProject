using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopUpPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject Panel;
    private InputAction inventoryButton;

    [SerializeField]
    private InventoryObject playerInventory;

    Tweener scaleTween;
    [SerializeField]
    private float scaleDuration = 0.15f;

    private DisplayInventory displayInventory;

    private bool isPanelOpen = false;

    void Awake()
    {
        inventoryButton = InputSystem.actions.FindAction("Inventory");
        inventoryButton.performed += ctx => TogglePanel();

        displayInventory = Panel.GetComponent<DisplayInventory>();
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
        if (isPanelOpen)
            ClosePanel();
        else
            OpenPanel();
    }

    void OpenPanel()
    {
        //this script is specifically for the player's inventory
        displayInventory.SetInventoryObject(playerInventory);

        // Kill any ongoing tween to avoid conflicts with animation
        scaleTween?.Kill();

        // Ensure panel is visible before animation
        Panel.SetActive(true);
        Panel.transform.localScale = Vector3.zero; 

        scaleTween = Panel.transform
            .DOScale(Vector3.one, scaleDuration)
            .SetEase(Ease.OutBack, 1.4f)
            .SetUpdate(true) // Ensures tween runs in "unscaled time" so it doesnt freeze
            .OnComplete(() => {
                isPanelOpen = true;
            });

        // Pause the game time
        Time.timeScale = 0f;

        // Unlock cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ClosePanel()
    {
        // Kill any ongoing tween before starting a new one
        scaleTween?.Kill();

        scaleTween = Panel.transform
            .DOScale(Vector3.zero, scaleDuration)
            .SetEase(Ease.InBack, 1.2f)
            .SetUpdate(true) // Ensures tween runs in "unscaled time"
            .OnComplete(() =>
            {
                Panel.SetActive(false);
                isPanelOpen = false; // Set panel state to closed after animation
            });

        // Resume the game time
        Time.timeScale = 1f;

        // Hide cursor and lock it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
