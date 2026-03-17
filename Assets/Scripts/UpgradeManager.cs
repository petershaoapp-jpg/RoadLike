using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
  [SerializeField] private PlayerData data;

  private void Start()
  {
    foreach (Upgrade upgrade in data.upgrades) {
      data.maxNitros += upgrade.maxNitros;
      data.nitroReplenishTime *= upgrade.nitroReplenishTime;
      data.maxSpeed += upgrade.maxSpeed;
      data.maxHealth += upgrade.maxHealth;
    }
  }
}
