using System.Collections.Generic;
using UnityEngine;

public class UpgradeEffects : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    public List<string> upgradeNames;

    private void Awake()
    {
        // Base stats
        playerData.maxSpeed = 30;
        playerData.maxHealth = 20;
        playerData.nitroReplenishTime = 10;
        playerData.maxNitros = 3;
        playerData.critChance = 20;
        playerData.critDamage = 2;
        playerData.attack = 5;
        playerData.speed = 0;
        
        upgradeNames = playerData.upgrades.ConvertAll(data => data.name);

        if (upgradeNames.Contains("Big Gun"))
        {
            playerData.attack += 2;
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
        
        // Late trigger
        if (upgradeNames.Contains("Horseshoe"))
        {
            playerData.speed += playerData.luck;
        }
        
        // Last trigger
        if (upgradeNames.Contains("Lust"))
        {
            playerData.critChance /= 2;
            playerData.critDamage += 500;
        }
    }
}
