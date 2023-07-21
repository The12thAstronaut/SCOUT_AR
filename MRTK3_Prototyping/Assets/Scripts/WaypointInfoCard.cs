using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaypointInfoCard : MonoBehaviour
{
	public Waypoint waypoint { get; set; }
	public int index { get; set; }
	public WaypointManager waypointManager { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

	// Update is called once per frame
	void Update()
	{
		if (waypoint == null) {
			return;
		}
		transform.GetChild(2).GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = $"{waypoint.distance.ToString("0.##")} m";
	}

	public void DeleteWaypoint() {
		waypointManager.RemoveWaypoint(index);
	}
}
