using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
       public void Interact(Interactor interactor)
    {
        //lockpicking mechanic
        //open the chest ui
        Debug.Log("Opening chest");
    }
}
