using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour, IMovementController
{
    [SerializeField] private WheelCollider[] _wheelColliders;

    private InputAction _moveAction;
    private float _currentSteerAngle = 0;
    private float _targetSteerAngle;

    private void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        _targetSteerAngle = _moveAction.ReadValue<Vector2>().x * 10;
        _currentSteerAngle = Mathf.Lerp(_currentSteerAngle, _targetSteerAngle, Time.deltaTime * 5);

        _wheelColliders[0].steerAngle = _currentSteerAngle;
        _wheelColliders[1].steerAngle = _currentSteerAngle;
    }

    public Vector3 GetMovement()
    {
        return transform.forward * _moveAction.ReadValue<Vector2>().y;
    }

    public float GetMaxSpeed()
    {
        return PlayerData.maxSpeed;
    }
}
