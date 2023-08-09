using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelemetryManager : MonoBehaviour {
	public Vector3 unitSpherePos {  get; private set; }
	public CoordinateDegrees longitudeLatitude;
	public float moonBaseRadius = 1719145; // meters
	public float moonMaxRadius = 1758957;

	void Start()
    {
		InvokeRepeating("UpdatePosition", 5f, 5f);
    }

    void Update()
    {
        
    }

    private void UpdatePosition() {
#if WINDOWS_UWP
		UWPGeolocation.GetLocation(pos => {
		   longitudeLatitude.latitude = pos.Coordinate.Point.Position.Latitude;
		   longitudeLatitude.longitude = pos.Coordinate.Point.Position.Longitude;
		}, err => {
		   Debug.LogError(err);
		});
#endif
		unitSpherePos = GeoMaths.CoordinateToPoint(longitudeLatitude.ConvertToRadians());
	}
}
