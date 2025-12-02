using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
  [SerializeField] private PlayerData data;

  private void Start()
  {
    data.maxSpeed = 30;
    data.maxHealth = 100;
    data.nitroReplenishTime = 10;
    data.maxNitros = 3;

    foreach (Upgrade upgrade in data.upgrades) {
      data.maxNitros += upgrade.maxNitros;
      data.nitroReplenishTime *= upgrade.nitroReplenishTime;
      data.maxSpeed += upgrade.maxSpeed;
      data.maxHealth += upgrade.maxHealth;
    }
  }
}
