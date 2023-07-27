using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelemetryManager : MonoBehaviour {
	[Range(-180, 180)] public float longitude;
	[Range(-90, 90)] public float latitude;

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
		   latitude = pos.Coordinate.Point.Position.Latitude;
		   longitude = pos.Coordinate.Point.Position.Longitude;
		}, err => {
		   Debug.LogError(err);
		});
#endif
	}
}
