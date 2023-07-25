using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPin : MonoBehaviour {
    public GameObject rightHandDetector { get; set; }
    public GameObject leftHandDetector { get; set; }

	// Start is called before the first frame update
	void Start()
    {
        //rightHandDetector = FindFirstObjectByType<NearInteractionModeDetector>().gameObject;
		//leftHandDetector = FindObjectsByType<NearInteractionModeDetector>(FindObjectsSortMode.InstanceID)[1].gameObject;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopPlacement() {
        transform.GetComponent<PressableButton>().enabled = true;
        transform.GetComponent<TapToPlace>().enabled = false;

		rightHandDetector.SetActive(true);
		leftHandDetector.SetActive(true);
	}
}
