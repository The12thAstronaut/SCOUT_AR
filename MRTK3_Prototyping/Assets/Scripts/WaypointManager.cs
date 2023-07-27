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

public class WaypointManager : MonoBehaviour
{

    public GameObject waypointPrefab;
	public GameObject mapMarkerPrefab;
    public Transform waypointCollection;
	public Transform mapMarkerCollection;
	public RectTransform mapWindow;
    public MRTKRayInteractor leftRay;
    public MRTKRayInteractor rightRay;
    public TMP_InputField inputName;
	public PressableButton newWaypointButton;
	public VirtualizedScrollRectList waypointScrollList;
	public GameObject waypointInfoCard;
	public GameObject rightHandDetector;
	public GameObject leftHandDetector;

	private int totalWaypoints = 0;
	public List<Waypoint> waypoints;

	private float startTime;
	private bool started = false;
	private bool ready = false;
	public bool isPlacing { get; set; } = false;

	// Start is called before the first frame update
	void Start()
    {
		waypointScrollList.OnVisible = PopulateInfoCard;
		waypointScrollList.OnInvisible = Depopulate;
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
			waypointScrollList.SetItemCount(totalWaypoints);
			started = false;
		}
	}

	public void CreateAnchorPlace() {
        if (!isPlacing && inputName.text != "") {
			isPlacing = true;
			var instance = Instantiate(waypointPrefab, Camera.main.transform.position, Quaternion.identity, waypointCollection);

			if (instance.GetComponent<ARAnchor>() == null) {
				instance.AddComponent<ARAnchor>();
			}

			instance.GetComponent<SolverHandler>().LeftInteractor = leftRay;
			instance.GetComponent<SolverHandler>().RightInteractor = rightRay;

			instance.GetComponent<Waypoint>().name = inputName.text;
			instance.GetComponent<Waypoint>().manager = this;

			newWaypointButton.ForceSetToggled(false);

			waypoints.Add(instance.GetComponent<Waypoint>());
			totalWaypoints++;
			waypointScrollList.SetItemCount(totalWaypoints);
		} else {
			Debug.Log("No name entered");
		}
    }

	public void CreateAnchorDrop() {
		if (!isPlacing && inputName.text != "") {
			isPlacing = true;
			var instance = Instantiate(waypointPrefab, Camera.main.transform.position, Quaternion.identity, waypointCollection);

			if (instance.GetComponent<ARAnchor>() == null) {
				instance.AddComponent<ARAnchor>();
			}

			instance.GetComponent<SolverHandler>().LeftInteractor = leftRay;
			instance.GetComponent<SolverHandler>().RightInteractor = rightRay;

			// Calling this means placement is started twice due to auto-start, which means it is stopped. See: TapToPlace.StartPlacement
			instance.GetComponent<TapToPlace>().StartPlacement();

			instance.GetComponent<Waypoint>().waypointName = inputName.text;
			instance.GetComponent<Waypoint>().manager = this;

			instance.transform.position = instance.transform.position + new Vector3(0f, -0.5f, 0f);

			newWaypointButton.ForceSetToggled(false);

			waypoints.Add(instance.GetComponent<Waypoint>());
			totalWaypoints++;
			waypointScrollList.SetItemCount(totalWaypoints);
		} else {
			Debug.Log("No name entered");
		}
	}

	public void CreateMapMarker() {
		if (isPlacing) return;
		isPlacing = true;

		var instance = Instantiate(mapMarkerPrefab, Camera.main.transform.position, Quaternion.identity, mapMarkerCollection);

		instance.GetComponent<SolverHandler>().LeftInteractor = leftRay;
		instance.GetComponent<SolverHandler>().RightInteractor = rightRay;

		instance.GetComponent<MapPin>().rightHandDetector = rightHandDetector;
		instance.GetComponent<MapPin>().leftHandDetector = leftHandDetector;
		instance.GetComponent<MapPin>().manager = this;
		instance.GetComponent<MapPin>().mapWindow = mapWindow;
	}

	public void PopulateInfoCard(GameObject infoCard, int index) {
		if (index < totalWaypoints) {
			infoCard.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = waypoints[index].waypointName;
			infoCard.transform.GetChild(2).GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = $"{waypoints[index].distance.ToString("0.##")} m";
			infoCard.transform.GetComponent<WaypointInfoCard>().waypoint = waypoints[index];
			infoCard.transform.GetComponent<WaypointInfoCard>().index = index;
			infoCard.transform.GetComponent<WaypointInfoCard>().waypointManager = this;
		}
	}

	public void Depopulate(GameObject infoCard, int index) {
		infoCard.transform.Translate(0, 30000, 0);
	}

	public void RemoveWaypoint(int index) {
		Destroy(waypoints[index].gameObject);
		waypoints.RemoveAt(index);
		
		totalWaypoints--;
		waypointScrollList.SetItemCount(0);
		ready = true;
	}
}
