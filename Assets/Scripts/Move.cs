using UnityEngine;

[RequireComponent(typeof(IMovementController),typeof(Ground))]
public class Move : MonoBehaviour
{
    [SerializeField] private float speed = 0;
    [SerializeField] private float maxSpeed = 30f;

    [SerializeField] private PlayerData data;

    private IMovementController _controller;
    private Rigidbody _rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controller = GetComponent<IMovementController>();
        _rb = GetComponent<Rigidbody>();

        if (gameObject.name == "Car") {
          maxSpeed = data.maxSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(_rb.linearVelocity.magnitude) > maxSpeed) return;

        _rb.AddForce(_controller.GetMovement() * speed * Time.deltaTime);
    }
}
