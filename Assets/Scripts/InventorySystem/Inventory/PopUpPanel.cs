using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PopUpPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject Panel;
    InputAction inventoryButton;

    Tweener scaleTween;
    [SerializeField]
    private float scaleDuration = 0.15f;

    void Awake()
    {
        inventoryButton = InputSystem.actions.FindAction("Inventory");
        inventoryButton.performed += ctx => TogglePanel();
    }

    void OnEnable()
    {
        inventoryButton.Enable();
    }

    void OnDisable()
    {
        inventoryButton.Disable();
    }

    void TogglePanel()
    {
        bool shouldOpen = !Panel.activeSelf;

        // Kill any ongoing tween so spamming doesn't break it
        scaleTween?.Kill();

        if (shouldOpen)
        {
            // Ensure panel is visible before animation
            Panel.SetActive(true);
            Panel.transform.localScale = Vector3.zero;

            scaleTween = Panel.transform
                .DOScale(Vector3.one, scaleDuration)
                .SetEase(Ease.OutBack, 1.4f);
        }
        else
        {
            scaleTween = Panel.transform
                .DOScale(Vector3.zero, scaleDuration)
                .SetEase(Ease.InBack, 1.2f)
                .OnComplete(() =>
                {
                    Panel.SetActive(false);
                });
        }
    }
}
