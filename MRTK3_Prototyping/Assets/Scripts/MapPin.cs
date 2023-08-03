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
	public Vector3 unitSpherePos { get; set; }
	public Vector3 moonPos { get; set; }
	public Coordinate longLat { get; set; }
	public FontIconSelector markerIcon { get; set; }

	private Transform mapParent;
	private int mapLayerMask = 1 << 3;

	// Start is called before the first frame update
	void Start()
    {
        mapWindow.GetLocalCorners(mapCorners);
		gameObject.layer = LayerMask.NameToLayer("UI");
		mapParent = mapLoader.transform.GetChild(0).GetChild(0);

		if (worldMarker.mapMarker == null) {
			worldMarker.mapMarker = this;
		}

		markerIcon = transform.GetComponentInChildren<FontIconSelector>();
	}

    // Update is called once per frame
    void Update()
    {
        if (transform.GetComponent<TapToPlace>().IsBeingPlaced) {
			//Debug.Log("Being placed on map");
			moonPos = mapParent.GetChild(1).InverseTransformPoint(transform.position);
			unitSpherePos = moonPos.normalized;
			longLat = GeoMaths.PointToCoordinate(unitSpherePos);

			float angle = Mathf.Atan2(Vector3.Magnitude(Vector3.Cross(mapLoader.worldPosition, moonPos)), Vector3.Dot(mapLoader.worldPosition, moonPos));
			float dist = (angle * manager.telemetryManager.moonBaseRadius);

			float angleFromRight = Mathf.Atan2(moonPos.y - mapLoader.worldPosition.y, moonPos.x - mapLoader.worldPosition.x) * Mathf.Rad2Deg;
			//float angleDepth = Mathf.Atan2(pos.z - mapLoader.worldPosition.z, pos.y - mapLoader.worldPosition.y) * Mathf.Rad2Deg;
			//float angleDepth = Vector3.SignedAngle(mapLoader.worldPosition, pos, Vector3.forward);
			//Debug.Log(angleDepth);

			worldMarker.transform.position = Camera.main.transform.position + new Vector3(dist * Mathf.Cos(angleFromRight * Mathf.Deg2Rad), manager.markerYOffset, dist * Mathf.Sin(angleFromRight * Mathf.Deg2Rad));
		}

		if (worldMarker.transform.GetComponent<TapToPlace>().IsBeingPlaced) {
			//Debug.Log("Being placed by local marker");
			unitSpherePos = GeoMaths.CoordinateToPoint(longLat);

			//Debug.Log(unitSpherePos.x);
			//Debug.Log(longLat.longitude + " : " + longLat.latitude);
			RaycastHit hit;
			//Physics.Raycast(mapParent.GetChild(1).TransformPoint(unitSpherePos * ((manager.telemetryManager.moonMaxRadius + 1) / manager.telemetryManager.moonBaseRadius)), -mapParent.GetChild(1).TransformPoint(unitSpherePos * ((manager.telemetryManager.moonMaxRadius + 1) / manager.telemetryManager.moonBaseRadius)) + mapParent.GetChild(1).TransformPoint(Vector3.zero), out hit, manager.telemetryManager.moonMaxRadius - manager.telemetryManager.moonBaseRadius + 1f, Physics.IgnoreRaycastLayer);
			Physics.Raycast(mapParent.GetChild(1).TransformPoint(unitSpherePos * ((manager.telemetryManager.moonMaxRadius + 1) / manager.telemetryManager.moonBaseRadius)), -mapParent.GetChild(1).TransformPoint(unitSpherePos * ((manager.telemetryManager.moonMaxRadius + 1) / manager.telemetryManager.moonBaseRadius)) + mapParent.GetChild(1).TransformPoint(Vector3.zero), out hit, manager.telemetryManager.moonMaxRadius - manager.telemetryManager.moonBaseRadius + 1f, mapLayerMask);
			//Debug.DrawRay(mapParent.GetChild(1).TransformPoint(unitSpherePos * ((manager.telemetryManager.moonMaxRadius + 10) / manager.telemetryManager.moonBaseRadius)), -mapParent.GetChild(1).TransformPoint(unitSpherePos * ((manager.telemetryManager.moonMaxRadius + 10) / manager.telemetryManager.moonBaseRadius)) + mapParent.GetChild(1).TransformPoint(Vector3.zero));
			//Debug.Log(hit.transform.name + ": " + hit.transform.position);
			transform.position = hit.point;
		}
    }

    public void StopPlacement() {
        Vector3 pos = mapWindow.transform.InverseTransformPoint(transform.TransformPoint(transform.position));

        if (pos.x * pos.x < (mapWindow.sizeDelta.x / 2) * (mapWindow.sizeDelta.x / 2) && pos.y * pos.y < (mapWindow.sizeDelta.y / 2) * (mapWindow.sizeDelta.y / 2)) {

			moonPos = mapParent.GetChild(1).InverseTransformPoint(transform.position);
			unitSpherePos = moonPos.normalized;
			longLat = GeoMaths.PointToCoordinate(unitSpherePos);

			SetBeingPlaced(false);

			//SetHandDetectors(true);

			Debug.Log(longLat.longitude + ", " + longLat.latitude);
		} else {
            manager.isPlacing = false;

			//SetHandDetectors(true);

			Destroy(transform.gameObject);
        }
	}

	/*public void SetHandDetectors(bool active) {
		rightHandDetector.SetActive(active);
		leftHandDetector.SetActive(active);
	}*/

	public void SetBeingPlaced(bool isPlacing) {
		transform.GetComponent<PressableButton>().enabled = !isPlacing;
		transform.GetComponent<Collider>().enabled = !isPlacing;
		transform.GetComponent<TapToPlace>().enabled = isPlacing;
		manager.isPlacing = isPlacing;
	}

	public void SelectMarker() {
		manager.selectedMarker = worldMarker;
		manager.markerViewer.OpenViewer();
		manager.markerViewer.UpdateInfo();
	}
}
