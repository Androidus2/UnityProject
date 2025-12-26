using UnityEngine;
public class PanelManager : MonoBehaviour
{
    private static PanelManager instance;
    private void Awake()
    {
        instance = this;
        Debug.Log("PanelManager instance assigned");
    }

    private bool isPauseMenuOpen = false;
    private bool isChestPanelOpen = false;
    private bool isInventoryPanelOpen = false;
    private bool isVendorPanelOpen = false;

    public static PanelManager GetInstance()
    {
        return instance;
    }


    public bool IsPauseMenuOpen()
    {
        return isPauseMenuOpen;
    }
    public void SetPauseMenuState(bool isOpen)
    {
        isPauseMenuOpen = isOpen;
    }


    public bool IsChestPanelOpen()
    {
        return isChestPanelOpen;
    }
    public void SetChestPanelState(bool isOpen)
    {
        isChestPanelOpen = isOpen;
    }


    public bool IsInventoryPanelOpen()
    {
        return isInventoryPanelOpen;
    }
    public void SetInventoryPanelState(bool isOpen)
    {
        isInventoryPanelOpen = isOpen;
    }

    public bool IsVendorPanelOpen()
    {
        return isVendorPanelOpen;
    }
    public void SetVendorPanelState(bool isOpen)
    {
        isVendorPanelOpen = isOpen;
    }

    public bool AreAllPanelsClosed()
    {
        return !isPauseMenuOpen && !isChestPanelOpen && !isInventoryPanelOpen && !isVendorPanelOpen;
    }

}