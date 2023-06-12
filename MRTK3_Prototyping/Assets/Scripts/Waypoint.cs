using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public string waypointName { get; set; }
	public float distance { get; set; }

	// Start is called before the first frame update
	void Start() {
		Vector3 lookPos = transform.position - Camera.main.transform.position;
		lookPos.y = 0;
		if (lookPos != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation(lookPos);
		}

		distance = Vector3.Distance(transform.position, Camera.main.transform.position);
		transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = waypointName;
		transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{distance.ToString("0.##")} m";
	}

    // Update is called once per frame
    void Update() {
		Vector3 lookPos = transform.position - Camera.main.transform.position;
		lookPos.y = 0;
		if (lookPos != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation(lookPos);
		}

		distance = Vector3.Distance(transform.position, Camera.main.transform.position);
		transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{distance.ToString("0.##")} m";
	}
}
