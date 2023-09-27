using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteboardManager : MonoBehaviour
{
    public GameObject linePrefab;

    private List<LineRenderer> whiteboardLines;
    private bool isDrawing = false;

    // Start is called before the first frame update
    void Start()
    {
        whiteboardLines = new List<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Draw(Vector3 pos) {
        if (!isDrawing) {
            LineRenderer lineRenderer = GameObject.Instantiate(linePrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<LineRenderer>();
			whiteboardLines.Add(lineRenderer);
            isDrawing = true;
		}
		AddPoint(whiteboardLines.Count - 1, pos);
	}

	public void AddPoint(int index, Vector3 pos) {
		whiteboardLines[index].positionCount = whiteboardLines[index].positionCount + 1;
        whiteboardLines[index].SetPosition(whiteboardLines[index].positionCount - 1, pos);
	}
}
