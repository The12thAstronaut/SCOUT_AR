using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class MarkerManager : MonoBehaviour {

    public GameObject markerPrefab;
	public GameObject mapMarkerPrefab;
    public Transform markerCollection;
	public Transform mapMarkerCollection;
	public RectTransform mapWindow;
    public MRTKRayInteractor leftRay;
    public MRTKRayInteractor rightRay;
    public TMP_InputField inputName;
	public PressableButton newMarkerButton;
	public VirtualizedScrollRectList markerScrollList;
	public VirtualizedScrollRectList markerGroupScrollList;
	public GameObject markerInfoCard;
	public GameObject rightHandDetector;
	public GameObject leftHandDetector;
	public TelemetryManager telemetryManager;
	public MapLoader mapLoader;
	public MarkerViewer markerViewer;
	public FontIconSelector markerIconSelector;
	public MarkerGroupViewer markerGroupViewer;
	public string[] markerIconNames = { "Icon 103" };
	public string groupMarkerIconName = "Icon 51";

	public float markerYOffset = -0.5f;
	public float mapGroupingRange = 0.02f;

	private int totalMarkers = 0;
	public List<Marker> markers = new List<Marker>();
	public List<MapPin> mapMarkers = new List<MapPin>();
	public List<MarkerGroup> mapMarkerGroups = new List<MarkerGroup>();
	public Marker selectedMarker { get; set; }
	public MarkerGroup selectedGroup { get; set; }

	private float startTime;
	private bool started = false;
	private bool ready = false;
	private int currentIconIndex = 0;
	private int mapLayerMask = 1 << 3;

	public bool isPlacing { get; set; } = false;
	public int movedCount { get; set; } = 0;

	// Start is called before the first frame update
	void Start()
    {
		markerScrollList.OnVisible = PopulateInfoCard;
		markerScrollList.OnInvisible = DepopulateInfoCard;

		markerGroupScrollList.OnVisible = PopulateGroupViewer;
		markerGroupScrollList.OnInvisible = DepopulateGroupViewer;
	}

    // Update is called once per frame
    void Update()
    {
		if (ready && !started) {
			startTime = Time.time;
			started = true;
			ready = false;
		}

		if (started && Time.time - startTime >= .01) {
			markerScrollList.SetItemCount(totalMarkers);
			started = false;
		}

		/*if (markerMovedWhileClosed && mapWindow.gameObject.activeInHierarchy && mapLoader.generated) {
			markerMovedWhileClosed = false;
			UpdateGroupings();
		}*/
	}

	public void CreateAnchorPlace() {
        if (!isPlacing && inputName.text != "") {
			isPlacing = true;

			GameObject marker = CreateLocalMarker();

			MapPin mapMarker = MakeMapMarker().GetComponent<MapPin>();
			mapMarker.GetComponent<TapToPlace>().StartPlacement();
			mapMarker.GetComponent<TapToPlace>().StartPlacement();

			marker.GetComponent<Marker>().mapMarker = mapMarker;
			mapMarker.worldMarker = marker.GetComponent<Marker>();
			mapMarker.SetBeingPlaced(false);

			mapMarkers.Add(mapMarker);

		} else {
			Debug.Log("No name entered");
		}
    }

	public void CreateAnchorDrop() {
		if (!isPlacing && inputName.text != "") {
			//isPlacing = true;

			GameObject marker = CreateLocalMarker();
			marker.GetComponent<TapToPlace>().StartPlacement();
			marker.GetComponent<Marker>().movedWhileMapClosed = true;
			movedCount++;
			//markerMovedWhileClosed = true;

			MapPin mapMarker = MakeMapMarker().GetComponent<MapPin>();
			mapMarker.GetComponent<TapToPlace>().StartPlacement();
			mapMarker.GetComponent<TapToPlace>().StartPlacement();

			marker.transform.GetComponent<Marker>().mapMarker = mapMarker;
			mapMarker.worldMarker = marker.GetComponent<Marker>();
			mapMarker.SetBeingPlaced(false);

			mapMarkers.Add(mapMarker);
		} else {
			Debug.Log("No name entered");
		}
	}

	private GameObject CreateLocalMarker() {
		GameObject instance = Instantiate(markerPrefab, Camera.main.transform.position, Quaternion.identity, markerCollection);

		if (instance.GetComponent<ARAnchor>() == null) {
			instance.AddComponent<ARAnchor>();
		}

		instance.GetComponent<SolverHandler>().LeftInteractor = leftRay;
		instance.GetComponent<SolverHandler>().RightInteractor = rightRay;

		instance.GetComponent<Marker>().markerName = inputName.text;
		instance.GetComponent<Marker>().markerDescription = "";
		instance.GetComponent<Marker>().manager = this;
		instance.GetComponent<Marker>().index = totalMarkers;
		instance.GetComponent<Marker>().currentIconName = markerIconNames[currentIconIndex];

		instance.transform.position = instance.transform.position + new Vector3(0f, markerYOffset, 0f);

		newMarkerButton.ForceSetToggled(false);

		markers.Add(instance.GetComponent<Marker>());

		totalMarkers++;
		markerScrollList.SetItemCount(totalMarkers);
		currentIconIndex = 0;
		markerIconSelector.CurrentIconName = markerIconNames[currentIconIndex];
		inputName.text = "";
		return instance;
	}

	public void CreateMapMarker() {
		if (isPlacing) return;
		isPlacing = true;

		GameObject instance = MakeMapMarker();

		instance.GetComponent<MapPin>().worldMarker = CreateLocalMarker().GetComponent<Marker>();
		instance.GetComponent<MapPin>().worldMarker.GetComponent<TapToPlace>().StartPlacement();

		mapMarkers.Add(instance.GetComponent<MapPin>());
	}

	private GameObject MakeMapMarker() {
		GameObject instance = Instantiate(mapMarkerPrefab, Camera.main.transform.position, Quaternion.identity, mapMarkerCollection);
		mapLoader.UpdateMarkerSize();

		instance.GetComponent<SolverHandler>().LeftInteractor = leftRay;
		instance.GetComponent<SolverHandler>().RightInteractor = rightRay;

		instance.GetComponent<MapPin>().rightHandDetector = rightHandDetector;
		instance.GetComponent<MapPin>().leftHandDetector = leftHandDetector;
		instance.GetComponent<MapPin>().manager = this;
		instance.GetComponent<MapPin>().mapWindow = mapWindow;
		instance.GetComponent<MapPin>().mapLoader = mapLoader;

		return instance;
	}

	public void PopulateInfoCard(GameObject infoCard, int index) {
		if (index < totalMarkers) {
			infoCard.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = markers[index].markerName;
			infoCard.transform.GetChild(2).GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = $"{markers[index].distance.ToString("0.##")} m";
			infoCard.transform.GetComponent<MarkerInfoCard>().marker = markers[index];
			infoCard.transform.GetComponent<MarkerInfoCard>().index = index;
			infoCard.transform.GetComponent<MarkerInfoCard>().markerManager = this;
		}
	}

	public void DepopulateInfoCard(GameObject infoCard, int index) {
		infoCard.transform.Translate(0, 30000, 0);
	}


	public void PopulateGroupViewer(GameObject selectorButton, int index) {
		if (index < selectedGroup.mapMarkers.Count) {
			selectorButton.GetComponentInChildren<TextMeshProUGUI>().text = selectedGroup.mapMarkers[index].worldMarker.markerName;
			selectorButton.GetComponentInChildren<FontIconSelector>().CurrentIconName = selectedGroup.mapMarkers[index].worldMarker.currentIconName;
			selectorButton.GetComponent<MarkerSelectorButton>().markerManager = this;
			selectorButton.GetComponent<MarkerSelectorButton>().markerIndex = index;
		}
	}

	public void DepopulateGroupViewer(GameObject selectorButton, int index) {
		selectorButton.transform.Translate(0, 30000, 0);
	}

	public void RemoveMarker(int index) {
		Destroy(markers[index].gameObject);
		Destroy(mapMarkers[index].gameObject);
		markers.RemoveAt(index);
		mapMarkers.RemoveAt(index);

		foreach (Marker marker in markers) {
			if (marker.index > index) marker.index--;
		}

		totalMarkers--;
		markerScrollList.SetItemCount(0);
		ready = true;

		if (mapWindow.gameObject.activeInHierarchy) {
			UpdateGroupings();
		} else {
			//markerMovedWhileClosed = true;
		}
	}

	public void NextIcon() {
		currentIconIndex = Mathf.Clamp(++currentIconIndex, 0, markerIconNames.Length - 1);
		markerIconSelector.CurrentIconName = markerIconNames[currentIconIndex];
	}

	public void PreviousIcon() {
		currentIconIndex = Mathf.Clamp(--currentIconIndex, 0, markerIconNames.Length - 1);
		markerIconSelector.CurrentIconName = markerIconNames[currentIconIndex];
	}

	public void UpdateGroupings() {

		foreach (MarkerGroup group in mapMarkerGroups) {
			Destroy(group.groupMapMarker.gameObject);
		}

		mapMarkerGroups.RemoveRange(0, mapMarkerGroups.Count);

		foreach (MapPin pin in mapMarkers) {
			pin.gameObject.SetActive(true);
		}

		int currentGroup = 0;
		bool[] usedIndex = new bool[mapMarkers.Count]; // True/false for if index is already in a group, defaulted to false

		// Generate map marker groups
		for (int i = 0; i < mapMarkers.Count; i++) {
			if (usedIndex[i]) continue; //If index is in a group, skip to next index
			if (mapMarkers.Count - i < 2) break;

			Vector3 centerTotalPoint = mapMarkers[i].transform.position;
			int count = 1;
			for ( int j = i + 1; j < mapMarkers.Count; j++) {
				if (usedIndex[j]) continue;

				if (Vector3.Distance(centerTotalPoint / count, mapMarkers[j].transform.position) < mapGroupingRange) {
					centerTotalPoint += mapMarkers[j].transform.position;
					count++;

					if (count == 2) {

						MapPin groupMapMarker = MakeMapMarker().GetComponent<MapPin>();
						groupMapMarker.GetComponent<TapToPlace>().StartPlacement();
						groupMapMarker.GetComponent<TapToPlace>().StartPlacement();
						groupMapMarker.SetBeingPlaced(false);
						groupMapMarker.isGroupMarker = true;
						groupMapMarker.groupIndex = currentGroup;

						mapMarkerGroups.Add(new MarkerGroup(currentGroup, groupMapMarker));
						mapMarkerGroups[currentGroup].mapMarkers.Add(mapMarkers[i]);
						usedIndex[i] = true;
					}
					mapMarkerGroups[currentGroup].mapMarkers.Add(mapMarkers[j]);
					usedIndex[j] = true;
				}
			}
			currentGroup++;
		}

		// Create map group markers, hide map markers in group
		foreach (MarkerGroup group in mapMarkerGroups) {
			Vector3 avgPoint = Vector3.zero;
			foreach (MapPin pin in group.mapMarkers) {
				avgPoint += pin.transform.localPosition;
				pin.gameObject.SetActive(false);
			}
			avgPoint /= group.mapMarkers.Count;

			Vector3 unitSpherePos = avgPoint.normalized;
			Transform mapParent = mapLoader.transform.GetChild(0).GetChild(0);
			RaycastHit hit;
			Physics.Raycast(mapParent.GetChild(1).TransformPoint(unitSpherePos * ((telemetryManager.moonMaxRadius + 1) / telemetryManager.moonBaseRadius)), -mapParent.GetChild(1).TransformPoint(unitSpherePos * ((telemetryManager.moonMaxRadius + 1) / telemetryManager.moonBaseRadius)) + mapParent.GetChild(1).TransformPoint(Vector3.zero), out hit, telemetryManager.moonMaxRadius - telemetryManager.moonBaseRadius + 1f, mapLayerMask);
			group.groupMapMarker.transform.position = hit.point;

		}
	}
}

public struct MarkerGroup {
	public List<MapPin> mapMarkers;// = new List<MapPin>();
	public int groupIndex;
	public MapPin groupMapMarker;

	public MarkerGroup(int groupIndex, MapPin groupMapMarker) {
		this.groupIndex = groupIndex;
		mapMarkers = new List<MapPin>();
		this.groupMapMarker = groupMapMarker;
	}
}