using UnityEngine;

public interface IMovementController
{
    public Vector3 GetMovement();
    public float GetMaxSpeed();
}