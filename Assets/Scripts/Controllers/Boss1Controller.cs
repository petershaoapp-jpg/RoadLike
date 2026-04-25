using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChargeController : MonoBehaviour, IMovementController
{
    [Header("Boss Charge Settings")]
    [Tooltip("Distance to trigger the charge (How close until it gets mad)")]
    [SerializeField] private float triggerDistance = 35f;
    [SerializeField] private float chargePrepTime = 1.5f; // Wait time before charging
    [SerializeField] private float chargeDuration = 2f;   // How long it charges
    [SerializeField] private float restTime = 3f;         // Cooldown after charging

    [Header("Speed Multipliers")]
    [Tooltip("Normal walking speed (0.5 means half of MoveConstantSpeed)")]
    [SerializeField] private float walkSpeedMultiplier = 0.5f;
    [Tooltip("Charging speed (3 means 300% speed)")]
    [SerializeField] private float chargeSpeedMultiplier = 3f;

    [Header("Charge Damage")]
    [SerializeField] private int chargeDamage = 30;
    [SerializeField] private float knockbackForce = 50f;

    [Header("Danger Zone")]
    [SerializeField] private float dangerZoneWidth = 3f;
    [SerializeField] private float segmentLength = 2f;
    [SerializeField] private Color dangerZoneColor = new Color(1f, 0f, 0f, 0.2f);

    private GameObject _player;
    private Vector3 _currentMovement = Vector3.zero;
    private bool _isCharging = false;
    private bool _isPrepping = false;
    private GameObject _dangerZoneParent;
    private List<GameObject> _segments = new List<GameObject>();
    private Material _dangerZoneMaterial;
    private bool _isActivelyCharging = false;
    private bool _hasHitPlayer = false;

    private void Start()
    {
        _player = GameObject.Find("Car");
    }

    private void Update()
    {
        if (_player == null) return;

        // Danger zone tracks the player during prep phase
        if (_isPrepping && _dangerZoneParent != null)
        {
            UpdateDangerZone();
        }

        if (_isCharging) return;

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance <= triggerDistance)
        {
            StartCoroutine(ChargeAttack());
        }
        else
        {
            // Slowly walk towards player
            Vector3 direction = _player.transform.position - transform.position;
            direction.y = 0;
            _currentMovement = direction.normalized * walkSpeedMultiplier;
        }
    }

    private IEnumerator ChargeAttack()
    {
        _isCharging = true;
        _isPrepping = true;

        // PHASE 1: PREPARE (Stop moving, show danger zone, aim at player)
        _currentMovement = Vector3.zero;
        CreateDangerZone();
        Debug.Log("[BOSS]: Preparing to Charge...");
        yield return new WaitForSeconds(chargePrepTime);

        // PHASE 2: CHARGE! (Destroy danger zone, lock direction, go fast)
        _isPrepping = false;
        DestroyDangerZone();

        _isActivelyCharging = true;
        _hasHitPlayer = false;

        if (_player != null)
        {
            Vector3 chargeDirection = _player.transform.position - transform.position;
            chargeDirection.y = 0;
            _currentMovement = chargeDirection.normalized * chargeSpeedMultiplier;
        }
        Debug.Log("[BOSS]: RAAAAAH! CHARGING!");

        yield return new WaitForSeconds(chargeDuration);

        // PHASE 3: COOLDOWN (Stop moving and rest)
        _isActivelyCharging = false;
        _currentMovement = Vector3.zero;
        Debug.Log("[BOSS]: Resting...");
        yield return new WaitForSeconds(restTime);

        _isCharging = false;
    }

    // ========== Danger Zone Methods ==========

    private void CreateDangerZone()
    {
        // Parent object to hold all segments
        _dangerZoneParent = new GameObject("BossDangerZone");

        // Create the shared material once (URP transparent)
        _dangerZoneMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        _dangerZoneMaterial.SetFloat("_Surface", 1);  // 1 = Transparent
        _dangerZoneMaterial.SetFloat("_Blend", 0);
        _dangerZoneMaterial.SetFloat("_ZWrite", 0);
        _dangerZoneMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _dangerZoneMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _dangerZoneMaterial.SetColor("_BaseColor", dangerZoneColor);
        _dangerZoneMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        _dangerZoneMaterial.renderQueue = 3000;

        UpdateDangerZone();
    }

    private void UpdateDangerZone()
    {
        if (_dangerZoneParent == null || _player == null) return;

        Vector3 direction = _player.transform.position - transform.position;
        direction.y = 0;
        float totalDistance = direction.magnitude;
        Vector3 dirNormalized = direction.normalized;

        // How many segments do we need?
        int segmentCount = Mathf.CeilToInt(totalDistance / segmentLength);

        // Add more segments if we don't have enough
        while (_segments.Count < segmentCount)
        {
            GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seg.name = "DangerSegment";
            Destroy(seg.GetComponent<Collider>());
            seg.GetComponent<MeshRenderer>().material = _dangerZoneMaterial;
            seg.transform.SetParent(_dangerZoneParent.transform);
            _segments.Add(seg);
        }

        // Hide extra segments if distance got shorter
        for (int i = 0; i < _segments.Count; i++)
        {
            _segments[i].SetActive(i < segmentCount);
        }

        // Position each segment along the path, each one raycasts down to find ground
        for (int i = 0; i < segmentCount; i++)
        {
            // Center of this segment along the boss→player line
            float t = (i + 0.5f) * segmentLength;
            if (t > totalDistance) t = totalDistance - segmentLength * 0.5f;

            Vector3 segPos = transform.position + dirNormalized * t;

            // Raycast down to find actual ground height at this point
            if (Physics.Raycast(segPos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f))
            {
                segPos.y = hit.point.y + 0.15f;
            }

            // Figure out the actual length of this segment (last one may be shorter)
            float thisLength = segmentLength;
            if (i == segmentCount - 1)
            {
                thisLength = totalDistance - (i * segmentLength);
            }

            _segments[i].transform.position = segPos;
            _segments[i].transform.rotation = Quaternion.LookRotation(dirNormalized);
            _segments[i].transform.localScale = new Vector3(dangerZoneWidth, 0.01f, thisLength);
        }
    }

    private void DestroyDangerZone()
    {
        if (_dangerZoneParent != null)
        {
            Destroy(_dangerZoneParent);
            _dangerZoneParent = null;
            _segments.Clear();
        }
        if (_dangerZoneMaterial != null)
        {
            Destroy(_dangerZoneMaterial);
            _dangerZoneMaterial = null;
        }
    }

    // ========== Collision Damage ==========

    private void OnCollisionEnter(Collision collision)
    {
        // Only deal damage while actively charging, and only once per charge
        if (!_isActivelyCharging || _hasHitPlayer) return;

        // Only damage the player car
        if (collision.gameObject.name != "Car") return;

        // Mark as hit so we don't damage again this charge
        _hasHitPlayer = true;

        // Deal damage
        Health playerHealth = collision.gameObject.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(chargeDamage);
            Debug.Log("[BOSS]: Hit the player for " + chargeDamage + " damage!");
        }

        // Knockback: launch the car in the charge direction + a bit upward
        Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 knockDirection = (collision.transform.position - transform.position).normalized;
            knockDirection.y = 0;
            knockDirection += Vector3.up * 0.3f; // Slight upward lift so the car gets "launched"
            playerRb.AddForce(knockDirection * knockbackForce, ForceMode.Impulse);
        }
    }

    // ========== Interface ==========

    public Vector3 GetMovement()
    {
        return _currentMovement;
    }
}
