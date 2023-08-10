using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MarkerInfoCard : MonoBehaviour
{
	public Marker marker { get; set; }
	public int index { get; set; }
	public MarkerManager markerManager { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

	// Update is called once per frame
	void Update()
	{
		if (marker == null) {
			return;
		}
		transform.GetChild(2).GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = $"{marker.distance.ToString("0.##")} m";
	}

	public void DeleteMarker() {
		markerManager.RemoveMarker(index);
	}
}
