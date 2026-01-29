using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeSelector : MonoBehaviour
{
    public List<Upgrade> upgrades;

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
        float rareThreshold = 30 * rarityBonusPerentage;
        float epicThreshold = 20 * rarityBonusPerentage;
        float legendaryThreshold = 15 * rarityBonusPerentage;
        float arcaneThreshold = 5 * rarityBonusPerentage;

        Rarity rarity;

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

        List<Upgrade> pool = GetAvailableUpgrades().Where(u => u.rarity == rarity).ToList();


        int index = Random.Range(0,pool.Count);

        return pool[index];
    }
}