using UnityEngine;

public class Timer : MonoBehaviour
{
    public float time = 0;   
    private bool _hasStarted = false;

    private void Update()
    {
      if (Input.anyKeyDown) _hasStarted = true;
      if (!_hasStarted) return;

      time += Time.deltaTime;
    }
}
