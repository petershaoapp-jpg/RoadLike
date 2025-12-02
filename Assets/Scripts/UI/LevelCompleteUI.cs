using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelCompleteUI : MonoBehaviour
{
  [Header("UI Components")]
  [SerializeField] private TMP_Text timeDisplay;
  [SerializeField] private TMP_Text rank;
  [SerializeField] private TMP_Text rarityBonus;
  [SerializeField] private Button retry;

  [SerializeField] private GameData data;

  private void Start() {
    timeDisplay.text = data.formattedTime;
    
    float t = data.time;
    
    if (t < 12) {
      rank.text = "S+";
    } else if (t < 15) {
      rank.text = "S";
    } else if (t < 20) {
      rank.text = "A";
    } else if (t  < 25) {
      rank.text = "B";
    } else if (t < 30) {
      rank.text = "C";
    } else if (t < 35) {
      rank.text = "D";
    } else {
      rank.text = "F";
    }
  }

  public void Retry() 
  {
    SceneManager.LoadScene(1);
  }
}
