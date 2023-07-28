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
    
    public MarkerManager manager { get; set; }
	public RectTransform mapWindow { get; set; }
	public Marker worldMarker { get; set; }
	public MapLoader mapLoader { get; set; }

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
        if (transform.GetComponent<TapToPlace>().IsBeingPlaced) {

			Vector3 pos = mapLoader.transform.GetChild(0).GetChild(0).GetChild(1).transform.InverseTransformPoint(transform.position);
			float angle = Mathf.Atan2(Vector3.Magnitude(Vector3.Cross(mapLoader.worldPosition, pos)), Vector3.Dot(mapLoader.worldPosition, pos));
			float dist = (angle * 1719145f);

			float angleFromRight = Mathf.Atan2(pos.y - mapLoader.worldPosition.y, pos.x - mapLoader.worldPosition.x) * Mathf.Rad2Deg;
			//float angleDepth = Mathf.Atan2(pos.z - mapLoader.worldPosition.z, pos.y - mapLoader.worldPosition.y) * Mathf.Rad2Deg;
			//float angleDepth = Vector3.SignedAngle(mapLoader.worldPosition, pos, Vector3.forward);
			//Debug.Log(angleDepth);

			worldMarker.transform.position = Camera.main.transform.position + new Vector3(dist * Mathf.Cos(angleFromRight * Mathf.Deg2Rad), manager.markerYOffset, dist * Mathf.Sin(angleFromRight * Mathf.Deg2Rad));
		}
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
