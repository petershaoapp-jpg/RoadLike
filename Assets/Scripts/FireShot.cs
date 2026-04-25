using UnityEngine;

public class FireShot : MonoBehaviour
{
    [HideInInspector] public int damage = 2;
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        // Auto-destroy after lifetime so stray bullets don't pile up
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only damage the player car
        if (collision.gameObject.name == "Car")
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("[FIRESHOT]: Hit player for " + damage + " damage!");
            }
        }

        // Destroy bullet on any collision (hit player, hit ground, hit anything)
        Destroy(gameObject);
    }
}
