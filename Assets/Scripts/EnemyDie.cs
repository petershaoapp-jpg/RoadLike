using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDie : MonoBehaviour, IDie
{
    [SerializeField] private PlayerData playerData;
    private List<string> _upgradeNames;

    [SerializeField] private GameObject soul;
    [SerializeField] private AudioClip die;

    private AudioSource _audioManager;

    private void Start()
    {
        _upgradeNames = playerData.upgrades.ConvertAll(data => data.name);
        _audioManager = GameObject.Find("Audio manager").GetComponent<AudioSource>();
    }

    public void OnDie()
    {
        if (_upgradeNames.Contains("Gluttony"))
        {
            Instantiate(soul, transform.position, transform.rotation);
        }
        
        _audioManager.PlayOneShot(die);
        
        Destroy(gameObject);
    }
}
