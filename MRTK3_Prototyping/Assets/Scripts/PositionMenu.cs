using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PositionMenu : MonoBehaviour
{
    public float distance = 0.5f;
    public float angle = 0f;
	public PressableButton pinButton;

    public bool isManipulated { get; set; } = false;
	private bool isPinned { get; set; } = false;

    private float pushStrength = 1f;
    private bool lockedPosFound = false;
    private bool locked = false;
    private Vector3 dir;
	private Vector3 target;
	private SettingsManager settingsManager;
	private ObjectManipulator manipulator;

    // Start is called before the first frame update
    void Start()
    {
		if (settingsManager == null) settingsManager = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();
		if (manipulator == null) manipulator = transform.GetComponentInChildren<ObjectManipulator>();

		locked = true;
	}

    // Update is called once per frame
    void Update()
    {
		if (isPinned) return;

		if (isManipulated) {
			ApplyConstraints();
		}

		if (!isManipulated && !locked) {
			if (!lockedPosFound) {
				transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
				dir = transform.position - Camera.main.transform.position;
				dir.y = 0;

				Vector3 focus = Camera.main.transform.forward;
				focus.y = 0;
				target = transform.parent.TransformPoint(dir.normalized * settingsManager.settings[0].value);

				lockedPosFound = true;
			}
			transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

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
		if (settingsManager == null) settingsManager = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();
		Vector3 focus = Camera.main.transform.forward;
		focus.y = 0;

		transform.position = transform.parent.TransformPoint(focus.normalized * settingsManager.settings[0].value);
		transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
	}

	private void ApplyConstraints() {
		Vector3 constraintToPose = transform.position - Camera.main.transform.position;
		constraintToPose.y = 0;

		constraintToPose = constraintToPose.normalized * settingsManager.settings[0].value;
		transform.position = transform.parent.position + constraintToPose;

		// Needs smoothing?

		transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
	}

	public void PinMenu(bool pinned) {
		isPinned = pinned;
		if (isPinned) {
			manipulator.AllowedManipulations = Microsoft.MixedReality.Toolkit.TransformFlags.Move | Microsoft.MixedReality.Toolkit.TransformFlags.Rotate;
		} else {
			manipulator.AllowedManipulations = Microsoft.MixedReality.Toolkit.TransformFlags.Move;
			ApplyConstraints();
		}
	}
}
