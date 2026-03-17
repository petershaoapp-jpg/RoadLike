using UnityEngine;

public class Ground : MonoBehaviour
{
    public Transform ground { get; private set; }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Ground")) return;

        EvaluateCollision(other);
    }

    private void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            
            if (normal.y > 0.95)
            {
                ground = collision.transform;
            }
        }
    }
}
