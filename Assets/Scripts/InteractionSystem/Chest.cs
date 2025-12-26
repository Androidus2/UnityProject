using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(InventoryObject))]
public class Chest : InteractableBase
{
    //inventory panel 
    [SerializeField]
    private GameObject panel;

    private InventoryObject chestInventory;

    private DisplayInventory displayInventory;

    Tweener scaleTween;
    [SerializeField]
    private float scaleDuration = 0.15f;


    protected override void Awake()
    {
        base.Awake();
        displayInventory = panel.GetComponent<DisplayInventory>();
        chestInventory = GetComponent<InventoryObject>();
        chestInventory.SetType(InventoryType.Chest); //for safety
        chestInventory.SetSize(15);
    }

    public override void Interact(Interactor interactor, InventoryObject inventory)
    {
        //if chest is unlocked
        //opening inventory panel
        
        TogglePanel();

        Debug.Log("Opening chest");
    }

    void TogglePanel()
    {
        
        if (PanelManager.GetInstance().IsChestPanelOpen())
            ClosePanel();
        else
            OpenPanel();
    }

    void OpenPanel()
    {
        displayInventory.SetInventoryObject(chestInventory);

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
                PanelManager.GetInstance().SetChestPanelState(true);
            });

        // Pause the game time
        Time.timeScale = 0f;

        // Unlock cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ClosePanel()
    {
        PanelManager.GetInstance().SetChestPanelState(false);

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
        if (PanelManager.GetInstance().AreAllPanelsClosed())
        {
            // Resume the game time
            Time.timeScale = 1f;

            // Hide cursor and lock it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }
}
