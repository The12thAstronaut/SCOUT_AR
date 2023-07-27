using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPin : MonoBehaviour {
    public GameObject rightHandDetector { get; set; }
    public GameObject leftHandDetector { get; set; }
    
    public WaypointManager manager { get; set; }
	public RectTransform mapWindow { get; set; }
    private Vector3[] mapCorners = new Vector3[4];

	// Start is called before the first frame update
	void Start()
    {
        mapWindow.GetLocalCorners(mapCorners);

		gameObject.layer = LayerMask.NameToLayer("UI");
		//rightHandDetector = FindFirstObjectByType<NearInteractionModeDetector>().gameObject;
		//leftHandDetector = FindObjectsByType<NearInteractionModeDetector>(FindObjectsSortMode.InstanceID)[1].gameObject;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopPlacement() {
        Vector3 pos = mapWindow.transform.InverseTransformPoint(transform.TransformPoint(transform.position));

        if (pos.x * pos.x < (mapWindow.sizeDelta.x / 2) * (mapWindow.sizeDelta.x / 2) && pos.y * pos.y < (mapWindow.sizeDelta.y / 2) * (mapWindow.sizeDelta.y / 2)) {
			transform.GetComponent<PressableButton>().enabled = true;
			transform.GetComponent<TapToPlace>().enabled = false;

			rightHandDetector.SetActive(true);
			leftHandDetector.SetActive(true);

			manager.isPlacing = false;
		} else {

			rightHandDetector.SetActive(true);
			leftHandDetector.SetActive(true);
            manager.isPlacing = false;

			Destroy(transform.gameObject);
        }
	}
}
