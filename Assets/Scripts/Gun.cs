using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    
   
    public float fireRate = 0.5f;
    public float turnSpeed = 15f; // This controls how quickly the gun turns to face the target. Higher values = faster turning. You can use this for upgrades as well! 

    [Header("Gun Settings")]
    [SerializeField] private float targetRange = 50f;

    public List<string> upgradeNames;
    private Collider _currentTarget;

    private void Start()
    {
        upgradeNames = playerData.upgrades.ConvertAll(data => data.name);
        
        // Use a single coroutine loop instead of dangerous recursive calls
        StartCoroutine(ShootRoutine());
    }

    private void Update()
    {
        // 1. Find enemies in range
        Collider[] colliders = Physics.OverlapSphere(transform.position, targetRange);
        colliders = colliders.Where(c => c.gameObject.CompareTag("Enemy")).ToArray();

        float closestDistance = Mathf.Infinity;
        _currentTarget = null;

        // 2. Find the closest enemy
        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                _currentTarget = collider;
            }
        }

        // 3. Smoothly aim at the target
        if (_currentTarget != null)
        {
            Vector3 directionToTarget = _currentTarget.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);

            RaycastHit hit;
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            // Draw ray in editor for testing
            Debug.DrawRay(transform.position, forward * targetRange, Color.red, fireRate);

            if (Physics.Raycast(transform.position, forward, out hit, targetRange))
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    Health enemyHealth = hit.collider.gameObject.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        CalculateAndApplyDamage(enemyHealth);
                    }
                }
            }
        }
    }
//cleaner method to keep track of the damage.
    private void CalculateAndApplyDamage(Health enemyHealth)
    {
        float damage = playerData.attack;

        if (Random.Range(1, 100) < playerData.critChance)
        {
            damage *= 2f + (playerData.critDamage / 100f);
            Debug.Log("[GUN]: CRITICAL HIT! Damage: " + damage);
        }
        else
        {
            if (upgradeNames.Contains("Lust"))
            {
                damage = 0f;
            }
            Debug.Log("[GUN]: Normal Hit. Damage: " + damage);
        }

        enemyHealth.TakeDamage((int)damage);
    }
}