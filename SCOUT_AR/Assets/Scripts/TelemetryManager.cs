using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TelemetryManager : MonoBehaviour {
	public Vector3 unitSpherePos {  get; private set; }
	public CoordinateDegrees longitudeLatitude;
	public float moonBaseRadius = 1719145; // meters
	public float moonMaxRadius = 1758957;
	public ClientAPI clientAPI;
	public TextMeshProUGUI coordinatesDisplayText;

	public UnityEvent onLocationUpdate;

	void Start()
    {
		InvokeRepeating("UpdatePosition", 0f, 5f);
    }

    void Update()
    {
        
    }



    private void UpdatePosition() {
#if WINDOWS_UWP
		UWPGeolocation.GetLocation(pos => {
		   longitudeLatitude.latitude = (float)pos.Coordinate.Point.Position.Latitude;
		   longitudeLatitude.longitude = (float)pos.Coordinate.Point.Position.Longitude;

		}, err => {
		   Debug.LogError(err);
		});
#endif
		unitSpherePos = GeoMaths.CoordinateToPoint(longitudeLatitude.ConvertToRadians());


		float northMinutes = (longitudeLatitude.latitude % 1) * 60;
		float northSeconds = (northMinutes % 1) * 60;

		string coordsText;

		if (longitudeLatitude.latitude >= 0) {
			coordsText = Mathf.FloorToInt(longitudeLatitude.latitude).ToString("00") + "\u00B0" + Mathf.FloorToInt(northMinutes).ToString("00") + "'" + Mathf.FloorToInt(northSeconds).ToString("00") + "\" N\n";
		} else {
			coordsText = Mathf.FloorToInt(-longitudeLatitude.latitude).ToString("00") + "\u00B0" + Mathf.FloorToInt(-northMinutes).ToString("00") + "'" + Mathf.FloorToInt(-northSeconds).ToString("00") + "\" S\n";
		}

		float eastMinutes = (longitudeLatitude.longitude % 1) * 60;
		float eastSeconds = (eastMinutes % 1) * 60;

		if (longitudeLatitude.longitude >= 0) {
			coordsText += Mathf.FloorToInt(longitudeLatitude.longitude).ToString("00") + "\u00B0" + Mathf.FloorToInt(eastMinutes).ToString("00") + "'" + Mathf.FloorToInt(eastSeconds).ToString("00") + "\" E\n";
		} else {
			coordsText += Mathf.FloorToInt(-longitudeLatitude.longitude).ToString("00") + "\u00B0" + Mathf.FloorToInt(-eastMinutes).ToString("00") + "'" + Mathf.FloorToInt(-eastSeconds).ToString("00") + "\" W\n";
		}

		coordinatesDisplayText.text = coordsText;

		onLocationUpdate.Invoke();
	}
}
