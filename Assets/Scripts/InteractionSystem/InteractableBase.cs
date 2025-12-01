using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Outline))]
public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    private Outline outline;

    [SerializeField]
    private float targetWidth = 6f; // Default thickness

    [SerializeField]
    private float tweenDuration = 0.15f; // Quick grow effect

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.OutlineWidth = 0f; // Start with zero width
        outline.enabled = false;
    }

    public void ShowOutline(bool enable)
    {
        outline.enabled = true;

        if (enable)
        {
            // Only start tween if width is zero or near zero
            if (outline.OutlineWidth <= 0.01f)
            {
                DOTween.To(() => outline.OutlineWidth, x => outline.OutlineWidth = x, targetWidth, tweenDuration)
                       .SetEase(Ease.OutBack);
            }
        }
        else
        {
            DOTween.To(() => outline.OutlineWidth, x => outline.OutlineWidth = x, 0f, tweenDuration)
                   .SetEase(Ease.InBack)
                   .OnComplete(() => outline.enabled = false);
        }
    }



    public abstract void Interact(Interactor interactor, InventoryObject inventory);
}
