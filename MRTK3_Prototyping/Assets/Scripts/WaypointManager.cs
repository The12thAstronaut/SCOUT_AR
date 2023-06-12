using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class WaypointManager : MonoBehaviour
{

    public GameObject waypointPrefab;
    public Transform waypointCollection;
    public MRTKRayInteractor leftRay;
    public MRTKRayInteractor rightRay;
    public InteractionModeManager interactionModeManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateAnchorPlace() {
        var instance = Instantiate(waypointPrefab, Camera.main.transform.position, Quaternion.identity, waypointCollection);

        if (instance.GetComponent<ARAnchor>() == null) {
            instance.AddComponent<ARAnchor>();
        }

        instance.GetComponent<SolverHandler>().LeftInteractor = leftRay;
        instance.GetComponent<SolverHandler>().RightInteractor = rightRay;
    }

	public void CreateAnchorDrop() {
		var instance = Instantiate(waypointPrefab, Camera.main.transform.position, Quaternion.identity, waypointCollection);

		if (instance.GetComponent<ARAnchor>() == null) {
			instance.AddComponent<ARAnchor>();
		}

		instance.GetComponent<SolverHandler>().LeftInteractor = leftRay;
		instance.GetComponent<SolverHandler>().RightInteractor = rightRay;

        // This makes it not start placement for some reason, so it's used here to stop placement.
        instance.GetComponent<TapToPlace>().StartPlacement();
	}
}
