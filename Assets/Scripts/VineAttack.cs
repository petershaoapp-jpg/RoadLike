using UnityEngine;

public class VineAttack : MonoBehaviour
{
    [HideInInspector] public int damage = 20;
    [HideInInspector] public float knockbackForce = 20f;

    private bool _hasHitPlayer = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Only damage the player car, and only once per vine
        if (_hasHitPlayer) return;
        if (collision.gameObject.name != "Car") return;

        _hasHitPlayer = true;

        // Deal damage
        Health playerHealth = collision.gameObject.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("[VINE ATTACK]: Hit player for " + damage + " damage!");
        }

        // Knockback: launch the car upward and away from the vine
        Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 knockDirection = (collision.transform.position - transform.position).normalized;
            knockDirection.y = 0;
            knockDirection += Vector3.up * 0.5f; // Strong upward launch since it erupts from below
            playerRb.AddForce(knockDirection * knockbackForce, ForceMode.Impulse);
            Debug.Log("[VINE ATTACK]: Player knocked back!");
        }
    }
}
