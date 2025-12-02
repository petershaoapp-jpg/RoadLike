using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Nitro : MonoBehaviour
{
    [SerializeField] private float nitroBoost = 500000;

    public float nitros;
    private InputAction _nitroInput;
    private Rigidbody _rb;
    [SerializeField] private PlayerData data;

    private void Start()
    {
        nitros = data.maxNitros;
        _nitroInput = InputSystem.actions.FindAction("Nitro");
        _rb = GetComponent<Rigidbody>();

        _nitroInput.performed += ActivateNitro;
    }

    private void ActivateNitro(InputAction.CallbackContext callbackContext)
    {
        if (nitros >= 1)
        {
            nitros--;
            _rb.AddForce(transform.forward * nitroBoost, ForceMode.Impulse);
            StartCoroutine(ReplenishNitro());
        }
    }

    private IEnumerator ReplenishNitro()
    {
      for (int i = 0; i < 10; i++) {
        yield return new WaitForSeconds(data.nitroReplenishTime/10);
        nitros += .1f;
      }
    }
}
