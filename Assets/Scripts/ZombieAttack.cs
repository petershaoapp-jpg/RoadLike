using UnityEngine;

public class ZombieAttack : MonoBehaviour
{[Header("Attack Settings")]
    [Tooltip("The amount of damage dealt to the car on impact.")]
    [SerializeField] private int damageAmount = 1;

    // This function is automatically called by Unity upon a physics collision
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object we collided with is the player's car
        if (collision.gameObject.name == "Car")
        {
            // Attempt to retrieve the Health component from the car
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            
            // If the car has a Health component, deal damage to it
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log("Zombie dealt damage to the Car!");
            }
        }
    }
}