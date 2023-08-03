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
	public GameObject markerInfoCard;
	public GameObject rightHandDetector;
	public GameObject leftHandDetector;
	public TelemetryManager telemetryManager;
	public MapLoader mapLoader;
	public MarkerViewer markerViewer;

	public float markerYOffset = -0.5f;

	private int totalMarkers = 0;
	public List<Marker> markers = new List<Marker>();
	public List<MapPin> mapMarkers = new List<MapPin>();
	public Marker selectedMarker { get; set; }

	private float startTime;
	private bool started = false;
	private bool ready = false;

	public bool isPlacing { get; set; } = false;

	// Start is called before the first frame update
	void Start()
    {
		markerScrollList.OnVisible = PopulateInfoCard;
		markerScrollList.OnInvisible = Depopulate;
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
	}

	public void CreateAnchorPlace() {
        if (!isPlacing && inputName.text != "") {
			isPlacing = true;

			GameObject marker = CreateLocalMarker();

			MapPin mapMarker = MakeMapMarker().GetComponent<MapPin>();
			mapMarker.GetComponent<TapToPlace>().StartPlacement();
			mapMarker.GetComponent<TapToPlace>().StartPlacement();

			marker.transform.GetComponent<Marker>().mapMarker = mapMarker;
			mapMarker.worldMarker = marker.GetComponent<Marker>();
			mapMarker.SetBeingPlaced(false);
			//mapMarker.SetHandDetectors(false);

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

			MapPin mapMarker = MakeMapMarker().GetComponent<MapPin>();
			mapMarker.GetComponent<TapToPlace>().StartPlacement();
			mapMarker.GetComponent<TapToPlace>().StartPlacement();

			marker.transform.GetComponent<Marker>().mapMarker = mapMarker;
			mapMarker.worldMarker = marker.GetComponent<Marker>();
			mapMarker.SetBeingPlaced(false);
			//mapMarker.SetHandDetectors(false);

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

		instance.transform.position = instance.transform.position + new Vector3(0f, markerYOffset, 0f);

		newMarkerButton.ForceSetToggled(false);

		markers.Add(instance.GetComponent<Marker>());

		totalMarkers++;
		markerScrollList.SetItemCount(totalMarkers);
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
			infoCard.transform.GetComponent<MarkerInfoCard>().waypointManager = this;
		}
	}

	public void Depopulate(GameObject infoCard, int index) {
		infoCard.transform.Translate(0, 30000, 0);
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
	}
}
