using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    
    private void Start()
    {
        StartCoroutine(Shoot());
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position,50);

        colliders = colliders.Where(c => c.gameObject.CompareTag("Enemy")).ToArray();

        Vector3 closest = Vector3.positiveInfinity;
        Collider target = null;

        foreach (Collider collider in colliders)
        {
            float closestDistance = Vector3.Distance(transform.position,closest);
            float testDistance = Vector3.Distance(transform.position,collider.transform.position);

            if (testDistance < closestDistance)
            {
                closest = collider.transform.position;
                target = collider;
            }
        }

        if (target != null)
        {
            transform.LookAt(target.transform);
        }
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(.5f);

        RaycastHit hit;

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10000);

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow,1);

        if (hit.collider && hit.collider.gameObject.CompareTag("Enemy"))
        {
            Health enemyHealth = hit.collider.gameObject.GetComponent<Health>();

            float damage = playerData.attack;

            if (Random.Range(1, 100) < playerData.critChance)
            {
                damage *= playerData.critDamage;
                
                Debug.Log("CRITICAL HIT!");
            } else Debug.Log("NONCRIT");
            
            Debug.Log(damage);
            
            enemyHealth.TakeDamage(damage);
        }

        StartCoroutine(Shoot());
    }
}
