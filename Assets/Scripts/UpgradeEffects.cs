using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeEffects : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    private Health _health;

    public List<string> upgradeNames;
    
    private float baseDamage;

    private void Awake()
    {
        _health = GetComponent<Health>();
        
        // Base stats
        playerData.maxSpeed = 45;
        playerData.maxHealth = 20;
        playerData.nitroReplenishTime = 10;
        playerData.maxNitros = 3;
        playerData.critChance = 20;
        playerData.critDamage = 2;
        playerData.attack = 5;
        playerData.speed = 0;
        playerData.handling = 80;
        playerData.acceleration = 3.5f;
        playerData.luck = 0;
        
        upgradeNames = playerData.upgrades.ConvertAll(data => data.name);

        if (upgradeNames.Contains("Big Gun"))
        {
            playerData.attack += 2;
        }

        if (upgradeNames.Contains("Huge Gun"))
        {
            playerData.attack += 3;
        }
        
        if (upgradeNames.Contains("Enormous Gun"))
        {
            playerData.attack += 3;
        }
        
        if (upgradeNames.Contains("Gun of the Titans"))
        {
            playerData.attack += 3;
        }

        if (upgradeNames.Contains("Vitamin A"))
        {
            playerData.maxHealth += 5;
        }
        
        if (upgradeNames.Contains("Vitamin B"))
        {
            playerData.maxHealth += 5;
        }
        
        if (upgradeNames.Contains("Vitamin C"))
        {
            playerData.maxHealth += 5;
        }
        
        if (upgradeNames.Contains("Vitamin X"))
        {
            playerData.attack += playerData.maxHealth - 20;
            playerData.maxHealth = 20;
        }

        if (upgradeNames.Contains("Better Wheels"))
        {
            playerData.acceleration += 1;
            playerData.maxSpeed += 10;
        }
        
        if (upgradeNames.Contains("Bad Omen"))
        {
            playerData.critDamage += 10;
        }

        if (upgradeNames.Contains("Worse Omen"))
        {
            playerData.critDamage += 10;
        }

        if (upgradeNames.Contains("Herald"))
        {
            playerData.critDamage += 10;
        }
        
        if (upgradeNames.Contains("Harbinger"))
        {
            playerData.critDamage += 10;
        }

        if (upgradeNames.Contains("Sharp Shooter"))
        {
            playerData.critChance += 5;
        }

        if (upgradeNames.Contains("Sniper"))
        {
            playerData.critChance += 10;
        }

        if (upgradeNames.Contains("Marksman"))
        {
            playerData.critChance += 15;
        }

        if (upgradeNames.Contains("William Tell"))
        {
            playerData.critChance += 20;
        }

        if (upgradeNames.Contains("Four-leaf Clover"))
        {
            playerData.luck += 5;
        }

        if (upgradeNames.Contains("Jackpot!"))
        {
            playerData.critChance += 7;
            playerData.critDamage += 7;
            playerData.luck += 7;
        }

        if (upgradeNames.Contains("Nitro+"))
        {
            playerData.maxNitros += 1;
        }
        
        // Late trigger
        if (upgradeNames.Contains("Horseshoe"))
        {
            playerData.maxSpeed += playerData.luck;
        }
        
        // Last trigger
        if (upgradeNames.Contains("Lust"))
        {
            playerData.critChance /= 2;
            playerData.critDamage += 500;
        }
        
        if (upgradeNames.Contains("Greed"))
        {
            playerData.attack += 50;
            StartCoroutine(GreedRoutine());
        }

        if (upgradeNames.Contains("Wrath"))
        {
            playerData.attack *= 1.25f;
            playerData.maxHealth /= 2;
        }

        if (upgradeNames.Contains("Pride"))
        {
            playerData.maxSpeed = 10000; // Good luck
        }

        if (upgradeNames.Contains("Sloth"))
        {
            playerData.maxSpeed = 0;
            playerData.nitroReplenishTime /= 2;
            playerData.maxHealth *= 2;
        }
        
        baseDamage = playerData.attack;
    }

    private void Update()
    {
        if (!upgradeNames.Contains("Envy")) return;

        float envyDamage = GameObject.FindGameObjectsWithTag("Enemy").Length;

        playerData.attack = baseDamage + envyDamage;
    }
    
    
    private IEnumerator GreedRoutine()
    {
        yield return new WaitForSeconds(1);
        float dmg =  (float) playerData.maxHealth / 100 * 1.5f;
        _health.TakeDamage(dmg);
        StartCoroutine(GreedRoutine());
    }
}
