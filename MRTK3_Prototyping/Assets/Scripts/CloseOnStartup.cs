using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseOnStartup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //transform.position = Vector3.back * 10;
        StartCoroutine(CloseObject());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CloseObject() {
		yield return null;
		//yield return null;

        //transform.position = Vector3.zero;
		gameObject.SetActive(false);
	}
}
