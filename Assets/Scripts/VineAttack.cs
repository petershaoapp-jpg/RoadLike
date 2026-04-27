using UnityEngine;

public class VineAttack : MonoBehaviour
{
    [HideInInspector] public int damage = 20;
    [HideInInspector] public float knockbackForce = 20f;

    private bool _hasHitPlayer = false;

    private void Start()
    {
        // Mark as trigger so OnTriggerEnter fires even when spawned overlapping
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only damage the player car, and only once per vine
        if (_hasHitPlayer) return;
        if (other.gameObject.name != "Car") return;

        _hasHitPlayer = true;

        // Deal damage
        Health playerHealth = other.gameObject.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("[VINE ATTACK]: Hit player for " + damage + " damage!");
        }

        // Knockback: directly set velocity to launch the car
        // Using linearVelocity instead of AddForce because WheelCollider physics overrides forces
        Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 knockDirection = (other.transform.position - transform.position).normalized;
            knockDirection.y = 0;
            knockDirection += Vector3.up * 0.5f;
            playerRb.linearVelocity = knockDirection.normalized * knockbackForce;
            Debug.Log("[VINE ATTACK]: Player knocked back!");
        }
    }
}
