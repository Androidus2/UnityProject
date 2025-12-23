using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopUpPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;
    private InputAction inventoryButton;

    [SerializeField]
    private InventoryObject playerInventory;

    Tweener scaleTween;
    [SerializeField]
    private float scaleDuration = 0.15f;

    private DisplayInventory displayInventory;



    void Awake()
    {
        inventoryButton = InputSystem.actions.FindAction("Inventory");
        // TODO: Make this unsubscribe when the scene is changed so that we don't constantly get errors whenever opening the inventory after being killed
        inventoryButton.performed += ctx => TogglePanel();

        displayInventory = panel.GetComponent<DisplayInventory>();
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
        
        if (PanelManager.instance.isInventoryPanelOpen)
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
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero; 

        scaleTween = panel.transform
            .DOScale(Vector3.one, scaleDuration)
            .SetEase(Ease.OutBack, 1.4f)
            .SetUpdate(true) // Ensures tween runs in "unscaled time" so it doesnt freeze
            .OnComplete(() => {
                PanelManager.instance.isInventoryPanelOpen = true;
            });

        // Pause the game time
        Time.timeScale = 0f;

        // Unlock cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ClosePanel()
    {
        PanelManager.instance.isInventoryPanelOpen = false;
        
        // Kill any ongoing tween before starting a new one
        scaleTween?.Kill();

        scaleTween = panel.transform
            .DOScale(Vector3.zero, scaleDuration)
            .SetEase(Ease.InBack, 1.2f)
            .SetUpdate(true) // Ensures tween runs in "unscaled time"
            .OnComplete(() =>
            {
                panel.SetActive(false);
            });

        //we resume time ONLY if all panels are closed
        if (!(PanelManager.instance.isPauseMenuOpen || PanelManager.instance.isChestPanelOpen))
        {
            // Resume the game time
            Time.timeScale = 1f;

            // Hide cursor and lock it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }
}
