using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSteal : MonoBehaviour
{
    InputAction stealAction;
    float stealValue;

    [SerializeField]
    float stealCooldown; 

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

        if (stealValue > 0f && timeSinceLastSteal >= stealCooldown)
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
            }
            else
            {
                Debug.Log("Enemy was already stolen from!");
            }
        }
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
        }
    }

}
