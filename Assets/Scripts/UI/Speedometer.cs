using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    private GameObject _car;
    private TMP_Text _tm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       _car = GameObject.Find("Car");
        _tm = gameObject.GetComponent<TMP_Text>();

    }

    // Update is called once per frame
    void Update()
    {
      float speed = _car.GetComponent<Rigidbody>().linearVelocity.magnitude;
      _tm.text = Mathf.Floor(speed) + " mph";
    }
}
