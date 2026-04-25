using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Nitro : MonoBehaviour
{
    [SerializeField] private float nitroBoost = 500000;

    public int nitros;
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
        yield return new WaitForSeconds(data.nitroReplenishTime);
        Debug.Log("Replenish Nitro");
        nitros++;
    }
}
