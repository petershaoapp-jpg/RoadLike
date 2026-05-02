using UnityEngine;
using UnityEngine.InputSystem;

public class BetterCarController : MonoBehaviour, IMovementController
{
    // Tunable parameters
    [Header("Data")]
    [SerializeField] private PlayerData data;
    
    [Header("Throttle")]
    [Tooltip("Keep as a low number 2-3")]
    [SerializeField] private float maxThrottle = 3;
    [Tooltip("Max speed in mph that matches with the speedometer")]
    [SerializeField] private float maxSpeedMph;

    [Header("Steering")]
    [Tooltip("How fast car turns. Used to increase/decrease steering angle")]
    [SerializeField] private float turnSpeed = 70;
    [Tooltip("Handling of the car, how well it 'sticks' to turning")]
    [SerializeField] private float handling;
    [SerializeField] private float acceleration;

    [Header("Drift Tuning")]
    [SerializeField] private float driftMaxAngle = 20;
    [SerializeField] private float driftHandling = 2;
    [Tooltip("Handling while drift held but no sideways input. Lower = slidier return to forward")]
    [SerializeField] private float driftReleaseHandling = 1;
    [SerializeField] private float driftBlendSpeed = 4;
    [Tooltip("How fast handling eases back to normal grip after drift button released. Lower = longer slide-out")]
    [SerializeField] private float driftReleaseBlendSpeed = 0.5f;
    [SerializeField] private float driftTurnMultiplier = 1.5f;

    [Header("Visual Lean")]
    [Tooltip("Child transform of the car mesh that gets tilted. Leave null to skip")]
    [SerializeField] private Transform carVisual;
    [SerializeField] private float maxLeanAngle = 12f;
    [SerializeField] private float leanSmoothing = 6f;
    [Tooltip("Lean amount while just steering, before drift adds more")]
    [Range(0f, 1f)]
    [SerializeField] private float baseLeanFactor = 0.4f;

    [Header("Reverse")]
    [Tooltip("Max reverse speed in mph (usually slower than forward max speed)")]
    [SerializeField] private float maxReverseSpeed = 15f;

    [Header("Steering Physics")]
    [Tooltip("Below this forward speed, steering is reduced. Prevents spin-in-place. At negative speeds, steering inverts (real car feel)")]
    [SerializeField] private float minSteerSpeed = 3f;

    [Header("Collision Feedback")]
    [Tooltip("Hits with relative velocity above this trigger a brief stun so the impact is visible")]
    [SerializeField] private float collisionStunThreshold = 8f;
    [Tooltip("How long control is disabled after a hard hit (seconds)")]
    [SerializeField] private float collisionStunDuration = 0.3f;

    private InputAction _moveAction;
    private InputAction _driftAction;
    private Rigidbody _rb;
    private float _throttle;
    private Vector2 _inputMovement;
    private bool _drifting;
    private float _baseTurnSpeed;
    private float _driftBlend;
    private float _driftHeldBlend;
    private float _currentLean;
    private Quaternion _visualBaseRotation;
    private float _stunTimer = 0f;

    // First things first
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _moveAction = InputSystem.actions.FindAction("Move");
        _driftAction = InputSystem.actions.FindAction("Drift");
        _baseTurnSpeed = turnSpeed;

        handling = data.handling;
        acceleration = data.acceleration;
        maxSpeedMph = data.maxSpeed;
        
        // Doing this so car doesn't do weird stuff with rotation of the model
        if (carVisual != null)
            _visualBaseRotation = carVisual.localRotation;
    }

    private void Update()
    {
        // Read input
        _inputMovement = _moveAction.ReadValue<Vector2>();
        _throttle = Mathf.Lerp(_throttle, _inputMovement.y, Time.deltaTime * 5f);
        _drifting = _driftAction.IsPressed();

        // Smooth drifting
        float driftAngleTarget = (_drifting && Mathf.Abs(_inputMovement.x) > 0.1f) ? 1f : 0f;
        _driftBlend = Mathf.MoveTowards(_driftBlend, driftAngleTarget, Time.deltaTime * driftBlendSpeed);
        float driftHeldTarget = _drifting ? 1f : 0f;

        float heldRate = _drifting ? driftBlendSpeed : driftReleaseBlendSpeed;
        _driftHeldBlend = Mathf.MoveTowards(_driftHeldBlend, driftHeldTarget, Time.deltaTime * heldRate);
        
        // Car tilt
        if (carVisual != null)
        {
            float leanFactor = Mathf.Lerp(baseLeanFactor, 1f, _driftBlend);
            float targetLean = _inputMovement.x * maxLeanAngle * leanFactor;
            _currentLean = Mathf.Lerp(_currentLean, targetLean, Time.deltaTime * leanSmoothing);
            carVisual.localRotation = _visualBaseRotation * Quaternion.Euler(0f, 0f, _currentLean);
        }
    }

    private void FixedUpdate()
    {
        // While stunned, hand control over to the physics engine so external forces
        // (knockback, hard collisions) actually move the car instead of being erased.
        if (_stunTimer > 0f)
        {
            _stunTimer -= Time.fixedDeltaTime;
            return;
        }

        // Compute SIGNED forward speed so reversing keeps its direction
        float forwardSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward);

        // Slow down when there is no driver input on the throttle
        if (Mathf.Abs(_throttle) < 0.01f && _rb.linearVelocity.magnitude > 0f)
        {
            _rb.linearVelocity = Vector3.Lerp(
                _rb.linearVelocity,
                Vector3.zero,
                _drifting ? Time.fixedDeltaTime * 1.3f : Time.fixedDeltaTime * 2.5f
            );
        }

        // Build the target velocity (along forward / drift direction). Use signed forwardSpeed
        // so negative speed means actually moving backward, not flipped to forward.
        Vector3 forwardVelocity = transform.forward * forwardSpeed;
        float driftAngle = -_inputMovement.x * driftMaxAngle * _driftBlend;
        Vector3 driftDirection = Quaternion.Euler(0f, driftAngle, 0f) * transform.forward;
        Vector3 driftVelocity = driftDirection * forwardSpeed;
        Vector3 targetVelocity = Vector3.Lerp(forwardVelocity, driftVelocity, _driftBlend);

        // lerping handling so car doesn't snap around when going in and out of drifts
        float heldHandling = Mathf.Lerp(driftReleaseHandling, driftHandling, _driftBlend);
        float currentHandling = Mathf.Lerp(handling, heldHandling, _driftHeldBlend);
        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * currentHandling);

        // Throttle: forward acceleration capped by maxSpeedMph
        if (_throttle > 0.001f && _throttle < maxThrottle && forwardSpeed <= maxSpeedMph)
        {
            _rb.AddForce(transform.forward * (_throttle * 10f) * acceleration, ForceMode.Acceleration);
        }
        // Throttle: reverse acceleration capped by maxReverseSpeed (slower than forward)
        else if (_throttle < -0.001f && forwardSpeed > -maxReverseSpeed)
        {
            _rb.AddForce(transform.forward * (_throttle * 10f) * acceleration, ForceMode.Acceleration);
        }

        // Drift turn multiplier (was applied in both branches above; pull it out so it always runs)
        turnSpeed = _baseTurnSpeed * Mathf.Lerp(1f, driftTurnMultiplier, _driftHeldBlend);

        // Steering: scale by signed speed factor (no rotation when nearly stopped, inverted when reversing)
        float steerInput = _inputMovement.x;
        float speedFactor = Mathf.Clamp(forwardSpeed / minSteerSpeed, -1f, 1f);
        float turnAmount = steerInput * turnSpeed * speedFactor * Time.fixedDeltaTime;
        Quaternion rotation = Quaternion.Euler(0f, turnAmount, 0f);
        _rb.MoveRotation(_rb.rotation * rotation);

        // Kill residual angular velocity from collisions so the car doesn't keep spinning by itself.
        // During stun, FixedUpdate returns early above so this line is skipped, letting hits visibly rotate the car.
        _rb.angularVelocity = Vector3.zero;
    }

    // Briefly disable the controller's velocity / rotation override so external physics
    // (knockback, ramp launches, collision response) can actually move the car.
    public void Stun(float duration)
    {
        if (duration > _stunTimer) _stunTimer = duration;
    }

    // Auto-stun on hard impacts so collisions feel weighty
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > collisionStunThreshold)
        {
            Stun(collisionStunDuration);
        }
    }

    // ========== IMovementController ==========

    public Vector3 GetMovement()
    {
        return Vector3.zero;
    }

    public float GetMaxSpeed()
    {
        return 0f;
    }
}
