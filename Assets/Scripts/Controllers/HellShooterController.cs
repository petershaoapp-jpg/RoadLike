using System.Collections;
using UnityEngine;

public class HellShooterController : MonoBehaviour, IMovementController
{
    [Header("Detection")]
    [Tooltip("How close the player needs to be before this enemy starts shooting")]
    [SerializeField] private float detectionRange = 40f;
    [SerializeField] private float turnSpeed = 5f;

    [Header("Burst Fire")]
    [Tooltip("Number of shots per burst")]
    [SerializeField] private int burstCount = 3;
    [Tooltip("Time between each shot in a burst")]
    [SerializeField] private float timeBetweenShots = 0.5f;
    [Tooltip("Rest time after a burst before shooting again")]
    [SerializeField] private float restTime = 3f;

    [Header("Projectile")]
    [SerializeField] private float projectileSpeed = 30f;
    [SerializeField] private int projectileDamage = 10;
    [SerializeField] private float bulletSize = 0.3f;
    [SerializeField] private Color bulletColor = new Color(1f, 0.3f, 0f, 1f); // Orange-red

    [Header("Fire Point")]
    [Tooltip("Drag in the child object where bullets spawn from")]
    [SerializeField] private Transform firePoint;

    private GameObject _player;
    private bool _isShooting = false;

    private void Start()
    {
        _player = GameObject.Find("Car");

        // If no fire point assigned, default to this object's position
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    private void Update()
    {
        if (_player == null) return;

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        // Player not in range, do nothing
        if (distance > detectionRange) return;

        // Smoothly rotate to face the player
        Vector3 directionToPlayer = _player.transform.position - transform.position;
        directionToPlayer.y = 0;
        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Start shooting if not already in a burst cycle
        if (!_isShooting)
        {
            StartCoroutine(BurstFireRoutine());
        }
    }

    private IEnumerator BurstFireRoutine()
    {
        _isShooting = true;

        // Fire a burst
        for (int i = 0; i < burstCount; i++)
        {
            if (_player == null) break;

            SpawnProjectile();
            Debug.Log("[HELLSHOOTER]: Shot " + (i + 1) + "/" + burstCount);

            if (i < burstCount - 1)
            {
                yield return new WaitForSeconds(timeBetweenShots);
            }
        }

        // Rest phase - player can rush in and attack
        Debug.Log("[HELLSHOOTER]: Resting...");
        yield return new WaitForSeconds(restTime);

        _isShooting = false;
    }

    private void SpawnProjectile()
    {
        if (_player == null) return;

        // Aim direction from fire point to player
        Vector3 direction = (_player.transform.position - firePoint.position).normalized;

        // Create the bullet sphere
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.name = "FireShot";
        bullet.transform.position = firePoint.position;
        bullet.transform.localScale = Vector3.one * bulletSize;

        // URP Lit material with emission (glow effect)
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", bulletColor);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", bulletColor * 3f);
        bullet.GetComponent<MeshRenderer>().material = mat;

        // Physics setup
        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = direction * projectileSpeed;

        // Attach the FireShot script for collision handling
        FireShot shot = bullet.AddComponent<FireShot>();
        shot.damage = projectileDamage;

        // Ignore collision between bullet and this enemy so it doesn't hit us
        Collider enemyCollider = GetComponent<Collider>();
        Collider bulletCollider = bullet.GetComponent<Collider>();
        if (enemyCollider != null && bulletCollider != null)
        {
            Physics.IgnoreCollision(bulletCollider, enemyCollider);
        }
    }

    // ========== IMovementController ==========

    public Vector3 GetMovement()
    {
        // Stationary enemy - never moves
        return Vector3.zero;
    }
}
