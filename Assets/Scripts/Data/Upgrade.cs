using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
  public string upgradeName;
  public Rarity rarity;
  public int priority; // THIS IS IMPORTANT. Please for the love of god set this number higher if you want an upgrade to trigger last. Please.
  public int maxNitros;
  public float nitroReplenishTime;
  public float acceleration;
  public float maxSpeed;
  public int maxHealth;
  public Material cardImage;
  public string description;

  public List<Upgrade> prerequisites;
}
