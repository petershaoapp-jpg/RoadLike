using UnityEngine;
using UnityEngine.SceneManagement;

public class Radio : MonoBehaviour
{
  [SerializeField] private AudioClip[] songs;
  private AudioSource _source;
  private AudioClip _currentSong;
  private AudioLowPassFilter _lowPassFilter;
  private AudioHighPassFilter _highPassFilter;
  
  private static GameObject _instance;

  private void Awake()
  {
    if (!_instance) _instance = gameObject;
    if (_instance != gameObject) Destroy(gameObject);
    
    DontDestroyOnLoad(gameObject);
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    _source = GetComponent<AudioSource>();
    _lowPassFilter = GetComponent<AudioLowPassFilter>();
    _highPassFilter = GetComponent<AudioHighPassFilter>();
  }

  // Update is called once per frame
  void Update()
  {
    if (SceneManager.GetActiveScene().name == "TitleScreen")
    {
      _instance = null;
      Destroy(gameObject);
    }
    
    if (SceneManager.GetActiveScene().name == "LevelComplete")
    {
      _lowPassFilter.cutoffFrequency = 2400;
      _highPassFilter.cutoffFrequency = 650;
    }
    else
    {
      _lowPassFilter.cutoffFrequency = 22000;
      _highPassFilter.cutoffFrequency = 10;
    }
    
    if (_source.isPlaying) return;

    AudioClip song;

    do {

      int randomIndex = Random.Range(0, songs.Length);

      song = songs[randomIndex];
    } while (song == _currentSong);    


    _currentSong = song;

    _source.clip = song;

    _source.Play();

    Debug.Log(song.name);
  }
}
