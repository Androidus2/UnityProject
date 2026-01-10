using UnityEngine;
using DG.Tweening;
using System;

public class Fade : MonoBehaviour
{
    [SerializeField] 
    private RectTransform fadeMover;
    [SerializeField] 
    private RectTransform canvasRect;
    [SerializeField] 
    private float duration = 0.4f;

    private Tween tween;
    private bool isFading;

    private Vector2 center = Vector2.zero;

    private Vector2 TopLeftOffscreen()
    {
        Vector2 size = canvasRect.rect.size;
        return new Vector2(-size.x, size.y);
    }

    private Vector2 BottomRightOffscreen()
    {
        Vector2 size = canvasRect.rect.size;
        return new Vector2(size.x, -size.y);
    }

    private void Awake()
    {
        fadeMover.anchoredPosition = TopLeftOffscreen();
    }

    private void Start()
    {
        isFading = false;
        EndFade();
    }

    public void BeginFade(Action onComplete = null)
    {
        if (isFading)
            return;

        isFading = true;
        KillTween();
        fadeMover.anchoredPosition = BottomRightOffscreen();
        fadeMover.gameObject.SetActive(true);
        tween = fadeMover
            .DOAnchorPos(center, duration)
            .OnComplete(() =>
            {
                isFading = false;
                onComplete?.Invoke();
            });
    }

    public void EndFade(Action onComplete = null)
    {
        if (isFading)
            return;

        isFading = true;
        KillTween();
        fadeMover.anchoredPosition = center;
        fadeMover.gameObject.SetActive(true);
        Vector2 targetPos = TopLeftOffscreen();

        tween = fadeMover
            .DOAnchorPos(targetPos, duration)
            .OnComplete(() =>
            {
                isFading = false;
                fadeMover.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }



    private void KillTween()
    {
        tween?.Kill();
    }
}
