using UnityEngine;

[RequireComponent(typeof(IMovementController))]
public class MoveConstantSpeed : MonoBehaviour
{
    private IMovementController _controller;
    private Rigidbody _rb;

    private void Start()
    {
        _controller = GetComponent<IMovementController>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _rb.linearVelocity = _rb.linearVelocity + (_controller.GetMovement() * Time.deltaTime * 10);
    }
}