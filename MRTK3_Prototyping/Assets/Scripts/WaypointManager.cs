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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateAnchorDrop() {
        var instance = Instantiate(waypointPrefab, Camera.main.transform.position, Quaternion.identity, waypointCollection);

        if (instance.GetComponent<ARAnchor>() == null) {
            instance.AddComponent<ARAnchor>();
        }

        instance.GetComponent<SolverHandler>().LeftInteractor = leftRay;
        instance.GetComponent<SolverHandler>().RightInteractor = rightRay;
    }

	public void CreateAnchorPlace() {
		var instance = Instantiate(waypointPrefab, Camera.main.transform.position, Quaternion.identity, waypointCollection);

		if (instance.GetComponent<ARAnchor>() == null) {
			instance.AddComponent<ARAnchor>();
		}

		instance.GetComponent<SolverHandler>().LeftInteractor = leftRay;
		instance.GetComponent<SolverHandler>().RightInteractor = rightRay;

        instance.GetComponent<TapToPlace>().StartPlacement();
	}
}
