using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Breadcrumbs : MonoBehaviour
{
    [Range(0, 1)] public float trailYOffset = 0f;
    public float minVertexDistance = 2.0f;
    public float checkTime = 2.0f;
    public int maxVertices = 10000;
    public Transform trackedObject;
    public List<Vector3> breadTrailPoints { get; private set; } = new List<Vector3>();

    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
		breadTrailPoints.Add(new Vector3(0, 0, 0));

        InvokeRepeating("CheckVertex", checkTime, checkTime);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckVertex() {
        Vector3 point = trackedObject.position - trackedObject.up * (trackedObject.parent.position.y - trailYOffset);

		if (Vector3.Distance(point, breadTrailPoints[breadTrailPoints.Count - 1]) > minVertexDistance) {

            if (breadTrailPoints.Count > maxVertices) {
                breadTrailPoints.RemoveAt(0);
            }

            breadTrailPoints.Add(point);
        }

        lineRenderer.positionCount = breadTrailPoints.Count;
        lineRenderer.SetPositions(breadTrailPoints.ToArray());
    }
}