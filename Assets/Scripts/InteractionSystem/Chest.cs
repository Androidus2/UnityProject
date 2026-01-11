using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(InventoryObject))]
public class Chest : InteractableBase
{
    [SerializeField] GameObject panel;
    [SerializeField] bool isLocked = false;

    private InventoryObject chestInventory;
    private DisplayInventory displayInventory;
    private Tweener scaleTween;

    protected override void Awake()
    {
        base.Awake();
        displayInventory = panel.GetComponent<DisplayInventory>();
        chestInventory = GetComponent<InventoryObject>();
        chestInventory.SetType(InventoryType.Chest);
    }

    public override void Interact(Interactor interactor, InventoryObject inventory)
    {
        if (isLocked)
        {
            // Pornim lockpick-ul
            StartLockpicking();
        }
        else
        {
            TogglePanel();
        }
    }

    void StartLockpicking()
    {
        if (LockPickingMinigame.Instance == null) return;

        // Oprim timpul si player-ul
        Time.timeScale = 0f;
        PanelManager.GetInstance().SetLockPickPanelState(true);

        // Apelam Singleton-ul si ii dam functia noastra de raspuns
        LockPickingMinigame.Instance.StartMinigame(HandleMinigameResult);
    }

    void HandleMinigameResult(bool success)
    {
        // 1. Repornim jocul
        Time.timeScale = 1f;
        PanelManager.GetInstance().SetLockPickPanelState(false);

        // 2. Verificam daca a castigat
        if (success)
        {
            Debug.Log("LOCKPICK REUSIT! Se deschide...");
            isLocked = false; // Descuiem permanent
            OpenPanel();      // Deschidem inventarul imediat
        }
        else
        {
            Debug.Log("Lockpick esuat.");
        }
    }

    void TogglePanel() { if (PanelManager.GetInstance().IsChestPanelOpen()) ClosePanel(); else OpenPanel(); }

    void OpenPanel()
    {
        displayInventory.SetInventoryObject(chestInventory);
        scaleTween?.Kill();
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;
        scaleTween = panel.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack, 1.4f).SetUpdate(true).OnComplete(() => { PanelManager.GetInstance().SetChestPanelState(true); });

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ClosePanel()
    {
        PanelManager.GetInstance().SetChestPanelState(false);
        scaleTween?.Kill();
        scaleTween = panel.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack, 1.2f).SetUpdate(true).OnComplete(() => { panel.SetActive(false); });

        if (PanelManager.GetInstance().AreAllPanelsClosed())
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}