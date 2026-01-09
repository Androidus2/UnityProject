using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] bool isLocked = true;
    [SerializeField] GameObject lockpickingMinigamePrefab; // Aici pui Prefab-ul cu Parent

    public void Interact(Interactor interactor, InventoryObject inventory)
    {
        // lockpicking mechanic
        if (isLocked)
        {
            StartLockpicking(inventory);
        }
        else
        {
            // open the chest ui
            Debug.Log("Opening chest (Already Unlocked)");
        }
    }

    void StartLockpicking(InventoryObject inventory)
    {
        // Instantiem tot Parent-ul (care contine camera, luminile, minigame-ul etc)
        GameObject minigameInstance = Instantiate(lockpickingMinigamePrefab);

        // --- MODIFICARE IMPORTANTA ---
        // Folosim GetComponentInChildren pentru ca scriptul LockPickingMinigame 
        // este probabil pe un copil al obiectului instantiat, nu pe Parent.
        LockPickingMinigame minigameScript = minigameInstance.GetComponentInChildren<LockPickingMinigame>();

        if (minigameScript != null)
        {
            // Trimitem inventarul catre minigame
            minigameScript.SetInventory(inventory);

            // Ne abonam la eveniment ca sa stim cand s-a terminat
            minigameScript.OnFinished += (success) =>
            {
                Destroy(minigameInstance); // Distrugem minigame-ul
                if (success)
                {
                    isLocked = false;
                    Debug.Log("Chest Unlocked!");
                }
            };
        }
        else
        {
            Debug.LogError("CRITIC: Nu am gasit scriptul LockPickingMinigame in prefab-ul instantiat!");
        }
    }
}