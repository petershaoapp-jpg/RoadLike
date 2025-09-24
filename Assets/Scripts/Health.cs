using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(IDie))]
public class Health : MonoBehaviour
{
    // TODO (Peter and Jake) (choose whatever you guys want to work on):
    // DONE - Create a public variable (with a private set value) called health
    // DONE - Get the die script required by this script
    // DONE - Create a Heal function that takes in an integer and adds that to the current health value
    // - Create a TakeDamage function that takes in an integer and removes that from the current health value
    //   Then, check if health is <= 0 (the player has died), and then call the die script's OnDie function

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

    // public void TakeDamage(...
}
