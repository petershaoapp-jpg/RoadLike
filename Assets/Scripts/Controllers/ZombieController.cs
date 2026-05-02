using System;
using UnityEngine;

public class ZombieController : MonoBehaviour, IMovementController
{
    private GameObject _player;
    private bool _hasNoticedPlayer = false;
    private AudioSource _source;
    [SerializeField] private AudioClip clip;
    

    private void Start() {
        _player = GameObject.Find("Car");
        _source = GetComponent<AudioSource>();
    }

    public Vector3 GetMovement()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) > 100) return Vector3.zero;
        
        if (!_hasNoticedPlayer) {
            _source.PlayOneShot(clip);
            Debug.Log("bro what");
            _hasNoticedPlayer = true;
        }
        
        Vector3 direction = _player.transform.position - transform.position;
        return direction.normalized;
    }

    private void Update()
    {
        transform.LookAt(_player.transform);
    }
}
