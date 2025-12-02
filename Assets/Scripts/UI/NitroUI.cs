using UnityEngine;
using System.Collections.Generic;

public class NitroUI : MonoBehaviour
{
    private Nitro _nitro;
    private GameObject _canvas;
    private List<GameObject> _images;
    private int _xpos = 0;

    [SerializeField] private GameObject image;

    private void Start()
    {
       _nitro = GameObject.Find("Car").GetComponent<Nitro>();
       _canvas = GameObject.Find("Canvas");
       _images = new List<GameObject>();
    }

    private void Update()
    {
      _xpos = 35;

      foreach (GameObject img in _images) {
        Destroy(img);
      }
      
      _images = new List<GameObject>();

      for (int i = 0; i < Mathf.Floor(_nitro.nitros); i++) {
        GameObject charge = Instantiate(image,new Vector3(_xpos,35,0),Quaternion.identity);
        charge.transform.parent = _canvas.transform;
        _images.Add(charge);
        _xpos += 60;
      }
    }
}
