using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    public int gridDensity = 5;
    public float gridSize = 190f;
    public float lineWidth = 0.5f;
    public GameObject gridLine;

    // Start is called before the first frame update
    void Start()
    {
		/*foreach (Transform child in transform.GetComponentsInChildren<Transform>()) {
			Destroy(child.gameObject);
		}*/

		GameObject[] horizontalGridLines = new GameObject[gridDensity - 1];
		GameObject[] verticalGridLines = new GameObject[gridDensity - 1];

		for (int i = 0; i < gridDensity - 1; i++) {
			horizontalGridLines[i] = Instantiate(gridLine, transform);
			verticalGridLines[i] = Instantiate(gridLine, transform);
		}

		for (int i = 0; i < horizontalGridLines.Length; i++) {
			RectTransform rect = horizontalGridLines[i].GetComponent<RectTransform>();
			rect.sizeDelta = new Vector2(gridSize, lineWidth);
			rect.anchoredPosition = new Vector2(0, -gridSize / 2 + (gridSize / gridDensity * (i + 1)));
		}

		for (int i = 0; i < verticalGridLines.Length; i++) {
			RectTransform rect = verticalGridLines[i].GetComponent<RectTransform>();
			rect.sizeDelta = new Vector2(lineWidth, gridSize);
			rect.anchoredPosition = new Vector2(-gridSize / 2 + (gridSize / gridDensity * (i + 1)), 0);
		}

	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
