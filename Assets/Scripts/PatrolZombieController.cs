using UnityEngine;

public class PatrolZombieController : MonoBehaviour, IMovementController
{
    [Header("Patrol Settings")][Tooltip("How far the zombie walks to the left and right from its starting point.")]
    [SerializeField] private float patrolDistance = 10f;

    private Vector3 _pointA;
    private Vector3 _pointB;
    private Vector3 _targetPoint;

    private void Start()
    {
        // Calculate two waypoints based on the zombie's initial position and its local right/left direction.
        _pointA = transform.position + (transform.right * patrolDistance);
        _pointB = transform.position - (transform.right * patrolDistance);
        
        // Start by moving towards Point A
        _targetPoint = _pointA;
    }

    public Vector3 GetMovement()
    {
        // [BUG FIX] Ignore the Y-axis (height) to prevent the zombie from getting stuck in pits.
        // We create flat versions (Y = 0) of the current position and target position.
        Vector3 currentPosFlat = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 targetPosFlat = new Vector3(_targetPoint.x, 0f, _targetPoint.z);

        // Check if the zombie is horizontally close enough to the target point (ignoring height)
        if (Vector3.Distance(currentPosFlat, targetPosFlat) < 1.5f)
        {
            // Switch target to the opposite point
            if (_targetPoint == _pointA)
            {
                _targetPoint = _pointB;
            }
            else
            {
                _targetPoint = _pointA;
            }
        }

        // Calculate the direction towards the current target point
        Vector3 direction = _targetPoint - transform.position;
        
        // Keep the movement strictly horizontal so it doesn't try to fly or dig into the ground
        direction.y = 0;
        
        // Return the normalized direction vector to the movement script
        return direction.normalized;
    }
}