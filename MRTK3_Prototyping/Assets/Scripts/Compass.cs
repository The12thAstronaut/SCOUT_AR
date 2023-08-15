using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Compass : MonoBehaviour
{
	[Header("Compass Properties")]
	public int numRings = 5;
	public float thetaScale = 0.01f;
	public float radius = 3f;
	public GameObject compassRing;

	[Header("Marking Properties")]
	public GameObject compassPinPrefab;
	public GameObject cardinalDirectionPrefab;
	public MarkerManager markerManager;
	public float distancePerRing = 2f;
	public Transform lineMarkings;
	public Color distanceMarkingColor;

	private Vector3[] cardinalDirections = { Vector3.forward, Vector3.left, Vector3.right, Vector3.back };
	private string[] cardinalText = { "N", "W", "E", "S" };

	private int size;
	private LineRenderer[] lineDrawers;
	private float theta = 0f;

	private List<CompassPin> compassPins = new List<CompassPin>();
	private GameObject[] cardinalPoints = new GameObject[4];
	private List<Transform> distanceMarkers = new List<Transform>();

	void Start() {
		lineDrawers = new LineRenderer[numRings];
		//lineDrawers = GetComponentsInChildren<LineRenderer>();
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

			pin.pin.transform.localPosition = pinVec.normalized * radius * 1000f * range / (distancePerRing * numRings);
			pin.pin.transform.GetChild(1).rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

			
			TextMeshProUGUI[] parts = pin.pin.GetComponentsInChildren<TextMeshProUGUI>();
			foreach (TextMeshProUGUI part in parts) {
				if (pin.refMarker.isTargeted) {
					part.color = markerManager.targetedColor;
				} else {
					part.color = Color.white;
				}
			}
				
		}

		for (int i = 0; i < 4; i++) {
			Vector3 dirVec = cardinalDirections[i];
			dirVec.y = dirVec.z;
			dirVec.z = 0;

			cardinalPoints[i].transform.localPosition = dirVec.normalized * radius * 1000f * (numRings + 1) / numRings;
			cardinalPoints[i].transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
		}

		foreach (Transform mark in distanceMarkers) {
			mark.localRotation = Quaternion.Euler(0, 0, -Camera.main.transform.rotation.eulerAngles.y + angle);
		}
	}

	void DrawCompass() {
		for (int i = 0; i < numRings; i++) {
			lineDrawers[i] = Instantiate(compassRing, transform.position, Quaternion.identity, transform).GetComponent<LineRenderer>();
			DrawCompassRing(lineDrawers[i], radius * (i + 1) / numRings);
		}
		DrawDirections();
		DrawMarkings();
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
		transform.rotation = Quaternion.identity;
		compassPins.RemoveRange(0, compassPins.Count);
	}

	private void DrawMarkings() {
		LineRenderer[] cardinalLines = lineMarkings.GetChild(0).GetComponentsInChildren<LineRenderer>();
		for (int i = 0; i < cardinalLines.Length; i++) {
			cardinalLines[i].positionCount = 2;
			cardinalLines[i].SetPosition(0, new Vector3(radius / numRings * cardinalDirections[i].x, radius / numRings * cardinalDirections[i].z, 0) * 1000f);
			cardinalLines[i].SetPosition(1, new Vector3(radius * (numRings + 1) / numRings * cardinalDirections[i].x, radius * (numRings + 1) / numRings * cardinalDirections[i].z, 0) * 1000f);

			for (int j = 0; j < numRings; j++) {
				TextMeshProUGUI distMark = Instantiate(cardinalDirectionPrefab, transform.position, Quaternion.identity, lineMarkings).GetComponent<TextMeshProUGUI>();
				distMark.transform.localScale *= 2;
				distMark.fontSize = 6 + ((j + 1f) / numRings);
				distMark.text = ((j + 1) * distancePerRing).ToString();
				distMark.color = distanceMarkingColor;

				distanceMarkers.Add(distMark.transform);

				distMark.transform.localPosition = new Vector3(radius * (j + 1) / numRings * cardinalDirections[i].x, radius * (j + 1) / numRings * cardinalDirections[i].z, 0) * 1000f;
			}
		}
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
