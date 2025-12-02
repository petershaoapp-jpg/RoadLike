using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int maxNitros = 3;
    public float nitroReplenishTime = 10;
    public float maxSpeed = 30;
    public int maxHealth = 100;

    public List<Upgrade> upgrades;
}
