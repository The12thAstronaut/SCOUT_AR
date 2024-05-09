using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentWalkability : MonoBehaviour
{
    public Vector3 upVector { get; set; } = Vector3.up;
    public float maxSlopeAngle = 30f;
    public float maxCastDistance = 10f;
    public float updateTime = 5f;
    public LayerMask layerMask;
    public float retentionDistance = 10f;
    public GameObject indicatorPrefab;
    public float addDistance = 0.2f;

    public float coneAngle = 90;

    [Range(2f, 30f)]
    public float density = 5;

    private List<GameObject> indicators = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("DetermineWalkability", 1f, updateTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DetermineWalkability() {
        // Remove indicators outside the retention distance
        for (int i = 0; i < indicators.Count; i++) {
            Vector3 cameraPos = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);
            if (Vector3.Magnitude(indicators[i].transform.position - cameraPos) > retentionDistance) {
                Destroy(indicators[i]);
                indicators.RemoveAt(i);
			}
        }

        /*GameObject[] terrain = GameObject.FindGameObjectsWithTag("AR Terrain");
		foreach (GameObject obj in terrain) {
			Vector3[] meshNormals = obj.transform.GetComponent<MeshFilter>().mesh.normals;

            for (int i = 0; i < meshNormals.Length; i++) {
                float angle = Vector3.Angle(meshNormals[i], upVector);


            }
		}*/
        Vector3[] directions = GetDirections();
        foreach (Vector3 direction in directions) {

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, direction, out hit, maxCastDistance, layerMask)) {

                float minDistance = retentionDistance;

                foreach (GameObject obj in indicators) {
                    float distance = Vector3.Magnitude(hit.point - obj.transform.position);
                    if (distance < minDistance) {
                        minDistance = distance;
                    }
                }

                if (minDistance < addDistance) {
                    continue;
                }

				float angle = Vector3.Angle(hit.normal, upVector);

                indicators.Add(GameObject.Instantiate(indicatorPrefab, hit.point, Quaternion.Euler(hit.normal), transform));
				indicators[indicators.Count - 1].transform.localScale = Vector3.one / 20;
				MeshRenderer meshRenderer = indicators[indicators.Count - 1].GetComponent<MeshRenderer>();
				meshRenderer.material.color = Color.Lerp(Color.green, Color.red, angle / maxSlopeAngle);
			}
        }
	}

    private Vector3[] GetDirections() {
        List<Vector3> vectors = new List<Vector3>();

        float angleIncrement = coneAngle / density * Mathf.Deg2Rad;

        vectors.Add(-upVector);

        for(int i = 1; i <= density; i++) {
            float thetaIncrement = 2 * Mathf.PI / ((i + 1) * (i + 1));

            for(int j = 0; j < ((i + 1) * (i + 1)); j++) {
                vectors.Add(new Vector3(Mathf.Cos(thetaIncrement * j) * Mathf.Sin(angleIncrement * i), -Mathf.Cos(angleIncrement * i), Mathf.Sin(thetaIncrement * j) * Mathf.Sin(angleIncrement * i)));
            }
        }

        return vectors.ToArray();
    }
}
