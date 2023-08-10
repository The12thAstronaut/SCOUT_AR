using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PositionMenu : MonoBehaviour
{
    public float distance = 0.5f;
    public float angle = 0f;

    public bool isManipulated { get; set; } = false;
    private float pushStrength = 1f;
    private bool lockedPosFound = false;
    private bool locked = false;
    private Vector3 dir;
	private Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
		Vector3 focus = Camera.main.transform.forward;
		focus.y = 0;
		Debug.Log(transform.position);
		transform.position = transform.parent.TransformPoint(focus.normalized * distance);
		Debug.Log(transform.position);
		//transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

		locked = true;
	}

    // Update is called once per frame
    void Update()
    {
		if (!isManipulated && !locked) {
			if (!lockedPosFound) {
				transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
				dir = transform.position - Camera.main.transform.position;
				dir.y = 0;

				Vector3 focus = Camera.main.transform.forward;
				focus.y = 0;
				target = transform.parent.TransformPoint(dir.normalized * distance);

				lockedPosFound = true;
			}
			transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

			//float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
			if (Vector3.Distance(transform.position, target) > 0.001f) {
				transform.position = Vector3.MoveTowards(transform.position, target, pushStrength * Time.deltaTime);
			} else {
				transform.position = target;
				lockedPosFound = false;
				locked = true;
				transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
			}
		}
	}

	public void BeingMoved() {
        isManipulated = true;
        locked = false;
    }

	public void ChangeDistance() {
		locked = false;
	}

	public void OpenMenu() {
		//locked = true;
		//isManipulated = false;

		Vector3 focus = Camera.main.transform.forward;
		focus.y = 0;
		transform.position = transform.parent.TransformPoint(focus.normalized * distance);
		transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);//Quaternion.LookRotation(transform.parent.TransformPoint(focus.normalized * distance) - Camera.main.transform.position);
	}
}
