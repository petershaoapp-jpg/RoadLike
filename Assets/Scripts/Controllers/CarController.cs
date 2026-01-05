using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour, IMovementController
{
    // Setup private vars (w/serialization in order to tune easily)
    [SerializeField] private PlayerData data;
    [Header("Wheels (0/1 = Front, 2/3 = Rear)")]
    [SerializeField] private WheelCollider[] wheelColliders;

    [Header("Steer")]
    [SerializeField] private float maxSteerAngle = 40f;
    [SerializeField] private float steerResponsiveness = 12f;
    [SerializeField] private float steerReturn = 8f;
    [SerializeField] private AnimationCurve speedToSteer = AnimationCurve.EaseInOut(0, 1, 40, 0.35f);

    [Header("Go/Stop")]
    [SerializeField] private float accelTorque = 1600f;
    [SerializeField] private float reverseTorque = 1200f;
    [SerializeField] private float brakeTorque = 2200f;
    [SerializeField] private float coastBrake = 180f;
    [SerializeField] private float maxSpeedKPH = 220f;

    [Header("Arcade Stuff")]
    [SerializeField] private float lateralGripStiffness = 2.2f;
    [SerializeField] private float forwardGripStiffness = 1.6f;
    [SerializeField] private float downforcePerKPH = 0.5f;
    [SerializeField] private float yawStability = 3.0f;

    [Header("Feel")]
    [SerializeField] private float inputSmoothing = 10f;
    [SerializeField] private float nitroMultiplier = 1.35f;

    private InputAction _moveAction;
    private Rigidbody _rb;
    private float _steerAngle;
    private float _throttle;


    // Setup movement + wheel colliders with tuned values 
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _moveAction = InputSystem.actions.FindAction("Move");

        _rb.linearDamping = 0.02f;
        _rb.angularDamping = 0.2f;

        for (int i = 0; i < wheelColliders.Length; i++)
        {
            SetupFriction(wheelColliders[i], forwardGripStiffness, lateralGripStiffness);
        }
    }

    private void SetupFriction(WheelCollider wheelCollider, float forwardStiffness, float sidewaysStiffness)
    {
        var forward = wheelCollider.forwardFriction;
        forward.extremumSlip = 0.4f;
        forward.asymptoteSlip = 0.8f;
        forward.extremumValue = 1.0f;
        forward.asymptoteValue = 0.6f;
        forward.stiffness = forwardStiffness;
        wheelCollider.forwardFriction = forward;

        var sideways = wheelCollider.sidewaysFriction;
        sideways.extremumSlip = 0.2f;
        sideways.asymptoteSlip = 0.6f;
        sideways.extremumValue = 1.0f;
        sideways.asymptoteValue = 0.75f;
        sideways.stiffness = sidewaysStiffness;
        wheelCollider.sidewaysFriction = sideways;
    }


    private void Update()
    {
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        _throttle = Mathf.Lerp(_throttle, Mathf.Clamp(moveInput.y, -1f, 1f), Time.deltaTime * inputSmoothing);

        // Update wheels to turn/steer
        float speedMetersPerSecond = _rb.linearVelocity.magnitude;
        float steerScale = speedToSteer.Evaluate(speedMetersPerSecond);
        float targetSteer = moveInput.x * maxSteerAngle * steerScale;
        float steerRate = Mathf.Lerp(steerReturn, steerResponsiveness, Mathf.Abs(moveInput.x));

        _steerAngle = Mathf.Lerp(_steerAngle, targetSteer, Time.deltaTime * steerRate);
        wheelColliders[0].steerAngle = _steerAngle;
        wheelColliders[1].steerAngle = _steerAngle;
    }

    private void FixedUpdate()
    {
        float speedKPH = _rb.linearVelocity.magnitude * 3.6f;
        float speedFactor = Mathf.InverseLerp(0f, maxSpeedKPH, speedKPH);
        float torqueFalloff = 1f - Mathf.SmoothStep(0f, 1f, speedFactor);

        float driveTorque = 0f;
        float appliedBrake = 0f;

        // Handle throttle and breaking
        if (_throttle > 0.01f)
        {
            driveTorque = _throttle * accelTorque * torqueFalloff;
        }
        else if (_throttle < -0.01f)
        {
            if (speedKPH < 5f)
            {
                driveTorque = _throttle * reverseTorque;
            }
            else
            {
                // Lerp for applying brake for smoother movement
                appliedBrake = Mathf.Lerp(appliedBrake, brakeTorque * Mathf.Abs(_throttle), 1f);
            }
        }
        else
        {
            appliedBrake = coastBrake;
        }

        if (speedKPH >= maxSpeedKPH)
        {
            driveTorque = 0f;
        }

        // Actually apply the handled torque
        wheelColliders[2].motorTorque = driveTorque;
        wheelColliders[3].motorTorque = driveTorque;

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].brakeTorque = appliedBrake;
        }


        // Apply forces to rigidbody of car
        float downforce = speedKPH * downforcePerKPH;
        _rb.AddForce(-transform.up * downforce, ForceMode.Force);

        Vector3 localAngularVelocity = transform.InverseTransformDirection(_rb.angularVelocity);
        float yaw = localAngularVelocity.y;
        _rb.AddRelativeTorque(0f, -yaw * yawStability, 0f, ForceMode.Acceleration);
    }

    public void PulseBoost(float seconds = 0.75f)
    {
        // Use coroutine because we are pulsing the boost
        StartCoroutine(Boost(seconds));
    }

    private IEnumerator Boost(float seconds)
    {
        float elapsed = 0f;
        float baseAccel = accelTorque;

        while (elapsed < seconds) // While boosting, apply multiplier to accelTorque and wait for next phys. update
        {
            accelTorque = baseAccel * nitroMultiplier;
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        accelTorque = baseAccel;
    }


    // Helpers
    public Vector3 GetMovement()
    {
        return transform.forward * _throttle;
    }

    public float GetMaxSpeed()
    {
        return data.maxSpeed;
    }
}
