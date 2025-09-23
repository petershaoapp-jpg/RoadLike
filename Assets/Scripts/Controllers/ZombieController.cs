using UnityEngine;

public class ZombieController : MonoBehaviour, IMovementController
{
    private GameObject _player;

    private void Start() {
        _player = GameObject.Find("Car");
    }

    public Vector3 GetMovement()
    {
        Vector3 direction = _player.transform.position - transform.position;
        return direction.normalized;
    }
}
