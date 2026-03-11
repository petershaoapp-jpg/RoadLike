using System;
using System.Collections;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("The amount of damage dealt to the car on impact.")]
    [SerializeField] private int damageAmount = 1;

    private Health _playerHealth;

    private bool _canAttackPlayer;
    
    // This function is automatically called by Unity upon a physics collision
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object we collided with is the player's car
        if (collision.gameObject.name == "Car")
        {
            _canAttackPlayer = true;
            // Attempt to retrieve the Health component from the car
            _playerHealth = collision.gameObject.GetComponent<Health>();
            
            // If the car has a Health component, deal damage to it
            if (_playerHealth != null)
            {
                StartCoroutine(DamageRoutine());
                Debug.Log("Zombie dealt damage to the Car!");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Car")
        {
            _canAttackPlayer = false;
        }
    }

    private IEnumerator DamageRoutine()
    {
        if (_canAttackPlayer)
        {
            _playerHealth.TakeDamage(damageAmount);
            yield return new WaitForSeconds(1);
            StartCoroutine(DamageRoutine());
        }
    }
}