using System.Collections.Generic;
using UnityEngine;

public class UpgradeEffects : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    public List<string> upgradeNames;

    private void Awake()
    {
        playerData.maxSpeed = 30;
        playerData.maxHealth = 20;
        playerData.nitroReplenishTime = 10;
        playerData.maxNitros = 3;
        playerData.critChance = 20;
        playerData.critDamage = 2;
        
        upgradeNames = playerData.upgrades.ConvertAll(data => data.name);

        if (upgradeNames.Contains("Bad Omen"))
        {
            playerData.critDamage += .1f;
        }

        if (upgradeNames.Contains("Worse Omen"))
        {
            playerData.critDamage += .1f;
        }

        if (upgradeNames.Contains("Herald"))
        {
            playerData.critDamage += .1f;
        }
        
        if (upgradeNames.Contains("Harbinger"))
        {
            playerData.critDamage += .1f;
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
    }
}
