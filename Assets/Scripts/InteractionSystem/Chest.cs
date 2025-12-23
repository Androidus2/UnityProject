using DG.Tweening;
using UnityEngine;

public class Chest : InteractableBase
{
    //inventory panel 
    [SerializeField]
    private GameObject panel;

    //seeding for each chest respectively? tbd how we approach this for multiple chests
    //TO DO  - possible solution - change into normal class, mark as System.Serializable and edit for each chest
    [SerializeField]
    private InventoryObject chestInventory;

    private DisplayInventory displayInventory;

    Tweener scaleTween;
    [SerializeField]
    private float scaleDuration = 0.15f;


    protected override void Awake()
    {
        base.Awake();
        displayInventory = panel.GetComponent<DisplayInventory>();
    }


    public void Start()
    {
        //for testing purposes only
        //initializing chest inventory with items
        var med = Resources.Load<ItemObject>("Items/Medicine");
        chestInventory.GetItems().Clear();
        chestInventory.AddItem(med);
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
        
        if (PanelManager.instance.isChestPanelOpen)
            ClosePanel();
        else
            OpenPanel();
    }

    void OpenPanel()
    {
        //this script is specifically for the player's inventory
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
                PanelManager.instance.isChestPanelOpen = true;
            });

        // Pause the game time
        Time.timeScale = 0f;

        // Unlock cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ClosePanel()
    {
        PanelManager.instance.isChestPanelOpen = false;
        
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
        if (!(PanelManager.instance.isPauseMenuOpen || PanelManager.instance.isInventoryPanelOpen))
        {
            // Resume the game time
            Time.timeScale = 1f;

            // Hide cursor and lock it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }
}
