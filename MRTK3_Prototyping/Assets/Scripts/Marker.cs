using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public string markerName { get; set; }
	public float distance { get; set; }

	public MarkerManager manager { get; set; }

	// Start is called before the first frame update
	void Start() {
		Vector3 lookPos = transform.position - Camera.main.transform.position;
		lookPos.y = 0;
		if (lookPos != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation(lookPos);
		}

		distance = Vector3.Distance(transform.position, Camera.main.transform.position);
		transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = markerName;
		transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{distance.ToString("0.##")} m";
	}

    // Update is called once per frame
    void Update() {

		distance = Vector3.Distance(transform.position, Camera.main.transform.position);

		if (distance < 1) {
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(1).gameObject.SetActive(false);
			return;
		}

		if (!transform.GetChild(0).gameObject.activeSelf) {
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(1).gameObject.SetActive(true);
		}

		Vector3 lookPos = transform.position - Camera.main.transform.position;
		lookPos.y = 0;
		if (lookPos != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation(lookPos);
		}

		transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{distance.ToString("0.##")} m";
	}

	public void StopPlacement() {
		manager.isPlacing = false;
	}
}
