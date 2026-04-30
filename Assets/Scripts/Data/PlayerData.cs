using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    // MISC DATA //
    public List<Upgrade> upgrades;   
    public int level = 1;

    // UPGRADABLE STATS //

    // Player stats
    public int maxHealth = 20;
    // TODO: Add regeneration (regenerates n% of health per second)
    public int luck = 0;
    
    // Car stats
    public float maxSpeed = 45;
    public float speed = 30;
    public float acceleration = 3.5f;
    public float handling = 80;
    // TODO: Add "handling" (how easy it is to control car)

    // Nitro stats
    public int maxNitros = 3;
    public float nitroReplenishTime = 10;
    
    // Damage stats
    public float attack = 5;
    public float critChance = 20;
    public float critDamage = 2;
}