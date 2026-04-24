using System;
using UnityEngine;

public class Soul : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Car")
        {
            GameObject car = other.gameObject;
            Health playerHealth = car.GetComponent<Health>();
            
            playerHealth.Heal(playerHealth.maxHealth / 10);
            
            Destroy(gameObject);
        }
    }
}
