using UnityEngine;

public class EnemyDie : MonoBehaviour, IDie
{
    public void OnDie()
    {
        Destroy(gameObject); // Destroying the attached game object by passing gameObject
    }
}
