using System.Collections;
using System.Linq;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private bool _hasTarget;

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

        if (target == null)
        {
            _hasTarget = false;
        } else
        {
            transform.LookAt(target.transform);
            _hasTarget = true;
        }
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(.5f);

        Debug.Log("shooting!!");

        RaycastHit hit;

        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10000);

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow,1);

        if (hit.collider && hit.collider.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("HIT!!" + hit.collider.gameObject.name);

            Health enemyHealth = hit.collider.gameObject.GetComponent<Health>();

            enemyHealth.TakeDamage(1);

            Debug.Log("Hit enemy: " + enemyHealth.health);
        }

        StartCoroutine(Shoot());
    }
}
