using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public string markerName { get; set; }
	public string markerDescription { get; set; }
	public float distance { get; set; }

	public MarkerManager manager { get; set; }
	public MapPin mapMarker { get; set; }
	public FontIconSelector markerIcon;
	public int index { get; set; }
	public string currentIconName { get; set; }
	
	public bool movedWhileMapClosed { get; set; }

	// Start is called before the first frame update
	void Start() {
		Vector3 lookPos = transform.position - Camera.main.transform.position;
		lookPos.y = 0;
		if (lookPos != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation(lookPos);
		}
		if (mapMarker.worldMarker == null) {
			mapMarker.worldMarker = this;
		}

		distance = Vector3.Distance(transform.position, Camera.main.transform.position);
		transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = markerName;
		transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{distance.ToString("0.##")} m";

		mapMarker.mapParent = mapMarker.mapLoader.transform.GetChild(0).GetChild(0);
		UpdateInfo();
	}

    // Update is called once per frame
    void Update() {

		distance = Vector3.Distance(transform.position, Camera.main.transform.position);

		if (distance < 0.5f || distance > 200) {
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(1).gameObject.SetActive(false);
			return;
		}

		if (!transform.GetChild(0).gameObject.activeSelf) {
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(1).gameObject.SetActive(true);
		}

		Vector3 lookPos = transform.position - Camera.main.transform.position;
		lookPos.y = 0;
		if (lookPos != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation(lookPos);
		}

		transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{distance.ToString("0.##")} m";

		if (transform.GetComponent<TapToPlace>().IsBeingPlaced) {
			UpdateLongLat();
		}
	}

	public void UpdateLongLat() {
		movedWhileMapClosed = false;
		Vector3 relativePos = transform.position - Camera.main.transform.position;
		float newLatitude = manager.telemetryManager.longitudeLatitude.latitude + (relativePos.z / manager.telemetryManager.moonBaseRadius) * (180f / Mathf.PI);
		float newLongitude = manager.telemetryManager.longitudeLatitude.longitude + (relativePos.x / manager.telemetryManager.moonBaseRadius) * (180f / Mathf.PI) / Mathf.Cos(manager.telemetryManager.longitudeLatitude.latitude * Mathf.PI / 180f);
		mapMarker.longLat = new Coordinate(newLongitude * Mathf.Deg2Rad, newLatitude * Mathf.Deg2Rad);

		if (mapMarker.mapParent.gameObject.activeInHierarchy) {
			mapMarker.PositionFromLocalMarker();
		} else {
			movedWhileMapClosed = true;
			manager.movedCount++;
		}

		/*
		Vector3 pos = mapLoader.transform.GetChild(0).GetChild(0).GetChild(1).transform.InverseTransformPoint(transform.position);
		float angle = Mathf.Atan2(Vector3.Magnitude(Vector3.Cross(mapLoader.worldPosition, pos)), Vector3.Dot(mapLoader.worldPosition, pos));
		float dist = (angle * 1719145f);

		float angleFromRight = Mathf.Atan2(pos.y - mapLoader.worldPosition.y, pos.x - mapLoader.worldPosition.x) * Mathf.Rad2Deg;
		//float angleDepth = Mathf.Atan2(pos.z - mapLoader.worldPosition.z, pos.y - mapLoader.worldPosition.y) * Mathf.Rad2Deg;
		//float angleDepth = Vector3.SignedAngle(mapLoader.worldPosition, pos, Vector3.forward);
		//Debug.Log(angleDepth);

		worldMarker.transform.position = Camera.main.transform.position + new Vector3(dist * Mathf.Cos(angleFromRight * Mathf.Deg2Rad), manager.markerYOffset, dist * Mathf.Sin(angleFromRight * Mathf.Deg2Rad));
		*/
	}

	public void StartPlacement() {
		//mapMarker.SetHandDetectors(false);
	}

	public void StopPlacement() {
		manager.isPlacing = false;

		if (manager.mapWindow.gameObject.activeInHierarchy) {
			manager.UpdateGroupings();
		} else {
			//manager.markerMovedWhileClosed = true;
		}

		//mapMarker.SetHandDetectors(true);
		Debug.Log(mapMarker.longLat.longitude + ", " + mapMarker.longLat.latitude);
	}

	public void UpdateInfo() {
		//if (mapMarker != null && manager.mapWindow.gameObject.activeInHierarchy) mapMarker.markerIcon.CurrentIconName = currentIconName;
		markerIcon.CurrentIconName = currentIconName;
		transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = markerName;
	}
}
