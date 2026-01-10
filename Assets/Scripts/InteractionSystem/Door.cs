using UnityEngine;

public class Door : InteractableBase
{
    [Header("Setari")]
    [SerializeField] bool isLocked = true;

    [Header("Referinte")]
    [SerializeField] Animator animator;
    [SerializeField] MonoBehaviour playerControllerScript;

    private bool isOpen = false;

    // Optional: Auto-find la start, la fel ca la chest, daca vrei
    private void Awake()
    {
        // if (playerControllerScript == null) playerControllerScript = FindFirstObjectByType<PlayerMovement>();
    }

    public override void Interact(Interactor interactor, InventoryObject inventory)
    {
        // 1. Daca usa e deja deschisa, nu facem nimic
        if (isOpen) return;

        // 2. Verificam starea
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
        if (playerControllerScript != null) playerControllerScript.enabled = false;

        // B. Apelam Minigame-ul Singleton
        LockPickingMinigame.Instance.StartMinigame(HandleMinigameResult);
    }

    // Callback-ul care se executa cand se termina minigame-ul
    void HandleMinigameResult(bool success)
    {
        // 1. Repornim jocul (Timpul si Player-ul)
        Time.timeScale = 1f;
        if (playerControllerScript != null) playerControllerScript.enabled = true;

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
        isOpen = true;

        // Declansam animatia
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }
    }
}