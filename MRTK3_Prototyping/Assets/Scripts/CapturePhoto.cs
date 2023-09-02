using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;

public class CapturePhoto : MonoBehaviour {

	private void Start() {
		PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
	}

	private PhotoCapture photoCaptureObject = null;

	void OnPhotoCaptureCreated(PhotoCapture captureObject) {
		photoCaptureObject = captureObject;

		Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

		CameraParameters c = new CameraParameters();
		c.hologramOpacity = 0.0f;
		c.cameraResolutionWidth = cameraResolution.width;
		c.cameraResolutionHeight = cameraResolution.height;
		c.pixelFormat = CapturePixelFormat.BGRA32;

		captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
	}

	void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result) {
		photoCaptureObject.Dispose();
		photoCaptureObject = null;
	}

	private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result) {
		if (result.success) {
			string filename = string.Format(@"CapturedImage{0}_n.jpg", Time.time);
			string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

			photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
		} else {
			Debug.LogError("Unable to start photo mode!");
		}
	}

	void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result) {
		if (result.success) {
			Debug.Log("Saved Photo to disk!");
			photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
		} else {
			Debug.Log("Failed to save Photo to disk");
		}
	}
}