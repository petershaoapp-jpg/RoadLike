using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    // MISC DATA //
    public List<Upgrade> upgrades;   


    // UPGRADABLE STATS //

    // Player stats
    public int maxHealth = 100;
    // TODO: Add regeneration (regenerates n% of health per second)
    public int luck = 0;

    // Car stats
    public float maxSpeed = 30;
    // TODO: Add "handling" (how easy it is to control car)

    // Nitro stats
    public int maxNitros = 3;
    public float nitroReplenishTime = 10;
}