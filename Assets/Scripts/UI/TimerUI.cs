using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
  private Timer _timer;
  private TMP_Text _text;

  private void Start()
  {
    _timer = GameObject.Find("Car").GetComponent<Timer>();
    _text = GetComponent<TMP_Text>();
  }

  private void Update() 
  {
    int minutes = (int)Mathf.Floor(_timer.time / 60);
    int seconds = (int)Mathf.Floor(_timer.time % 60);
    int ms = (int)Mathf.Floor((_timer.time % 1) * 100);

    string formatted = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + ms.ToString("D2");

    _text.text = formatted;
  }
}
