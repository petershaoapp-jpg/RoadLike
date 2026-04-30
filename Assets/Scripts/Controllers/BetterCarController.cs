using UnityEngine;
using UnityEngine.InputSystem;

public class BetterCarController : MonoBehaviour, IMovementController
{
    // Tunable parameters
    [Header("Throttle")]
    [Tooltip("Keep as a low number 2-3")]
    [SerializeField] private float maxThrottle = 3;
    [Tooltip("Max speed in mph that matches with the speedometer")]
    [SerializeField] private float maxSpeedMph = 45;

    [Header("Steering")]
    [Tooltip("How fast car turns. Used to increase/decrease steering angle")]
    [SerializeField] private float turnSpeed = 70;
    [Tooltip("Handling of the car, how well it 'sticks' to turning")]
    [SerializeField] private float handling = 80;
    [SerializeField] private float acceleration = 3.5f;

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

    private InputAction _moveAction;
    private InputAction _driftAction;
    private Rigidbody _rb;
    private float _throttle;
    private Vector2 _inputMovement;
    private AudioSource _audioSource;
    private bool _drifting;
    private float _baseTurnSpeed;
    private float _driftBlend;
    private float _driftHeldBlend;
    private float _currentLean;
    private Quaternion _visualBaseRotation;

    // First things first
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _rb = GetComponent<Rigidbody>();
        _moveAction = InputSystem.actions.FindAction("Move");
        _driftAction = InputSystem.actions.FindAction("Drift");
        _baseTurnSpeed = turnSpeed;

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
        // Slow down when no input
        if (_inputMovement.y == 0 && _rb.linearVelocity.magnitude >= 0)
        {
            _rb.linearVelocity = Vector3.Lerp(
                _rb.linearVelocity,
                Vector3.zero,
                _drifting ? Time.fixedDeltaTime * 1.3f : Time.fixedDeltaTime * 2.5f
            );
        }

        // Smoother grip for drifting
        float speed = _rb.linearVelocity.magnitude;
        Vector3 forwardVelocity = transform.forward * speed;
        float driftAngle = -_inputMovement.x * driftMaxAngle * _driftBlend;
        Vector3 driftDirection = Quaternion.Euler(0f, driftAngle, 0f) * transform.forward;
        Vector3 driftVelocity = driftDirection * speed;
        Vector3 targetVelocity = Vector3.Lerp(forwardVelocity, driftVelocity, _driftBlend);

        // lerping handling so car doesn't snap around when going in and out of drifts
        float heldHandling = Mathf.Lerp(driftReleaseHandling, driftHandling, _driftBlend);
        float currentHandling = Mathf.Lerp(handling, heldHandling, _driftHeldBlend);
        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * currentHandling);

        // Apply throttle force while within speed cap
        if (_throttle > 0.001f && _throttle < maxThrottle && _rb.linearVelocity.magnitude <= maxSpeedMph)
        {
            turnSpeed = _baseTurnSpeed * Mathf.Lerp(1f, driftTurnMultiplier, _driftHeldBlend);

            _rb.AddForce(transform.forward * (_throttle * 10f) * acceleration, ForceMode.Acceleration);

            // Update sound pitch based on speed
            _audioSource.pitch = _rb.linearVelocity.magnitude;
        }
        else
        {
            turnSpeed = _baseTurnSpeed * Mathf.Lerp(1f, driftTurnMultiplier, _driftHeldBlend);
        }

        // Rotate car with steering angle
        float steerInput = _inputMovement.x;
        float turnAmount = steerInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion rotation = Quaternion.Euler(0f, turnAmount, 0f);
        _rb.MoveRotation(_rb.rotation * rotation);
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
