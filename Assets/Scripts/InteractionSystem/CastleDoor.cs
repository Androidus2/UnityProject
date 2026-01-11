using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CastleDoor : InteractableBase
{
    [SerializeField] bool isLocked = true;

    [SerializeField]
    GameObject OpenDoorObject;

    [SerializeField]
    GameObject CloseDoorObject;


    protected override void Awake()
    {
        base.Awake();
        //make sure the door is closed at start
        OpenDoorObject.SetActive(false);
        CloseDoorObject.SetActive(true);
    }

    public override void Interact(Interactor interactor, InventoryObject inventory)
    {
        

        //Verificam starea
        if (isLocked)
        {
            StartLockpicking();
        }
        else
        {
            OpenDoor();
        }
    }

    void StartLockpicking()
    {
        // Safety check simplu, exact ca la Chest
        if (LockPickingMinigame.Instance == null) return;

        // A. Oprim timpul si jucatorul
        Time.timeScale = 0f;
        PanelManager.GetInstance().SetLockPickPanelState(true);

        // B. Apelam Minigame-ul Singleton
        LockPickingMinigame.Instance.StartMinigame(HandleMinigameResult);
    }

    // Callback-ul care se executa cand se termina minigame-ul
    void HandleMinigameResult(bool success)
    {
        // 1. Repornim jocul (Timpul si Player-ul)
        Time.timeScale = 1f;
        PanelManager.GetInstance().SetLockPickPanelState(false);

        // 2. Verificam rezultatul
        if (success)
        {
            Debug.Log("USA DESCUIATA!");
            isLocked = false; // Usa ramane descuiata permanent
            OpenDoor();
        }
        else
        {
            Debug.Log("Lockpick esuat.");
            // Usa ramane incuiata, nu facem nimic altceva
        }
    }

    private void OpenDoor()
    {
        OpenDoorObject.SetActive(true);
        CloseDoorObject.SetActive(false);
    }
}