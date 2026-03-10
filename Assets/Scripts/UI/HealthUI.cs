using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health health;

    private void Update()
    {
        float scale = health.health / health.maxHealth;
        transform.localScale = new Vector3(scale, 1, 1);
    }
}