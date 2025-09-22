using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour, IMovementController
{
    [SerializeField] private WheelCollider[] _wheelColliders;

    private Rigidbody _rb;
    private InputAction _moveAction;
    private float _currentSteerAngle = 0;
    private float _targetSteerAngle;

    private void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _targetSteerAngle = _moveAction.ReadValue<Vector2>().x * 10;
        _currentSteerAngle = Mathf.Lerp(_currentSteerAngle, _targetSteerAngle, Time.deltaTime * 5);

        _wheelColliders[0].steerAngle = _currentSteerAngle;
        _wheelColliders[1].steerAngle = _currentSteerAngle;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            WheelFrictionCurve sidewaysFriction = _wheelColliders[2].sidewaysFriction;
            sidewaysFriction.stiffness = 1.2f;
            _wheelColliders[2].sidewaysFriction = sidewaysFriction;
            _wheelColliders[2].sidewaysFriction = sidewaysFriction;

        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            WheelFrictionCurve sidewaysFriction = _wheelColliders[2].sidewaysFriction;
            sidewaysFriction.stiffness = 2;
            _wheelColliders[2].sidewaysFriction = sidewaysFriction;
            _wheelColliders[2].sidewaysFriction = sidewaysFriction;


            _rb.AddForce(transform.forward * 100000, ForceMode.Impulse);
        }
    }

    public Vector3 GetMovement()
    {
        return transform.forward * _moveAction.ReadValue<Vector2>().y;
    }
}
