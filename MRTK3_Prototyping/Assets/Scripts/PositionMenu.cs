using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PositionMenu : MonoBehaviour
{
    public float distance = 0.5f;
    public float angle = 0f;

    public bool isManipulated { get; set; } = false;
    private float pushStrength = .01f;
    private bool lockedPosFound = false;
    private bool locked = false;
    private Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (!isManipulated && !locked) {
			if (!lockedPosFound) {
				dir = transform.position - Camera.main.transform.position;
				dir.y = 0;
				lockedPosFound = true;
				transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
			}

			float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
			if (dist < distance || dist > distance + 0.1f) {
				transform.Translate(dir.normalized * pushStrength);
			} else {
				lockedPosFound = false;
				locked = true;
				transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
			}
		}
	}

	public void beingMoved() {
        isManipulated = true;
        locked = false;
    }

	public void OpenMenu() {
		Vector3 focus = Camera.main.transform.forward;
		focus.y = 0;
		transform.position = transform.parent.TransformPoint(focus.normalized * distance);
		transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
	}
}
