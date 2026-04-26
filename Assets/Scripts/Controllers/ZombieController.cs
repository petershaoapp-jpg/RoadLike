using UnityEngine;

public class ZombieController : MonoBehaviour, IMovementController
{
    private GameObject _player;

    private void Start() {
        _player = GameObject.Find("Car");
    }

    public Vector3 GetMovement()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) > 100) return Vector3.zero;
        
        Vector3 direction = _player.transform.position - transform.position;
        return direction.normalized;
    }
}
