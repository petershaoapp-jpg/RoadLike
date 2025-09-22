using UnityEngine;

[RequireComponent(typeof(IDie))]
public class Health : MonoBehaviour
{
    // TODO (Peter and Jake) (choose whatever you guys want to work on):
    // - Create a public variable (with a private set value) called health
    // - Get the die script required by this script
    // - Create a Heal function that takes in an integer and adds that to the current health value
    // - Create a TakeDamage function that takes in an integer an removes that from the current health value
    // Then, check if health is <= 0 (the player has died), and then call the die script's OnDie function
}
