using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BetterCarController : MonoBehaviour, IMovementController
{
    // Make adjustable parameters
    [Header("Keep as a low number 2-3")]
    [SerializeField] private float maxThrottle = 2;
    [Header("Max speed in mph that matches with spedometer")]
    [SerializeField] private float maxSpeedMph = 45;
    [Header("How fast car turns\ncan be used to increase/decrease steering angle")]
    [SerializeField] private float turnSpeed = 50;
    [Header("Handling of the car, how well it 'sticks' to turning")]
    [SerializeField] private float handling = 50f;
    [Header("Acceleration, self explanatory")]
    [SerializeField] private float acceleration = 10;

    // other vars and stuff
    private InputAction _moveAction;
    private Rigidbody _rb;
    private float _throttle;
    private Vector2 _inputMovement;
    private Vector2 _steerAngle;
    
    // using awake to make it get the things early
    private void Awake() 
    {
        // get the car body and input action
        _rb = GetComponent<Rigidbody>();
        _moveAction = InputSystem.actions.FindAction("Move");
        _rb.linearDamping = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for input on update
        _inputMovement = _moveAction.ReadValue<Vector2>();
        _throttle = Mathf.Lerp(_throttle, _inputMovement.y, Time.deltaTime * 5f);
        Debug.Log("Throttle: " + _throttle.ToString());
    }

    // Use fixed update for applying movement
    void FixedUpdate()
    {
        // Slow down when no input
        if (Mathf.Abs(_throttle) < 0.01f)
        {
            _rb.linearVelocity = Vector3.Lerp(
                _rb.linearVelocity,
                Vector3.zero,
                Time.fixedDeltaTime * 3f // stopping speed
            );
        }

        if (_throttle > 0.001f && _throttle < maxThrottle && _rb.linearVelocity.magnitude < maxSpeedMph)
        {
            // Calculate drive force
            float driveForce = _throttle * acceleration;

            // Make car move
            Vector3 forwardVelocity = transform.forward * _rb.linearVelocity.magnitude;
            _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, forwardVelocity, Time.fixedDeltaTime * handling);
            _rb.AddForce(transform.forward * _throttle * acceleration, ForceMode.Acceleration);

            Debug.Log("input movement: " + _inputMovement.x.ToString() + ", " + _inputMovement.y.ToString());
        }

        // Make car rotate w/steering angle
        //TODO: Make steering smoother
        float steerInput = _inputMovement.x;
        float turnAmount = steerInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion _rotation = Quaternion.Euler(0f, turnAmount, 0f);
        _rb.MoveRotation(_rb.rotation * _rotation);

        //TODO: Make car slightly tilt in steering direction
    }

    // Helpers
    public Vector3 GetMovement()
    {
        return new Vector3(0,0,0);
    }

    public float GetMaxSpeed()
    {
        return 0.0f;
    }
}
