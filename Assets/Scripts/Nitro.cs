using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Nitro : MonoBehaviour
{
    [SerializeField] private float nitroBoost = 500000;

    private int _nitros;
    private InputAction _nitroInput;
    private Rigidbody _rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _nitros = PlayerData.maxNitros;
        _nitroInput = InputSystem.actions.FindAction("Nitro");
        _rb = GetComponent<Rigidbody>();

        _nitroInput.performed += ActivateNitro;
    }

    private void ActivateNitro(InputAction.CallbackContext callbackContext)
    {
        if (_nitros > 0)
        {
            _nitros--;
            _rb.AddForce(transform.forward * nitroBoost, ForceMode.Impulse);
            StartCoroutine(ReplenishNitro());
        }
    }

    private IEnumerator ReplenishNitro()
    {
        yield return new WaitForSeconds(PlayerData.nitroReplenishTime);
        _nitros++;
    }
}
