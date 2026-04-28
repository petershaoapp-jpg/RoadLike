using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBossController : MonoBehaviour, IMovementController
{
    // =============================================
    //  ABILITY 1: BULLET BARRAGE
    // =============================================
    [Header("Barrage Settings")]
    [Tooltip("Number of bullets per wave")]
    [SerializeField] private int barrageCount = 16;
    [Tooltip("Spread angle in degrees (360 = full ring)")]
    [SerializeField] private float barrageSpreadAngle = 360f;
    [Tooltip("Number of waves per barrage")]
    [SerializeField] private int barrageWaves = 3;
    [Tooltip("Delay between waves within one barrage")]
    [SerializeField] private float barrageWaveDelay = 0.4f;
    [Tooltip("Warning time before barrage fires")]
    [SerializeField] private float barrageChargeTime = 2f;
    [Tooltip("Cooldown after all waves finish")]
    [SerializeField] private float barrageCooldown = 7f;

    [Header("Barrage Projectile")]
    [SerializeField] private float barrageProjectileSpeed = 20f;
    [SerializeField] private int barrageProjectileDamage = 8;
    [SerializeField] private float barrageBulletSize = 1f;
    [SerializeField] private Color barrageBulletColor = new Color(0.6f, 1f, 0.2f, 1f); // Sickly green

    // =============================================
    //  ABILITY 2: VINE STRIKE
    // =============================================
    [Header("Vine Strike Settings")]
    [Tooltip("Total number of vines per strike (1 targets player, rest are random)")]
    [SerializeField] private int vineCount = 4;
    [Tooltip("Random vines spawn within this radius of the boss")]
    [SerializeField] private float vineRandomRange = 40f;
    [Tooltip("How close the player must be for vine strikes")]
    [SerializeField] private float vineStrikeRange = 50f;
    [Tooltip("Warning time before vine erupts")]
    [SerializeField] private float vineWarningTime = 1.5f;
    [Tooltip("How long the vine stays up before retracting")]
    [SerializeField] private float vineLingerTime = 1f;
    [Tooltip("Cooldown between vine strikes")]
    [SerializeField] private float vineStrikeCooldown = 5f;

    [Header("Vine Strike Damage")]
    [SerializeField] private int vineStrikeDamage = 20;
    [SerializeField] private float vineKnockbackForce = 20f;

    [Header("Vine Strike Visuals")]
    [SerializeField] private float vineRadius = 8f;
    [SerializeField] private float vineHeight = 15f;
    [SerializeField] private Color dangerZoneColor = new Color(1f, 0f, 0f, 0.25f);
    [SerializeField] private Color vineColor = new Color(0.2f, 0.5f, 0.1f, 1f); // Dark green

    // =============================================
    //  ABILITY 3: VINE WALL
    // =============================================
    [Header("Vine Wall Settings")]
    [Tooltip("Total number of walls per cycle (1 blocks player, rest are random)")]
    [SerializeField] private int wallCount = 3;
    [Tooltip("Random walls spawn within this radius of the boss")]
    [SerializeField] private float wallRandomRange = 50f;
    [Tooltip("Distance ahead of the player to spawn the blocking wall")]
    [SerializeField] private float wallSpawnDistance = 25f;
    [Tooltip("Number of segments in the wall")]
    [SerializeField] private int wallSegmentCount = 8;
    [Tooltip("Width of each wall segment")]
    [SerializeField] private float wallSegmentWidth = 3f;
    [Tooltip("Height of the wall")]
    [SerializeField] private float wallHeight = 4f;
    [Tooltip("How long the wall exists before disappearing")]
    [SerializeField] private float wallDuration = 8f;
    [Tooltip("Cooldown between wall spawns")]
    [SerializeField] private float wallCooldown = 12f;
    [SerializeField] private Color wallColor = new Color(0.15f, 0.4f, 0.05f, 1f); // Deep green

    // =============================================
    //  GENERAL
    // =============================================
    [Header("General")]
    [Tooltip("Player must be within this range to activate the boss")]
    [SerializeField] private float activationRange = 60f;

    private GameObject _player;
    private bool _isActive = false;

    // Track active objects for cleanup
    private List<GameObject> _activeVines = new List<GameObject>();
    private List<GameObject> _activeWalls = new List<GameObject>();

    private void Start()
    {
        _player = GameObject.Find("Car");
    }

    private void Update()
    {
        if (_player == null || _isActive) return;

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance <= activationRange)
        {
            _isActive = true;
            Debug.Log("[TREE BOSS]: Awakened! Starting all ability cycles.");
            StartCoroutine(BarrageCycle());
            StartCoroutine(VineStrikeCycle());
            StartCoroutine(VineWallCycle());
        }
    }

    // =================================================================
    //  ABILITY 1: BULLET BARRAGE CYCLE
    // =================================================================

    private IEnumerator BarrageCycle()
    {
        while (true)
        {
            if (_player == null) yield break;

            // CHARGE-UP: Visual warning
            Debug.Log("[TREE BOSS]: Charging barrage...");
            yield return StartCoroutine(BarrageChargeEffect());

            // FIRE: Multiple waves, each at a different random angle offset
            for (int wave = 0; wave < barrageWaves; wave++)
            {
                float angleOffset = Random.Range(0f, 360f / barrageCount);
                FireBarrage(angleOffset);
                Debug.Log("[TREE BOSS]: BARRAGE WAVE " + (wave + 1) + "/" + barrageWaves);

                if (wave < barrageWaves - 1)
                {
                    yield return new WaitForSeconds(barrageWaveDelay);
                }
            }

            // COOLDOWN
            yield return new WaitForSeconds(barrageCooldown);
        }
    }

    private IEnumerator BarrageChargeEffect()
    {
        // Flash the tree's material to warn the player
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            float elapsed = 0f;

            while (elapsed < barrageChargeTime)
            {
                // Pulse between original and bright color
                float t = Mathf.PingPong(elapsed * 4f, 1f);
                renderer.material.color = Color.Lerp(originalColor, barrageBulletColor, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            renderer.material.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(barrageChargeTime);
        }
    }

    private void FireBarrage(float angleOffset = 0f)
    {
        if (_player == null) return;

        // Calculate the starting angle
        float startAngle;
        if (barrageSpreadAngle >= 360f)
        {
            startAngle = 0f;
        }
        else
        {
            Vector3 toPlayer = _player.transform.position - transform.position;
            toPlayer.y = 0;
            float centerAngle = Mathf.Atan2(toPlayer.x, toPlayer.z) * Mathf.Rad2Deg;
            startAngle = centerAngle - barrageSpreadAngle / 2f;
        }

        float angleStep = barrageSpreadAngle / barrageCount;

        // Store all bullet colliders so we can ignore collisions between them
        List<Collider> bulletColliders = new List<Collider>();

        for (int i = 0; i < barrageCount; i++)
        {
            float angle = startAngle + angleStep * i + angleOffset;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            // Create bullet
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.name = "FireShot";
            bullet.transform.position = new Vector3(transform.position.x, _player.transform.position.y + barrageBulletSize * 0.5f, transform.position.z) + direction * 3f;
            bullet.transform.localScale = Vector3.one * barrageBulletSize;

            // URP Lit material with emission
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetColor("_BaseColor", barrageBulletColor);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", barrageBulletColor * 3f);
            bullet.GetComponent<MeshRenderer>().material = mat;

            // Physics
            Rigidbody rb = bullet.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = direction * barrageProjectileSpeed;

            // Reuse FireShot script
            FireShot shot = bullet.AddComponent<FireShot>();
            shot.damage = barrageProjectileDamage;

            // Ignore collision with the boss itself
            Collider bossCollider = GetComponent<Collider>();
            Collider bulletCollider = bullet.GetComponent<Collider>();
            if (bossCollider != null && bulletCollider != null)
            {
                Physics.IgnoreCollision(bulletCollider, bossCollider);
            }

            if (bulletCollider != null)
            {
                bulletColliders.Add(bulletCollider);
            }
        }

        // Ignore collisions between all bullets so they don't destroy each other
        for (int i = 0; i < bulletColliders.Count; i++)
        {
            for (int j = i + 1; j < bulletColliders.Count; j++)
            {
                Physics.IgnoreCollision(bulletColliders[i], bulletColliders[j]);
            }
        }

        // Ignore collisions between bullets and vine walls so bullets pass through
        VineWall[] vineWalls = FindObjectsByType<VineWall>(FindObjectsSortMode.None);
        foreach (Collider bc in bulletColliders)
        {
            foreach (VineWall vw in vineWalls)
            {
                Collider wallCol = vw.GetComponent<Collider>();
                if (bc != null && wallCol != null)
                {
                    Physics.IgnoreCollision(bc, wallCol);
                }
            }
        }
    }

    // =================================================================
    //  ABILITY 2: VINE STRIKE CYCLE
    // =================================================================

    private IEnumerator VineStrikeCycle()
    {
        while (true)
        {
            if (_player == null) yield break;

            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if (distance > vineStrikeRange)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // Build target positions: first one targets the player, rest are random around the boss
            List<Vector3> vinePositions = new List<Vector3>();

            // Vine #1: aimed at the player
            Vector3 playerTargetPos = _player.transform.position;
            Rigidbody playerRb = _player.GetComponent<Rigidbody>();
            if (playerRb != null && playerRb.linearVelocity.sqrMagnitude > 1f)
            {
                playerTargetPos += playerRb.linearVelocity * 0.3f;
            }
            else
            {
                Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(1f, 3f);
                playerTargetPos += new Vector3(randomOffset.x, 0f, randomOffset.y);
            }
            vinePositions.Add(playerTargetPos);

            // Remaining vines: random positions around the boss
            for (int i = 1; i < vineCount; i++)
            {
                Vector2 rng = Random.insideUnitCircle * vineRandomRange;
                Vector3 randomPos = transform.position + new Vector3(rng.x, 0f, rng.y);
                vinePositions.Add(randomPos);
            }

            // PHASE 1: Show all danger zones at once
            List<GameObject> dangerZones = new List<GameObject>();
            foreach (Vector3 pos in vinePositions)
            {
                dangerZones.Add(CreateDangerZone(pos));
            }
            Debug.Log("[TREE BOSS]: Vine strike warning! (" + vineCount + " vines)");
            yield return new WaitForSeconds(vineWarningTime);

            // PHASE 2: Destroy all danger zones, spawn all vines
            foreach (GameObject dz in dangerZones)
            {
                if (dz != null) Destroy(dz);
            }
            List<GameObject> spawnedVines = new List<GameObject>();
            foreach (Vector3 pos in vinePositions)
            {
                spawnedVines.Add(SpawnVine(pos));
            }
            Debug.Log("[TREE BOSS]: VINES ERUPT!");

            // PHASE 3: All vines linger then retract
            yield return new WaitForSeconds(vineLingerTime);
            foreach (GameObject vine in spawnedVines)
            {
                if (vine != null)
                {
                    _activeVines.Remove(vine);
                    Destroy(vine);
                }
            }

            // COOLDOWN
            yield return new WaitForSeconds(vineStrikeCooldown);
        }
    }

    private GameObject CreateDangerZone(Vector3 position)
    {
        // Flat cylinder on the ground as danger indicator
        GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        zone.name = "VineDangerZone";
        Destroy(zone.GetComponent<Collider>()); // No collision, visual only

        // Raycast to find ground - start from high up, skip the car
        Vector3 rayOrigin = new Vector3(position.x, 200f, position.z);
        float groundY = 0f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 400f))
        {
            if (hit.collider.gameObject.name != "Car")
            {
                groundY = hit.point.y;
            }
        }

        zone.transform.position = new Vector3(position.x, groundY + 0.05f, position.z);
        zone.transform.localScale = new Vector3(vineRadius * 2f, 0.02f, vineRadius * 2f);

        // URP transparent material
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetFloat("_Surface", 1);
        mat.SetFloat("_Blend", 0);
        mat.SetFloat("_ZWrite", 0);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetColor("_BaseColor", dangerZoneColor);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = 3000;
        zone.GetComponent<MeshRenderer>().material = mat;

        return zone;
    }

    private GameObject SpawnVine(Vector3 position)
    {
        // Vine is a tall cylinder that erupts from the ground
        GameObject vine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        vine.name = "VineStrike";

        // Find ground level - use only the XZ of target, raycast straight down
        Vector3 rayOrigin = new Vector3(position.x, 200f, position.z);
        float groundY = 0f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 400f))
        {
            if (hit.collider.gameObject.name != "Car")
            {
                groundY = hit.point.y;
            }
        }

        // Position so the vine sticks up from the ground
        vine.transform.position = new Vector3(position.x, groundY + vineHeight / 2f, position.z);
        vine.transform.localScale = new Vector3(vineRadius * 0.5f, vineHeight / 2f, vineRadius * 0.5f);

        // Green material with emission
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", vineColor);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", vineColor * 1.5f);
        vine.GetComponent<MeshRenderer>().material = mat;

        // Physics - kinematic so it doesn't fall
        Rigidbody rb = vine.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        // Attach VineAttack script for damage
        VineAttack attack = vine.AddComponent<VineAttack>();
        attack.damage = vineStrikeDamage;
        attack.knockbackForce = vineKnockbackForce;

        _activeVines.Add(vine);
        return vine;
    }

    // =================================================================
    //  ABILITY 3: VINE WALL CYCLE
    // =================================================================

    private IEnumerator VineWallCycle()
    {
        while (true)
        {
            if (_player == null) yield break;

            // Wall #1: blocks the player's path
            List<GameObject> spawnedWalls = new List<GameObject>();
            spawnedWalls.Add(SpawnVineWall());

            // Remaining walls: random positions and orientations around the boss
            for (int i = 1; i < wallCount; i++)
            {
                Vector2 rng = Random.insideUnitCircle * wallRandomRange;
                Vector3 randomCenter = transform.position + new Vector3(rng.x, 0f, rng.y);
                float randomAngle = Random.Range(0f, 360f);
                Vector3 randomFacing = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;
                spawnedWalls.Add(SpawnVineWallAt(randomCenter, randomFacing));
            }
            Debug.Log("[TREE BOSS]: Vine walls spawned! (" + wallCount + " walls)");

            // All walls last for the same duration
            yield return new WaitForSeconds(wallDuration);

            foreach (GameObject wall in spawnedWalls)
            {
                if (wall != null)
                {
                    _activeWalls.Remove(wall);
                    Destroy(wall);
                }
            }
            Debug.Log("[TREE BOSS]: Vine walls crumbled.");

            // COOLDOWN
            yield return new WaitForSeconds(wallCooldown);
        }
    }

    // Spawns a wall blocking the player's path
    private GameObject SpawnVineWall()
    {
        if (_player == null) return null;

        Rigidbody playerRb = _player.GetComponent<Rigidbody>();
        Vector3 playerForward = Vector3.forward;
        if (playerRb != null && playerRb.linearVelocity.sqrMagnitude > 1f)
        {
            playerForward = playerRb.linearVelocity.normalized;
        }
        else
        {
            playerForward = _player.transform.forward;
        }
        playerForward.y = 0;
        playerForward.Normalize();

        Vector3 wallCenter = _player.transform.position + playerForward * wallSpawnDistance;
        return SpawnVineWallAt(wallCenter, playerForward);
    }

    // Spawns a wall at any position facing any direction
    private GameObject SpawnVineWallAt(Vector3 center, Vector3 facing)
    {
        facing.y = 0;
        facing.Normalize();

        Vector3 wallDirection = Vector3.Cross(Vector3.up, facing).normalized;

        GameObject wallParent = new GameObject("VineWall");

        float totalWidth = wallSegmentCount * wallSegmentWidth;
        Vector3 startPos = center - wallDirection * (totalWidth / 2f);

        for (int i = 0; i < wallSegmentCount; i++)
        {
            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            segment.name = "VineWallSegment";

            Vector3 segPos = startPos + wallDirection * (i * wallSegmentWidth + wallSegmentWidth / 2f);

            if (Physics.Raycast(segPos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f))
            {
                segPos.y = hit.point.y + wallHeight / 2f;
            }

            segment.transform.position = segPos;
            segment.transform.localScale = new Vector3(wallSegmentWidth, wallHeight, 1.5f);
            segment.transform.rotation = Quaternion.LookRotation(facing);
            segment.transform.SetParent(wallParent.transform);

            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetColor("_BaseColor", wallColor);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", wallColor * 1.2f);
            segment.GetComponent<MeshRenderer>().material = mat;

            segment.AddComponent<VineWall>();
        }

        _activeWalls.Add(wallParent);
        return wallParent;
    }

    // =================================================================
    //  CLEANUP
    // =================================================================

    private void OnDestroy()
    {
        // Clean up any active vines, walls, and danger zones when boss dies
        foreach (GameObject vine in _activeVines)
        {
            if (vine != null) Destroy(vine);
        }
        foreach (GameObject wall in _activeWalls)
        {
            if (wall != null) Destroy(wall);
        }
        foreach (GameObject zone in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (zone != null && zone.name == "VineDangerZone")
            {
                Destroy(zone);
            }
        }
        _activeVines.Clear();
        _activeWalls.Clear();
    }

    // =================================================================
    //  IMovementController
    // =================================================================

    public Vector3 GetMovement()
    {
        return Vector3.zero;
    }
}
