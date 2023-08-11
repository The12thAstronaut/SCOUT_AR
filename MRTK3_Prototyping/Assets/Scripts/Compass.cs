using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
	public int numRings = 5;
	public float thetaScale = 0.01f;
	public float radius = 3f;

	private int size;
	private LineRenderer[] lineDrawers;
	private float theta = 0f;

	void Start() {
		lineDrawers = new LineRenderer[numRings];
		lineDrawers = GetComponentsInChildren<LineRenderer>();
		DrawCompass();
	}

	void Update() {
		
	}

	void DrawCompass() {
		for (int i = 0; i < numRings; i++) {
			DrawCompassRing(lineDrawers[i], radius * (i + 1) / numRings);
		}
	}

	void DrawCompassRing(LineRenderer lineDrawer, float segmentRadius) {
		theta = 0f;
		size = (int)((1f / thetaScale) + 1f);
		lineDrawer.positionCount = size;
		for (int i = 0; i < size; i++) {
			theta += (2.0f * Mathf.PI * thetaScale);
			float x = segmentRadius * Mathf.Cos(theta);
			float y = segmentRadius * Mathf.Sin(theta);
			lineDrawer.SetPosition(i, new Vector3(x, 0, y));
		}
	}
}
