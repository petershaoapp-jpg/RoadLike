using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TitleScreenUI : MonoBehaviour
{
  [SerializeField] private PlayerData data;

  public void Play()
  {

    data.upgrades = new List<Upgrade>();
    SceneManager.LoadScene(1);
  }
}
