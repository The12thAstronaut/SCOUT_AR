using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonPalm : MonoBehaviour
{
    public bool isRotating { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating) {
            transform.RotateAround(transform.position, transform.up, 5 * Time.deltaTime);
        }
    }
}
