using System.Collections.Generic;
using UnityEngine;

public class UpgradeEffects : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    public List<string> upgradeNames;

    private void Awake()
    {
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
    }
}
