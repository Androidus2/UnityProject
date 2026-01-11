using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSteal : MonoBehaviour
{
    InputAction stealAction;
    float stealValue;

    [SerializeField]
    float stealCooldown;

    [SerializeField]
    private GameObject stealTextObject; // The E to steal button

    private Tween stealTextTween;

    float timeSinceLastSteal;

    int itemsStolen = 0;

    List<EnemyController> targets;

    void Start()
    {
        stealAction = InputSystem.actions.FindAction("Interact");

        timeSinceLastSteal = stealCooldown;

        targets = new List<EnemyController>();

    }

    void Update()
    {
        stealValue = stealAction.ReadValue<float>();

        timeSinceLastSteal += Time.deltaTime;

        if (stealValue > 0f && timeSinceLastSteal >= stealCooldown && PlayerMechanicsUnlocker.Instance.IsMechanicUnlocked("Stealing"))
        {
            timeSinceLastSteal = 0f;
            Steal();
        }
    }

    void Steal()
    {
        if (targets.Count > 0)
        {
            EnemyController targetEnemy = targets[0];

            if (targetEnemy.GetIsStealable())
            {
                targetEnemy.MarkStolen();
                itemsStolen++; 

                Debug.Log("Player just stole! Number of stolen items: " + itemsStolen);

                targets.RemoveAt(0);

                if (targets.Count == 0)
                    DisableStealText();
            }
            else
            {
                Debug.Log("Enemy was already stolen from!");
            }
        }
    }

    void EnableStealText()
    {
        stealTextTween?.Kill();
        stealTextObject.SetActive(true);
        stealTextTween = stealTextObject.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
    }

    void DisableStealText()
    {
        stealTextTween?.Kill();
        stealTextTween = stealTextObject.transform.DOScale(0, 0.2f).SetEase(Ease.InBack).OnComplete(() => stealTextObject.SetActive(false));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy == null)
            {
                return;
            }
            Debug.Log("Enemy entered steal range.");
            targets.Add(enemy);

            if (targets.Count == 1)
                EnableStealText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy == null)
            {
                return;
            }

            targets.Remove(enemy);

            if (targets.Count == 0)
                DisableStealText();

        }
    }

}
