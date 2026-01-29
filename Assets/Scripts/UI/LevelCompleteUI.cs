using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelCompleteUI : MonoBehaviour
{
  [Header("UI Components")]
  [SerializeField] private TMP_Text timeDisplay;
  [SerializeField] private TMP_Text rank;
  [SerializeField] private TMP_Text rarityBonus;
  [SerializeField] private Button retry;

  [SerializeField] private GameObject upgradeCard;
  [SerializeField] private Canvas canvas;

  [Header("Data objects")]
  [SerializeField] private GameData data;
  [SerializeField] private PlayerData playerData;
  private UpgradeSelector _upgradeSelector;
  

  private void Start() {
    _upgradeSelector = GetComponent<UpgradeSelector>();

    int bonusRarityChance = playerData.luck;

    timeDisplay.text = data.formattedTime;
    
    float t = data.time;
    
    if (t < 12) {
      rank.text = "S+";
      bonusRarityChance += 215;
    } else if (t < 15) {
      rank.text = "S";
      bonusRarityChance += 200;
    } else if (t < 20) {
      rank.text = "A";
      bonusRarityChance += 175;
    } else if (t  < 25) {
      rank.text = "B";
      bonusRarityChance += 150;
    } else if (t < 30) {
      rank.text = "C";
      bonusRarityChance += 125;
    } else if (t < 35) {
      rank.text = "D";
      bonusRarityChance += 110;
    } else {
      rank.text = "F";
      bonusRarityChance += 100;
    }

    Debug.Log("Bonus rarity chance:" + bonusRarityChance);

    List<Upgrade> purchasable = new List<Upgrade>();

    float x = 200;

    for (var i = 0; i < 3; i++)
    {
      Upgrade upgrade = _upgradeSelector.SelectUpgrade(bonusRarityChance);
      purchasable.Add(upgrade);
      GameObject card = Instantiate(upgradeCard);
      card.transform.SetParent(canvas.transform,false);
      card.GetComponent<UpgradeCard>().upgrade = upgrade;
      card.GetComponent<RectTransform>().anchoredPosition = new Vector3(x,0,0);
      
      x -= 200;
    }


  }

  public void Retry() 
  {
    SceneManager.LoadScene(1);
  }
}
