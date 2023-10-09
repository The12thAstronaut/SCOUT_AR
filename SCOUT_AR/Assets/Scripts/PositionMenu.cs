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
	public float yOffset = -0.1f;
	public PressableButton pinButton;
	public MeshRenderer[] contentBackplates;

    public bool isManipulated { get; set; } = false;
	private bool isPinned { get; set; } = false;

	public Material contentBackplateMaterial;

    private float pushStrength = 1f;
    private bool lockedPosFound = false;
    private bool locked = false;
    private Vector3 dir;
	private Vector3 target;
	private SettingsManager settingsManager;
	private ObjectManipulator manipulator;
	private float defaultBackplateMaterialRadius;
	private float defaultBackplateMaterialWidth;

	// Start is called before the first frame update
	void Start()
    {
		contentBackplateMaterial = Instantiate<Material>(contentBackplateMaterial);

		if (settingsManager == null) settingsManager = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();
		if (manipulator == null) manipulator = transform.GetComponentInChildren<ObjectManipulator>();

		defaultBackplateMaterialRadius = contentBackplateMaterial.GetFloat("_Radius_");
		defaultBackplateMaterialWidth = contentBackplateMaterial.GetFloat("_Line_Width_");

		foreach (MeshRenderer backplate in contentBackplates) {
			backplate.sharedMaterial = contentBackplateMaterial;
			//contentBackplateMaterial = backplate.materials[0];
		}

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
				dir.y = yOffset;

				target = transform.parent.TransformPoint(dir.normalized * settingsManager.settings[0].value);

				lockedPosFound = true;
			}
			transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position - transform.parent.transform.up * yOffset);

			if (Vector3.Distance(transform.position, target) > 0.001f) {
				transform.position = Vector3.MoveTowards(transform.position, target, pushStrength * Time.deltaTime);
			} else {
				transform.position = target;
				lockedPosFound = false;
				locked = true;
				transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position - transform.parent.transform.up * yOffset);
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

	public void UpdateBackplates() {
		float sizeFactor = transform.localScale.x / 1f;
		Debug.Log(contentBackplateMaterial.GetFloat("_Radius_"));
		Debug.Log(contentBackplateMaterial.GetFloat("_Line_Width_"));
		contentBackplateMaterial.SetFloat("_Radius_", defaultBackplateMaterialRadius * sizeFactor);
		contentBackplateMaterial.SetFloat("_Line_Width_", defaultBackplateMaterialWidth * sizeFactor);

	}

	public void OpenMenu() {
		if (settingsManager == null) settingsManager = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();
		Vector3 focus = Camera.main.transform.forward;
		focus.y = yOffset;

		focus = transform.parent.TransformPoint(focus.normalized * settingsManager.settings[0].value) - Camera.main.transform.position;
		focus.y = yOffset;

		transform.position = transform.parent.TransformPoint(focus.normalized * settingsManager.settings[0].value);

		transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position - transform.parent.transform.up * yOffset);

		UpdateBackplates();
	}

	private void ApplyConstraints() {
		Vector3 constraintToPose = transform.position - Camera.main.transform.position;
		constraintToPose.y = yOffset;

		constraintToPose = constraintToPose.normalized * settingsManager.settings[0].value;
		transform.position = transform.parent.position + constraintToPose;

		// Needs smoothing?

		transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position - transform.parent.transform.up * yOffset);
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
