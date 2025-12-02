using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
  [SerializeField] private GameData data;
  private Timer _timer;

  private void Start() 
  {
    _timer = GameObject.Find("Car").GetComponent<Timer>();
  }

  private void OnTriggerEnter(Collider other) {
    if (other.name != "Car") return; 

    int minutes = (int)Mathf.Floor(_timer.time / 60);
    int seconds = (int)Mathf.Floor(_timer.time % 60);
    int ms = (int)Mathf.Floor((_timer.time % 1) * 100);

    string formatted = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + ms.ToString("D2");

    data.formattedTime = formatted;
    data.time = _timer.time;

    SceneManager.LoadScene(2);
  }
}
