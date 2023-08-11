using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

	public Transform mapParent;
	public bool isGroupMarker { get; set; }
	public int groupIndex { get; set; }

	private int mapLayerMask = 1 << 3;
	private bool waitNextFrame = true;
	private float passedTime;

	// Start is called before the first frame update
	void Start()
    {
        mapWindow.GetLocalCorners(mapCorners);
		gameObject.layer = LayerMask.NameToLayer("UI");
		if (mapParent == null) {
			mapParent = mapLoader.transform.GetChild(0).GetChild(0);
		}

		if (!isGroupMarker && worldMarker.mapMarker == null) {
			worldMarker.mapMarker = this;
		}

		markerIcon = transform.GetComponentInChildren<FontIconSelector>();

		if (!isGroupMarker) {
			worldMarker.UpdateInfo();
			UpdateMapIcon();
		} else {
			markerIcon.CurrentIconName = manager.groupMarkerIconName;
			transform.rotation = mapWindow.rotation;
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (isGroupMarker) return;
		if (!(mapLoader.generated && mapLoader.loaded)) return;
		if (waitNextFrame) {
			waitNextFrame = false;
			return;
		}

        if (transform.GetComponent<TapToPlace>().IsBeingPlaced) {
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

		if (worldMarker.movedWhileMapClosed && (passedTime += Time.deltaTime) > .01f) {
			manager.movedCount--;
			worldMarker.UpdateLongLat();
			if (manager.movedCount == 0) {
				manager.UpdateGroupings();
			}
		}

		if (worldMarker.transform.GetComponent<TapToPlace>().IsBeingPlaced) {
			PositionFromLocalMarker();
		}
    }

	public void PositionFromLocalMarker() {
		unitSpherePos = GeoMaths.CoordinateToPoint(longLat);
		//Debug.Log(unitSpherePos.x);
		//Debug.Log(longLat.longitude + " : " + longLat.latitude);
		RaycastHit hit;
		//Physics.Raycast(mapParent.GetChild(1).TransformPoint(unitSpherePos * ((manager.telemetryManager.moonMaxRadius + 1) / manager.telemetryManager.moonBaseRadius)), -mapParent.GetChild(1).TransformPoint(unitSpherePos * ((manager.telemetryManager.moonMaxRadius + 1) / manager.telemetryManager.moonBaseRadius)) + mapParent.GetChild(1).TransformPoint(Vector3.zero), out hit, manager.telemetryManager.moonMaxRadius - manager.telemetryManager.moonBaseRadius + 1f, Physics.IgnoreRaycastLayer);
		Physics.Raycast(mapParent.GetChild(1).TransformPoint(unitSpherePos * ((manager.telemetryManager.moonMaxRadius + 100) / manager.telemetryManager.moonBaseRadius)), -mapParent.GetChild(1).TransformPoint(unitSpherePos * ((manager.telemetryManager.moonMaxRadius + 100) / manager.telemetryManager.moonBaseRadius)) + mapParent.GetChild(1).TransformPoint(Vector3.zero), out hit, manager.telemetryManager.moonMaxRadius - manager.telemetryManager.moonBaseRadius + 100f, mapLayerMask);
		//Debug.DrawRay(mapParent.GetChild(1).TransformPoint(unitSpherePos * ((manager.telemetryManager.moonMaxRadius + 10) / manager.telemetryManager.moonBaseRadius)), -mapParent.GetChild(1).TransformPoint(unitSpherePos * ((manager.telemetryManager.moonMaxRadius + 10) / manager.telemetryManager.moonBaseRadius)) + mapParent.GetChild(1).TransformPoint(Vector3.zero));
		//Debug.Log(hit.transform.name + ": " + hit.transform.position);
		transform.position = hit.point;
		transform.rotation = mapWindow.rotation;
	}

    public void StopPlacement() {
        Vector3 pos = mapWindow.transform.InverseTransformPoint(transform.TransformPoint(transform.position));

        if (pos.x * pos.x < (mapWindow.sizeDelta.x / 2) * (mapWindow.sizeDelta.x / 2) && pos.y * pos.y < (mapWindow.sizeDelta.y / 2) * (mapWindow.sizeDelta.y / 2)) {

			moonPos = mapParent.GetChild(1).InverseTransformPoint(transform.position);
			unitSpherePos = moonPos.normalized;
			longLat = GeoMaths.PointToCoordinate(unitSpherePos);

			SetBeingPlaced(false);
			manager.UpdateGroupings();

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

	public /*async*/ void SetBeingPlaced(bool isPlacing) {
		transform.GetComponent<Collider>().enabled = !isPlacing;
		transform.GetComponent<TapToPlace>().enabled = isPlacing;
		//await Task.Delay(1000);
		transform.GetComponent<PressableButton>().enabled = !isPlacing;
		manager.isPlacing = isPlacing;
	}

	public void SelectMarker() {
		if (isGroupMarker) {
			manager.selectedGroup = manager.mapMarkerGroups[groupIndex];
			manager.markerGroupScrollList.SetItemCount(manager.selectedGroup.mapMarkers.Count);
			manager.markerGroupViewer.OpenViewer();
		} else {
			manager.selectedMarker = worldMarker;
			manager.markerViewer.OpenViewer();
		}
	}

	public void UpdateMapIcon() {
		markerIcon.CurrentIconName = worldMarker.currentIconName;
	}
}
