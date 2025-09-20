using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour, IMovementController
{
    [SerializeField] private float _rotationSpeed = 10f;

    private Rigidbody _rb;
    private InputAction _moveAction;
    [SerializeField] private WheelCollider[] _wheelColliders;

    private void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float steerAngle = _moveAction.ReadValue<Vector2>().x * 30;
        _wheelColliders[0].steerAngle = steerAngle;
        _wheelColliders[1].steerAngle = steerAngle;
    }

    public Vector3 GetMovement()
    {
        return transform.forward * _moveAction.ReadValue<Vector2>().y;
    }
}
