using UnityEngine;

public class EnemyDie : MonoBehaviour, IDie
{
    public void OnDie()
    {
        Debug.Log("[INFO]: Enemy died");
        Destroy(gameObject); // Destroying the attached game object by passing gameObject
    }
}
