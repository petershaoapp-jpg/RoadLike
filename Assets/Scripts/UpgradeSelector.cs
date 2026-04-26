using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeSelector : MonoBehaviour
{
    public List<Upgrade> upgrades;
    public List<Upgrade> shown;
    
    [SerializeField] private PlayerData _playerData;

    public List<Upgrade> GetAvailableUpgrades()
    {
        HashSet<Upgrade> owned = _playerData.upgrades.ToHashSet();


        List<Upgrade> filtered = upgrades.Where(u => u.prerequisites.ToHashSet().IsSubsetOf(owned)).ToList();
        filtered = filtered.Where(u => !owned.Contains(u)).ToList();

        return filtered;
    }

    public Upgrade SelectUpgrade(int bonusRarityChance)
    {
        float rarityBonusPerentage = bonusRarityChance / 100;

        float random = Random.Range(0,100);

        // Rarity thresholds
        float rareThreshold = 10 * rarityBonusPerentage;
        float epicThreshold = 5 * rarityBonusPerentage;
        float legendaryThreshold = rarityBonusPerentage;
        float arcaneThreshold = 0.2f * rarityBonusPerentage;

        Rarity rarity;
        
        Debug.Log("[UPGRADES]: Rarity bonus %: " + rarityBonusPerentage);
        Debug.Log("[UPGRADES] Random #:" +  random);

        if (random < arcaneThreshold)
        {
            rarity = Rarity.Arcane;
        } else if (random < legendaryThreshold)
        {
            rarity = Rarity.Legendary;
        } else if (random < epicThreshold)
        {
            rarity = Rarity.Epic;
        } else if (random < rareThreshold)
        {
            rarity = Rarity.Rare;
        } else
        {
            rarity = Rarity.Common;
        }

        Debug.Log(rarity);
        
        List<Upgrade> pool = GetAvailableUpgrades().Where(u => u.rarity == rarity).ToList();

        int index = Random.Range(0,pool.Count);

        if (index < pool.Count && !shown.Contains(pool[index]))
        {
            shown.Add(pool[index]);
            return pool[index];
        }

        if (GetAvailableUpgrades().Count == 0) return null;
        
        return SelectUpgrade(bonusRarityChance);
    }
}