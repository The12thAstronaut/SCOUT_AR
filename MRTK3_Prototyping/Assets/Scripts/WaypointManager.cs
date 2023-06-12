using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class WaypointManager : MonoBehaviour
{

    public GameObject waypointPrefab;
    public Transform waypointCollection;
    public MRTKRayInteractor leftRay;
    public MRTKRayInteractor rightRay;
    public TMP_InputField inputName;
	public PressableButton newWaypointButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateAnchorPlace() {
        if (inputName.text != "") {
			var instance = Instantiate(waypointPrefab, Camera.main.transform.position, Quaternion.identity, waypointCollection);

			if (instance.GetComponent<ARAnchor>() == null) {
				instance.AddComponent<ARAnchor>();
			}

			instance.GetComponent<SolverHandler>().LeftInteractor = leftRay;
			instance.GetComponent<SolverHandler>().RightInteractor = rightRay;

			instance.GetComponent<Waypoint>().name = inputName.text;

			newWaypointButton.ForceSetToggled(false);
		} else {
			Debug.Log("No name entered");
		}
    }

	public void CreateAnchorDrop() {
		if (inputName.text != "") {
			var instance = Instantiate(waypointPrefab, Camera.main.transform.position, Quaternion.identity, waypointCollection);

			if (instance.GetComponent<ARAnchor>() == null) {
				instance.AddComponent<ARAnchor>();
			}

			instance.GetComponent<SolverHandler>().LeftInteractor = leftRay;
			instance.GetComponent<SolverHandler>().RightInteractor = rightRay;

			// This makes it not start placement for some reason, so it's used here to stop placement.
			instance.GetComponent<TapToPlace>().StartPlacement();

			instance.GetComponent<Waypoint>().name = inputName.text;

			instance.transform.position = instance.transform.position + new Vector3(0f, -0.5f, 0f);

			newWaypointButton.ForceSetToggled(false);
		} else {
			Debug.Log("No name entered");
		}
	}
}
