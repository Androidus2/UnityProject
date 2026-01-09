using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Setari")]
    public bool isLocked = true;

    [Header("Referinte")]
    public Animator animator;
    public LockPickingMinigame minigame; // Referinta la scriptul minigame-ului

    private bool isOpen = false;

    public void Interact(Interactor interactor, InventoryObject inventory)
    {
        // 1. Daca usa e deja deschisa, nu facem nimic
        if (isOpen) return;

        // 2. Daca e incuiata
        if (isLocked)
        {
            if (minigame != null)
            {
                Debug.Log("Usa e incuiata. Pornesc minigame-ul...");

                // --- AICI ERA EROAREA ---
                // Nu apelam StartMinigame, ci doar activam obiectul vizual
                minigame.gameObject.SetActive(true);

                // Ii trimitem inventarul jucatorului
                minigame.SetInventory(inventory);

                // Ne abonam la evenimentul de final (Win/Lose)
                // (Intai scoatem abonarea veche ca sa fim siguri, apoi o punem pe cea noua)
                minigame.OnFinished -= HandleMinigameResult;
                minigame.OnFinished += HandleMinigameResult;
            }
            else
            {
                Debug.LogError("LIPSA: Nu ai pus scriptul LockPickingMinigame pe Ușă in Inspector!");
            }
        }
        else
        {
            // 3. Daca nu e incuiata, o deschidem
            OpenDoor();
        }
    }

    // Aceasta functie se apeleaza automat cand termini minigame-ul
    private void HandleMinigameResult(bool success)
    {
        // Ne dezabonam
        minigame.OnFinished -= HandleMinigameResult;

        // Ascundem minigame-ul la loc
        minigame.gameObject.SetActive(false);

        if (success)
        {
            Debug.Log("Ai descuiat usa!");
            isLocked = false; // Usa acum e descuiata permanent
            OpenDoor();
        }
        else
        {
            Debug.Log("Ai esuat. Mai incearca.");
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