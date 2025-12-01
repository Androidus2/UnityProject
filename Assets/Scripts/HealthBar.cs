using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Gradient gradient;

    [SerializeField]
    private Image fill;

    [SerializeField]
    private float tweenDuration = 0.25f;

    private Tweener healthTween;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        // Kill any running tween so quick hits don't conflict
        healthTween?.Kill();

        float targetValue = Mathf.Clamp(health, slider.minValue, slider.maxValue);

        healthTween = slider.DOValue(targetValue, tweenDuration)
            .SetEase(Ease.OutCubic)
            .OnUpdate(() =>
            {
                fill.color = gradient.Evaluate(slider.normalizedValue);
            });
    }
}
