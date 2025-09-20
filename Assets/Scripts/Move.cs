using UnityEngine;

[RequireComponent(typeof(IMovementController),typeof(Ground))]
public class Move : MonoBehaviour
{
    [SerializeField] private float speed = 0;
    [SerializeField] private float maxSpeed = 30f;

    private IMovementController _controller;
    private Rigidbody _rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controller = GetComponent<IMovementController>();
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_rb.linearVelocity.x > maxSpeed) return;

        _rb.AddForce(_controller.GetMovement() * speed * Time.deltaTime);
    }
}
