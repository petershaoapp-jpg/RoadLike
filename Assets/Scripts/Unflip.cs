using UnityEngine;
using UnityEngine.InputSystem;

public class Unflip : MonoBehaviour
{
  private InputAction _input;
  private Rigidbody _rb;

  [SerializeField] private float upwardsForce;
  [SerializeField] private float torque;

  private void Awake() {
    _input = InputSystem.actions.FindAction("Unflip");
    _rb = GetComponent<Rigidbody>();
  }

  private void Update() {
    if (_input.triggered && _rb.linearVelocity.magnitude < 1) {
      _rb.AddForce(new Vector3(0,upwardsForce,0));
      _rb.AddTorque(torque * transform.forward);
    }
  }
}
