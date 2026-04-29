using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BetterCarController : MonoBehaviour, IMovementController
{
    // Make adjustable parameters
    [Header("Keep as a low number 2-3")]
    [SerializeField] private float maxThrottle = 20;
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
    private InputAction _driftAction;
    private Rigidbody _rb;
    private float _throttle;
    private Vector2 _inputMovement;
    private Vector2 _steerAngle;
    private AudioSource _audioSource;
    private float _driftAmount;
    private bool _drifting;
    private float _bTurnSpeed;
    
    // using awake to make it get the things early
    private void Awake() 
    {
        // setup for sound
        _audioSource = GetComponent<AudioSource>();

        // get the car body and input action
        _rb = GetComponent<Rigidbody>();
        _moveAction = InputSystem.actions.FindAction("Move");
        _driftAction = InputSystem.actions.FindAction("Drift");
        _rb.linearDamping = 3f;
        _bTurnSpeed = turnSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for input on update
        _inputMovement = _moveAction.ReadValue<Vector2>();
        _throttle = Mathf.Lerp(_throttle, _inputMovement.y, Time.deltaTime * 5f);
        _drifting = _driftAction.IsPressed();
        Debug.Log("Throttle: " + _throttle.ToString());
    }

    // Use fixed update for applying movement
    void FixedUpdate()
    {
        // Slow down when no input
        if (_inputMovement.y == 0 && _rb.linearVelocity.magnitude >= 0)
        {
            _rb.linearVelocity = Vector3.Lerp(
                _rb.linearVelocity,
                Vector3.zero,
                Time.fixedDeltaTime * 2.5f // stopping speed
            );
        }

        // If there is input to move/if we are able to move
        if (_throttle > 0.001f && _throttle < maxThrottle && _rb.linearVelocity.magnitude < maxSpeedMph)
        {
            turnSpeed = _bTurnSpeed;
            // Make car move
            // get the forward velocity of the car
            Vector3 forwardVelocity = transform.forward * _rb.linearVelocity.magnitude;
            // set the linear velocity of the car in a lerp of the current velocity and the forward velocity
            if (!_drifting) 
            {
                _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, forwardVelocity, Time.fixedDeltaTime * handling);
            }
            // make car go forward
            //TODO work on making entering and exiting drifting smoother by lerping the drift direction and stuff
            float driftDirection = Mathf.Clamp(_inputMovement.x, -1, 1);
            /*
            if (_drifting && _inputMovement.x != 0)
            {
                turnSpeed = turnSpeed * 1.5f;
                if (_inputMovement.x < 0)
                {
                    _rb.AddForce(transform.forward + transform.right * (_throttle * 6) * acceleration, ForceMode.Acceleration);
                }
                else
                {
                    _rb.AddForce(transform.forward + (transform.right * -1) * (_throttle * 6) * acceleration, ForceMode.Acceleration);
                }
                _rb.AddForce(transform.forward * (_throttle * 8) * acceleration, ForceMode.Acceleration);
            }
            else
            {
                _rb.AddForce(transform.forward * (_throttle * 10) * acceleration, ForceMode.Acceleration);
            }
            */

            // update sound pitch based on speed
            _audioSource.pitch = _rb.linearVelocity.magnitude;

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
