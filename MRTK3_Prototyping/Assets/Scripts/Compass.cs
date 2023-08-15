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
	public GameObject cardinalDirectionPrefab;
	public MarkerManager markerManager;
	public float distancePerRing = 2f;
	public Transform lineMarkings;

	private Vector3[] cardinalDirections = { Vector3.forward, Vector3.left, Vector3.right, Vector3.back };
	private string[] cardinalText = { "N", "W", "E", "S" };

	private int size;
	private LineRenderer[] lineDrawers;
	private float theta = 0f;

	private List<CompassPin> compassPins = new List<CompassPin>();
	private GameObject[] cardinalPoints = new GameObject[4];

	void Start() {
		lineDrawers = new LineRenderer[numRings];
		lineDrawers = GetComponentsInChildren<LineRenderer>();
		DrawCompass();
	}

	void Update() {
		Vector3 target = transform.position - Camera.main.transform.position;
		float angle = Vector2.SignedAngle(new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z), new Vector2(target.x, target.z));

		transform.localRotation = Quaternion.Euler(0, 0, Camera.main.transform.rotation.eulerAngles.y - angle);

		foreach (CompassPin pin in compassPins) {
			Vector3 pinVec = pin.refMarker.transform.position - transform.position;
			pinVec.y = pinVec.z;
			pinVec.z = 0;

			float distance = Vector3.Distance(new Vector3(pin.refMarker.transform.position.x, 0, pin.refMarker.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z));
			float range = Mathf.Clamp(distance, 0, distancePerRing * numRings);
			pin.pin.transform.localPosition = pinVec.normalized * range * radius / distancePerRing * 1000f * radius;

			pin.pin.transform.GetChild(1).rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
		}

		for (int i = 0; i < 4; i++) {
			Vector3 dirVec = cardinalDirections[i];
			dirVec.y = dirVec.z;
			dirVec.z = 0;

			cardinalPoints[i].transform.localPosition = dirVec.normalized * (numRings + 1) * radius * 1000f * radius;

			cardinalPoints[i].transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
		}

		AlignMarkings();
	}

	void DrawCompass() {
		for (int i = 0; i < numRings; i++) {
			DrawCompassRing(lineDrawers[i], radius * (i + 1) / numRings);
		}
		DrawDirections();
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
		AlignMarkings();
	}

	public void UnloadPins() {
		foreach (CompassPin pin in compassPins) {
			Destroy(pin.pin);
		}
		transform.rotation = Quaternion.identity;
		compassPins.RemoveRange(0, compassPins.Count);
	}

	private void AlignMarkings() {
		//lineMarkings.localRotation = Quaternion.Euler(0, 0, Vector3.Angle(transform.up, Vector3.forward));
		//Debug.Log(Vector3.SignedAngle(transform.up, Vector3.forward, /*Vector3.Cross(transform.up, Vector3.forward)*/Vector3.up));
	}

	private void DrawDirections() {
		for (int i = 0; i < 4; i++) {
			GameObject dirText = Instantiate(cardinalDirectionPrefab, transform.position, Quaternion.identity, lineMarkings);
			dirText.transform.localScale *= 2;
			dirText.GetComponent<TextMeshProUGUI>().text = cardinalText[i];
			cardinalPoints[i] = dirText;
		}
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
