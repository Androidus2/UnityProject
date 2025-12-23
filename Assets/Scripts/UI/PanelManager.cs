using UnityEngine;
public class PanelManager : MonoBehaviour
{
    public static PanelManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    public bool isPauseMenuOpen = false;
    public bool isChestPanelOpen = false;
    public bool isInventoryPanelOpen = false;
}