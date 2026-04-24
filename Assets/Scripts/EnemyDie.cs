using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDie : MonoBehaviour, IDie
{
    [SerializeField] private PlayerData playerData;
    private List<string> _upgradeNames;

    [SerializeField] private GameObject soul;

    private void Start()
    {
        _upgradeNames = playerData.upgrades.ConvertAll(data => data.name);
    }

    public void OnDie()
    {
        if (_upgradeNames.Contains("Gluttony"))
        {
            Instantiate(soul, transform.position, transform.rotation);
        }
        
        Destroy(gameObject);
    }
}
