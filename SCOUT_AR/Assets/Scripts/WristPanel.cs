using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristPanel : MonoBehaviour
{
    public enum Handedness { Left, Right };
    public Handedness handedness;
    [Range(0.05f, 0.4f)] public float heightOffset = 0.05f;
	[Range(-.4f, 0.4f)] public float armOffset = -0.1f;


	[SerializeField]
    private Transform handVector;

    // Start is called before the first frame update
    void Start()
    {
        if (handedness == Handedness.Left) {
            handVector = GameObject.Find("MRTK LeftHand Controller").transform;
        } else {
			handVector = GameObject.Find("MRTK RightHand Controller").transform;
		}
    }

    // Update is called once per frame
    void Update() {
        // Setting rotation
        transform.rotation = handVector.rotation;

        transform.GetChild(0).LookAt(Camera.main.transform);
        float angle = -transform.GetChild(0).localEulerAngles.x;

		//transform.GetChild(0).localRotation = Quaternion.Euler(0, -67, 27f);
		transform.GetChild(0).localRotation = Quaternion.Euler(167f, 0f, 124f);
		//transform.GetChild(0).rotation *= Quaternion.AngleAxis(angle, transform.GetChild(0).right);

        // Setting position
		transform.localPosition = transform.GetChild(0).up * heightOffset + transform.GetChild(0).right * armOffset;
	}
}
