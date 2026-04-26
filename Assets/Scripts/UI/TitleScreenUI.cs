using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TitleScreenUI : MonoBehaviour
{
  [SerializeField] private PlayerData data;

  public void Play()
  {
    data.upgrades = new List<Upgrade>();
    data.level = 1;
    
    SceneManager.LoadScene(1);
  }
}
