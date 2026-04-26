using System;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] private float speed = 2;
    
    private void Update()
    {
        transform.Rotate(new Vector3(Time.deltaTime * speed, 0, 0));
    }
}
