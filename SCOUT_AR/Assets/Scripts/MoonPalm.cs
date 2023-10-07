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
        if (!transform.parent.parent.GetComponentInChildren<LodMeshLoader>().IsLoaded()) return;

        if (isRotating) {
			transform.parent.rotation = Quaternion.identity;
			transform.RotateAround(transform.position, transform.up, 2 * Time.deltaTime);
        }
    }
}
