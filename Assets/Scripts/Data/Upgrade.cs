using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
  public string upgradeName;
  public Rarity rarity;
  public Material cardImage;
  public string description;

  public List<Upgrade> prerequisites;
}
