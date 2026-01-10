using System.Collections;
using UnityEngine;

// When the player interacts with this object, they will get a new dialog piece
public class DialogPieceCollector : InteractableBase
{
    [SerializeField]
    private DialogPiece piece;

    private Dialogue dialog;

    private BoxCollider boxCollider;
    private Animator anim;

    protected override void Awake()
    {
        base.Awake();
        dialog = FindFirstObjectByType<Dialogue>();

        if (!dialog)
            Debug.LogError("DialogPieceCollector hasn't found a Dialogue component in the current scene!");

        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
    }

    public override void Interact(Interactor interactor, InventoryObject inventory)
    {
        dialog.SetDialogPiece(piece);
        boxCollider.enabled = false;
        anim.SetTrigger("Collect");

        Destroy(gameObject, 2f);
    }
}
