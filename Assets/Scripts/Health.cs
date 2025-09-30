using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(IDie))]
public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100; // for customizablility
    public float health { get; private set; } // public to get, not public to set

    private IDie dieScript;


    private void Awake()
    {
        health = maxHealth;
        dieScript = GetComponent<IDie>();
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
            dieScript.OnDie();
        }
    }
}
