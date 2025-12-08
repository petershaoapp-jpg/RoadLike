using UnityEngine;

[RequireComponent(typeof(IMovementController))]
public class MoveConstantSpeed : MonoBehaviour
{
    private IMovementController _controller;
    private Rigidbody _rb;
    [SerializeField] private float speed = 50f;

    private void Start()
    {
        _controller = GetComponent<IMovementController>();
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = _controller.GetMovement();
        Vector3 newVelocity = moveDirection * speed;
        newVelocity.y = _rb.linearVelocity.y;
        _rb.linearVelocity = newVelocity;
    }
}
