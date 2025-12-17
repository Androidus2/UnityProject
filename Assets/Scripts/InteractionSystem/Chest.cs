using DG.Tweening;
using UnityEngine;

public class Chest : InteractableBase
{
    //inventory panel 
    [SerializeField]
    private GameObject Panel;

    //seeding for each chest respectively? tbd how we approach this for multiple chests
    [SerializeField]
    private InventoryObject chestInventory;

    private DisplayInventory displayInventory;

    Tweener scaleTween;
    [SerializeField]
    private float scaleDuration = 0.15f;

    private bool isPanelOpen = false;


    public void Awake()
    {
        displayInventory = Panel.GetComponent<DisplayInventory>();
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
        if (isPanelOpen)
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
