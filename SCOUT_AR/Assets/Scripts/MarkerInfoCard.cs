using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;

public class MarkerInfoCard : MonoBehaviour
{
	public Marker marker { get; set; }
	public int index { get; set; }
	public MarkerManager markerManager { get; set; }
	public FontIconSelector infoCardIcon;
	public TextMeshProUGUI markerName;
	public TextMeshProUGUI positionText;
	public TextMeshProUGUI distanceText;

    // Start is called before the first frame update
    void Start()
    {
		transform.localRotation = Quaternion.identity;
		if (marker != null) {
			UpdateLongLatText();
			marker.UpdateInfo();
		}
    }

	// Update is called once per frame
	void Update()
	{
		if (marker == null) {
			return;
		}
		distanceText.text = $"Distance: {marker.distance.ToString("0.##")} m";
	}

	public void UpdateLongLatText() {
		float northMinutes = (marker.mapMarker.longLat.ConvertToDegrees().latitude % 1) * 60;
		float northSeconds = (northMinutes % 1) * 60;

		string coordsText;

		if (marker.mapMarker.longLat.ConvertToDegrees().latitude >= 0) {
			coordsText = Mathf.FloorToInt(marker.mapMarker.longLat.ConvertToDegrees().latitude).ToString("00") + "\u00B0" + Mathf.FloorToInt(northMinutes).ToString("00") + "'" + Mathf.FloorToInt(northSeconds).ToString("00") + "\" N\n";
		} else {
			coordsText = Mathf.FloorToInt(-marker.mapMarker.longLat.ConvertToDegrees().latitude).ToString("00") + "\u00B0" + Mathf.FloorToInt(-northMinutes).ToString("00") + "'" + Mathf.FloorToInt(-northSeconds).ToString("00") + "\" S\n";
		}

		float eastMinutes = (marker.mapMarker.longLat.ConvertToDegrees().longitude % 1) * 60;
		float eastSeconds = (eastMinutes % 1) * 60;

		if (marker.mapMarker.longLat.ConvertToDegrees().longitude >= 0) {
			coordsText += Mathf.FloorToInt(marker.mapMarker.longLat.ConvertToDegrees().longitude).ToString("00") + "\u00B0" + Mathf.FloorToInt(eastMinutes).ToString("00") + "'" + Mathf.FloorToInt(eastSeconds).ToString("00") + "\" E\n";
		} else {
			coordsText += Mathf.FloorToInt(-marker.mapMarker.longLat.ConvertToDegrees().longitude).ToString("00") + "\u00B0" + Mathf.FloorToInt(-eastMinutes).ToString("00") + "'" + Mathf.FloorToInt(-eastSeconds).ToString("00") + "\" W\n";
		}

		positionText.text = coordsText;
	}

	public void TargetMarker(bool isTargeting) {
		markerManager.selectedMarker = marker;
		markerManager.TargetSelected(isTargeting);
	}

	public void EditMarker() {
		marker.mapMarker.SelectMarker();
	}

	public void DeleteMarker() {
		markerManager.RemoveMarker(index);
	}
}
