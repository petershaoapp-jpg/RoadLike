using UnityEngine;

[RequireComponent(typeof(IDie))]
public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100; // for customizablility
    [SerializeField] private PlayerData data;
    public float health { get; private set; } // public to get, not public to set

    private IDie _dieScript;


    private void Awake()
    {
        health = maxHealth;
        _dieScript = GetComponent<IDie>();
        
        if (gameObject.name == "Car") {
          maxHealth =  data.maxHealth;
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        health -= amount;
        if (health < 0)
        {
            health = 0;
            _dieScript.OnDie();
        }
    }
}
