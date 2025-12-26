using DG.Tweening;
using UnityEngine;
public enum VendorType
{
    Blacksmith,
    Armourer,
    Pharmacist
}

[RequireComponent(typeof(InventoryObject))]
public class Vendor : InteractableBase
{
    //inventory panel 
    [SerializeField]
    private GameObject panel;

    private InventoryObject vendorInventory;

    private DisplayInventory displayInventory;

    [SerializeField]
    protected VendorType vendorType;

    Tweener scaleTween;
    [SerializeField]
    private float scaleDuration = 0.15f;

    private GameObject background = null;


    protected override void Awake()
    {
        base.Awake();
        displayInventory = panel.GetComponent<DisplayInventory>();
        vendorInventory = GetComponent<InventoryObject>();
        vendorInventory.SetType(InventoryType.Vendor); //for safety
        vendorInventory.SetSize(5);
    }

    public override void Interact(Interactor interactor, InventoryObject inventory)
    {
        //if vendor is available
        //opening inventory panel
        TogglePanel();

        Debug.Log("Opening vendor display");
    }

    void TogglePanel()
    {

        if (PanelManager.GetInstance().IsVendorPanelOpen())
            ClosePanel();
        else
            OpenPanel();
    }

    void OpenPanel()
    {
        displayInventory.SetInventoryObject(vendorInventory);

        // Kill any ongoing tween to avoid conflicts with animation
        scaleTween?.Kill();

        // Ensure panel is visible before animation
        panel.SetActive(true);

        //pick correct background based on vendor type
        //will have Blacksmith, Armourer and Pharmacist
        
        if (vendorType == VendorType.Pharmacist)
        {
            background = panel.transform.Find("PharmacistBackground").gameObject;
        }
        else if (vendorType == VendorType.Armourer)
        {
            background = panel.transform.Find("ArmourerBackground").gameObject;

        }
        else //default for now in case of anything
        {
            background = panel.transform.Find("BlacksmithBackground").gameObject;

        }
        background.SetActive(!background.gameObject.activeSelf);

        panel.transform.localScale = Vector3.zero;

        scaleTween = panel.transform
            .DOScale(Vector3.one, scaleDuration)
            .SetEase(Ease.OutBack, 1.4f)
            .SetUpdate(true) // Ensures tween runs in "unscaled time" so it doesnt freeze
            .OnComplete(() => {
                PanelManager.GetInstance().SetVendorPanelState(true);
            });

        // Pause the game time
        Time.timeScale = 0f;

        // Unlock cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ClosePanel()
    {
        PanelManager.GetInstance().SetVendorPanelState(false);

        // Kill any ongoing tween before starting a new one
        scaleTween?.Kill();

        scaleTween = panel.transform
            .DOScale(Vector3.zero, scaleDuration)
            .SetEase(Ease.InBack, 1.2f)
            .SetUpdate(true) // Ensures tween runs in "unscaled time"
            .OnComplete(() =>
            {
                background.SetActive(!background.gameObject.activeSelf);
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

    public VendorType GetVendorType()
    {
        return vendorType;
    }

}