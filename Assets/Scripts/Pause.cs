using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    private InputAction _pauseAction;
    [SerializeField] private GameObject _pauseScreen;
    private bool _paused = false;

    private void Start()
    {
        _pauseAction = InputSystem.actions.FindAction("Pause");
    }

    private void Update()
    {
        if (_pauseAction.triggered)
        {
            _paused = !_paused;
            _pauseScreen.SetActive(_paused);

            if (_paused)
            {
                Time.timeScale = 0;
            } else
            {
                Time.timeScale = 1;
            }
        }
    }
}
