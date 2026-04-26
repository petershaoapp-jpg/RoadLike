using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBossController : MonoBehaviour, IMovementController
{
    // =============================================
    //  ABILITY 1: BULLET BARRAGE
    // =============================================
    [Header("Barrage Settings")]
    [Tooltip("Number of bullets per barrage")]
    [SerializeField] private int barrageCount = 16;
    [Tooltip("Spread angle in degrees (360 = full ring)")]
    [SerializeField] private float barrageSpreadAngle = 360f;
    [Tooltip("Warning time before barrage fires")]
    [SerializeField] private float barrageChargeTime = 2f;
    [Tooltip("Cooldown after barrage")]
    [SerializeField] private float barrageCooldown = 7f;

    [Header("Barrage Projectile")]
    [SerializeField] private float barrageProjectileSpeed = 20f;
    [SerializeField] private int barrageProjectileDamage = 8;
    [SerializeField] private float barrageBulletSize = 0.4f;
    [SerializeField] private Color barrageBulletColor = new Color(0.6f, 1f, 0.2f, 1f); // Sickly green

    // =============================================
    //  ABILITY 2: VINE STRIKE
    // =============================================
    [Header("Vine Strike Settings")]
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
    [SerializeField] private float vineRadius = 3f;
    [SerializeField] private float vineHeight = 6f;
    [SerializeField] private Color dangerZoneColor = new Color(1f, 0f, 0f, 0.25f);
    [SerializeField] private Color vineColor = new Color(0.2f, 0.5f, 0.1f, 1f); // Dark green

    // =============================================
    //  ABILITY 3: VINE WALL
    // =============================================
    [Header("Vine Wall Settings")]
    [Tooltip("Distance ahead of the player to spawn the wall")]
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

            // FIRE: Spawn ring/fan of bullets
            FireBarrage();
            Debug.Log("[TREE BOSS]: BARRAGE FIRED!");

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

    private void FireBarrage()
    {
        if (_player == null) return;

        // Calculate the starting angle
        // If 360, fire a full ring; otherwise fire a fan aimed at the player
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

        for (int i = 0; i < barrageCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            // Create bullet
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.name = "FireShot";
            bullet.transform.position = new Vector3(transform.position.x, _player.transform.position.y, transform.position.z) + direction * 1.5f;
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

            // Pick a target position near the player (with slight prediction)
            Vector3 targetPos = _player.transform.position;
            Rigidbody playerRb = _player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Lead the target slightly based on velocity
                targetPos += playerRb.linearVelocity * 0.3f;
            }

            // PHASE 1: Show red danger zone
            GameObject dangerZone = CreateDangerZone(targetPos);
            Debug.Log("[TREE BOSS]: Vine strike warning!");
            yield return new WaitForSeconds(vineWarningTime);

            // PHASE 2: Destroy danger zone, spawn vine
            if (dangerZone != null) Destroy(dangerZone);
            GameObject vine = SpawnVine(targetPos);
            Debug.Log("[TREE BOSS]: VINE ERUPTS!");

            // PHASE 3: Vine lingers then retracts
            yield return new WaitForSeconds(vineLingerTime);
            if (vine != null)
            {
                _activeVines.Remove(vine);
                Destroy(vine);
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

        // Raycast to find ground
        Vector3 groundPos = position;
        if (Physics.Raycast(position + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f))
        {
            groundPos = hit.point;
        }

        zone.transform.position = groundPos + Vector3.up * 0.05f;
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

        // Find ground level
        Vector3 groundPos = position;
        if (Physics.Raycast(position + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f))
        {
            groundPos = hit.point;
        }

        // Position so the vine sticks up from the ground
        vine.transform.position = groundPos + Vector3.up * (vineHeight / 2f);
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

            // Spawn a wall ahead of the player
            GameObject wall = SpawnVineWall();
            Debug.Log("[TREE BOSS]: Vine wall spawned!");

            // Wall lasts for a duration
            yield return new WaitForSeconds(wallDuration);

            if (wall != null)
            {
                _activeWalls.Remove(wall);
                Destroy(wall);
            }
            Debug.Log("[TREE BOSS]: Vine wall crumbled.");

            // COOLDOWN
            yield return new WaitForSeconds(wallCooldown);
        }
    }

    private GameObject SpawnVineWall()
    {
        if (_player == null) return null;

        // Get player's forward direction to place wall perpendicular to it
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

        // Wall spawns ahead of the player
        Vector3 wallCenter = _player.transform.position + playerForward * wallSpawnDistance;

        // Wall direction is perpendicular to player's movement
        Vector3 wallDirection = Vector3.Cross(Vector3.up, playerForward).normalized;

        // Parent object for all segments
        GameObject wallParent = new GameObject("VineWall");

        // Calculate starting position (center the wall)
        float totalWidth = wallSegmentCount * wallSegmentWidth;
        Vector3 startPos = wallCenter - wallDirection * (totalWidth / 2f);

        for (int i = 0; i < wallSegmentCount; i++)
        {
            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            segment.name = "VineWallSegment";

            // Position this segment
            Vector3 segPos = startPos + wallDirection * (i * wallSegmentWidth + wallSegmentWidth / 2f);

            // Raycast to find ground
            if (Physics.Raycast(segPos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f))
            {
                segPos.y = hit.point.y + wallHeight / 2f;
            }

            segment.transform.position = segPos;
            segment.transform.localScale = new Vector3(wallSegmentWidth, wallHeight, 1.5f);
            segment.transform.rotation = Quaternion.LookRotation(playerForward);
            segment.transform.SetParent(wallParent.transform);

            // Green material
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetColor("_BaseColor", wallColor);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", wallColor * 1.2f);
            segment.GetComponent<MeshRenderer>().material = mat;

            // Attach VineWall script (no damage, just solid)
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
        // Clean up any active vines and walls when boss dies
        foreach (GameObject vine in _activeVines)
        {
            if (vine != null) Destroy(vine);
        }
        foreach (GameObject wall in _activeWalls)
        {
            if (wall != null) Destroy(wall);
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
