using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Compass : MonoBehaviour
{
	[Header("Compass Properties")]
	public int numRings = 5;
	public float thetaScale = 0.01f;
	public float radius = 3f;

	[Header("Marking Properties")]
	public GameObject compassPinPrefab;
	public MarkerManager markerManager;
	public float distancePerRing = 2f;

	private int size;
	private LineRenderer[] lineDrawers;
	private float theta = 0f;

	private List<CompassPin> compassPins = new List<CompassPin>();

	void Start() {
		lineDrawers = new LineRenderer[numRings];
		lineDrawers = GetComponentsInChildren<LineRenderer>();
		DrawCompass();
	}

	void Update() {
		foreach (CompassPin pin in compassPins) {
			Vector3 pinVec = pin.refMarker.transform.position - transform.position;
			pinVec.y = pinVec.z;
			pinVec.z = 0;

			float distance = Vector3.Distance(new Vector3(pin.refMarker.transform.position.x, 0, pin.refMarker.transform.position.z), new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z));
			float range = Mathf.Clamp(distance, 0, distancePerRing * numRings);
			Debug.Log(range);
			pin.pin.transform.localPosition = pinVec.normalized * range * radius / distancePerRing * 1000f * radius;
		}
	}

	void DrawCompass() {
		for (int i = 0; i < numRings; i++) {
			DrawCompassRing(lineDrawers[i], radius * (i + 1) / numRings);
		}
	}

	void DrawCompassRing(LineRenderer lineDrawer, float segmentRadius) {
		theta = 0f;
		size = (int)((1f / thetaScale) + 1f);
		lineDrawer.positionCount = size;
		for (int i = 0; i < size; i++) {
			theta += (2.0f * Mathf.PI * thetaScale);
			float x = segmentRadius * Mathf.Cos(theta);
			float y = segmentRadius * Mathf.Sin(theta);
			lineDrawer.SetPosition(i, new Vector3(x, y, 0) * 1000f);
		}
	}

	public void LoadPins() {
		foreach (Marker marker in markerManager.markers) {
			if (marker.gameObject.activeSelf) {
				GameObject pin = Instantiate(compassPinPrefab, transform.position, Quaternion.identity, transform);
				pin.transform.Rotate(-90, 0, 0);
				pin.transform.localScale *= 2;
				compassPins.Add(new CompassPin(marker, pin));
			}
		}
	}

	public void UnloadPins() {
		foreach (CompassPin pin in compassPins) {
			Destroy(pin.pin);
		}
		compassPins.RemoveRange(0, compassPins.Count);
	}
}

struct CompassPin {
	public int index;
	public Marker refMarker;
	public GameObject pin;

	public CompassPin(Marker marker, GameObject pin) {
		index = marker.index;
		refMarker = marker;
		this.pin = pin;
		UpdateInfo();
	}

	public void UpdateInfo() {
		pin.transform.GetChild(1).GetChild(0).GetComponent<FontIconSelector>().CurrentIconName = refMarker.currentIconName;
		pin.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = refMarker.markerName;
	}
}
