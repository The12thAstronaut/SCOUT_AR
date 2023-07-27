using UnityEngine;

namespace UserInfo {
	[CreateAssetMenu(menuName = "Telemetry/Position Info")]
	public class PositionInfo : ScriptableObject {
		[Range(-180, 180)] public float longitude = 0;
		[Range(-90, 90)] public float latitude = 0;
		#if WINDOWS_UWP
		UWPGeolocation.GetLocation(pos => {
		   latitude = pos.Coordinate.Point.Position.Latitude;
		   longitude = pos.Coordinate.Point.Position.Longitude;
		}, err => {
		   Debug.LogError(err);
		});
		#endif
		void Update() {

		}
	}
}