using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
  public string upgradeName;
  public Rarity rarity;  
  public int maxNitros;
  public float nitroReplenishTime;
  public float acceleration;
  public float maxSpeed;
  public int maxHealth;

  public Upgrade[] prerequisites;
}
