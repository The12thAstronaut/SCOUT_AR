using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GridDistanceIndicator : MonoBehaviour
{
    public RectTransform mapWindow;
    public TextMeshProUGUI indicatorText;
    public MapGrid mapGrid;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(mapGrid.gridSize / mapGrid.gridDensity, transform.GetComponent<RectTransform>().sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateIndicator() {
        RaycastHit hit;
        if (!Physics.Raycast(mapWindow.position, mapWindow.forward, out hit, 100f, Physics.IgnoreRaycastLayer)) return;

		Vector3 point1 = mapWindow.GetChild(0).GetChild(0).GetChild(0).GetChild(1).transform.InverseTransformPoint(hit.point).normalized;

        float distance = Vector3.Distance(transform.GetChild(1).GetComponent<RectTransform>().position, transform.GetChild(2).GetComponent<RectTransform>().position);
        Vector3 refPos = mapWindow.position + mapWindow.right * distance;
		Physics.Raycast(refPos, mapWindow.forward, out hit, 100f, Physics.IgnoreRaycastLayer);

		Vector3 point2 = mapWindow.GetChild(0).GetChild(0).GetChild(0).GetChild(1).transform.InverseTransformPoint(hit.point).normalized;

		float angle = Mathf.Atan2(Vector3.Magnitude(Vector3.Cross(point1, point2)), Vector3.Dot(point1, point2));
		//indicatorText.text = (Vector3.Angle(point1, point2) * Mathf.Deg2Rad * 1719145f).ToString() + " m";

		indicatorText.text = (angle * 1719145f).ToString("F2") + " m";
	}
}
