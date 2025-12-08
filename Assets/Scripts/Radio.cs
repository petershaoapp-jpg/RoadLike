using UnityEngine;

public class Radio : MonoBehaviour
{
  [SerializeField] private AudioClip[] songs;
  private AudioSource _source;
  private AudioClip _currentSong;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    _source = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update()
  {
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
