using UnityEngine;

public class VineWall : MonoBehaviour
{
    // This script marks a segment as part of a vine wall.
    // It's a pure physical barrier - no damage on collision.
    // Lifetime is managed by TreeBossController (destroys the parent object).
    //
    // The Cube primitive already has a BoxCollider, so the car
    // will collide with it and be blocked. No extra logic needed.
}
