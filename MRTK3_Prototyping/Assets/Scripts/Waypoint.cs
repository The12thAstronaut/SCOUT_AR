using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public string name { get; set; }

    // Start is called before the first frame update
    void Start() {
		Vector3 lookPos = transform.position - Camera.main.transform.position;
		lookPos.y = 0;
		transform.rotation = Quaternion.LookRotation(lookPos);

		transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
		transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{Vector3.Distance(transform.position, Camera.main.transform.position).ToString("0.##")} m";
	}

    // Update is called once per frame
    void Update() {
		Vector3 lookPos = transform.position - Camera.main.transform.position;
		lookPos.y = 0;
		transform.rotation = Quaternion.LookRotation(lookPos);

		transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{Vector3.Distance(transform.position, Camera.main.transform.position).ToString("0.##")} m";
	}
}
