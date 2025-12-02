using UnityEngine;
using UnityEngine.InputSystem;

public class CarControllerArcade : MonoBehaviour, IMovementController
{
    [SerializeField] private PlayerData data;

    [Header("Wheels (0/1 = Front, 2/3 = Rear)")]
    [SerializeField] private WheelCollider[] _wheelColliders;

    [Header("Steer")]
    [SerializeField] float maxSteerAngle = 40f;                 // Big angle = arcade
    [SerializeField] float steerResponsiveness = 12f;           // How fast we reach target
    [SerializeField] float steerReturn = 8f;                    // Return-to-center when no input
    [SerializeField] AnimationCurve speedToSteer =              // Less steer at high speed
        AnimationCurve.EaseInOut(0, 1, 40, 0.35f);              // x=speed m/s, y=steer scale

    [Header("Go/Stop")]
    [SerializeField] float accelTorque = 1600f;                 // Nm applied to driven wheels
    [SerializeField] float reverseTorque = 1200f;
    [SerializeField] float brakeTorque = 2200f;
    [SerializeField] float coastBrake = 180f;                   // Light drag when no input
    [SerializeField] float maxSpeedKPH = 220f;

    [Header("Arcade Stability")]
    [SerializeField] float lateralGripStiffness = 2.2f;         // Sideways grip (bigger = stickier)
    [SerializeField] float forwardGripStiffness = 1.6f;         // Forward grip
    [SerializeField] float downforcePerKPH = 0.5f;              // N of downforce per kph
    [SerializeField] float yawStability = 3.0f;                 // Damps spin-outs

    [Header("Feel")]
    [SerializeField] float inputSmoothing = 10f;                // Smooth throttle/brake input
    [SerializeField] float nitroMultiplier = 1.35f;             // Optional boost

    private InputAction _moveAction;
    private Rigidbody _rb;
    private float _steerAngle;
    private float _throttle;   // smoothed forward input
    private float _brake;      // smoothed brake (implicit when reversing or no input)

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _moveAction = InputSystem.actions.FindAction("Move");

        // Keep the body lively
        _rb.linearDamping = 0.02f;
        _rb.angularDamping = 0.2f;

        // Make friction curves “arcade sticky”
        SetupFriction(_wheelColliders[0], forwardGripStiffness, lateralGripStiffness);
        SetupFriction(_wheelColliders[1], forwardGripStiffness, lateralGripStiffness);
        SetupFriction(_wheelColliders[2], forwardGripStiffness, lateralGripStiffness);
        SetupFriction(_wheelColliders[3], forwardGripStiffness, lateralGripStiffness);
    }

    private void SetupFriction(WheelCollider wc, float forwardStiff, float sidewaysStiff)
    {
        var f = wc.forwardFriction;
        f.extremumSlip = 0.4f;    // bite point
        f.asymptoteSlip = 0.8f;   // slides after here
        f.extremumValue = 1.0f;
        f.asymptoteValue = 0.6f;
        f.stiffness = forwardStiff;
        wc.forwardFriction = f;

        var s = wc.sidewaysFriction;
        s.extremumSlip = 0.2f;
        s.asymptoteSlip = 0.6f;
        s.extremumValue = 1.0f;
        s.asymptoteValue = 0.75f;
        s.stiffness = sidewaysStiff;
        wc.sidewaysFriction = s;
    }

    private void Update()
    {
        // Read input: x = steer, y = throttle/brake
        Vector2 move = _moveAction.ReadValue<Vector2>();

        // Smooth inputs so it feels premium
        _throttle = Mathf.Lerp(_throttle, Mathf.Clamp(move.y, -1f, 1f), Time.deltaTime * inputSmoothing);

        // Steer target scaled by speed (less twitchy at high speed)
        float speedMS = _rb.linearVelocity.magnitude;
        float steerScale = speedToSteer.Evaluate(speedMS);
        float targetSteer = move.x * maxSteerAngle * steerScale;

        // Quick response toward target, quick return when no input
        float lerpRate = Mathf.Lerp(steerReturn, steerResponsiveness, Mathf.Abs(move.x));
        _steerAngle = Mathf.Lerp(_steerAngle, targetSteer, Time.deltaTime * lerpRate);

        // Apply to front wheels
        _wheelColliders[0].steerAngle = _steerAngle;
        _wheelColliders[1].steerAngle = _steerAngle;
    }

    private void FixedUpdate()
    {
        float speedKPH = _rb.linearVelocity.magnitude * 3.6f;
        float speedFactor = Mathf.InverseLerp(0f, maxSpeedKPH, speedKPH);
        float torqueFalloff = 1f - Mathf.SmoothStep(0f, 1f, speedFactor); // less torque as speed rises

        // Decide torque vs brake from throttle sign
        float driveTorque = 0f;
        float appliedBrake = 0f;

        if (_throttle > 0.01f)
        {
            driveTorque = _throttle * accelTorque * torqueFalloff;
            appliedBrake = 0f;
        }
        else if (_throttle < -0.01f)
        {
            // Reverse when nearly stopped, otherwise brake
            if (speedKPH < 5f)
                driveTorque = _throttle * reverseTorque;
            else
                appliedBrake = Mathf.Lerp(appliedBrake, brakeTorque * Mathf.Abs(_throttle), 1f);
        }
        else
        {
            appliedBrake = coastBrake; // arcade coast drag
        }

        // Hard cap: stop adding velocity above max
        if (speedKPH >= maxSpeedKPH) driveTorque = 0f;

        // Apply to REAR wheels (arcade RWD feel)
        _wheelColliders[2].motorTorque = driveTorque;
        _wheelColliders[3].motorTorque = driveTorque;

        // Brakes to all wheels for stability
        for (int i = 0; i < 4; i++)
            _wheelColliders[i].brakeTorque = appliedBrake;

        // Downforce for grip at speed
        float downforce = speedKPH * downforcePerKPH;
        _rb.AddForce(-transform.up * downforce, ForceMode.Force);

        // Yaw stability: counter small spins
        Vector3 localAngVel = transform.InverseTransformDirection(_rb.angularVelocity);
        float yaw = localAngVel.y;
        _rb.AddRelativeTorque(0f, -yaw * yawStability, 0f, ForceMode.Acceleration);
    }

    // Optional “nitro” you can call from your nitro script:
    public void PulseBoost(float seconds = 0.75f)
    {
        StartCoroutine(Boost(seconds));
    }
    private System.Collections.IEnumerator Boost(float seconds)
    {
        float t = 0f;
        float baseAccel = accelTorque;
        while (t < seconds)
        {
            accelTorque = baseAccel * nitroMultiplier;
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        accelTorque = baseAccel;
    }

    // Keep your interface alive:
    public Vector3 GetMovement() => transform.forward * _throttle;
    public float GetMaxSpeed() => data.maxSpeed; // or return maxSpeedKPH if you want to consolidate
}
