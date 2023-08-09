using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;

public class MarkerGroupViewer : MonoBehaviour {

	public MarkerManager markerManager;
	public GameObject generalViewer;
	public VirtualizedScrollRectList markerScrollList;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OpenViewer() {
		generalViewer.SetActive(false);
		gameObject.SetActive(true);
		markerScrollList.SetItemCount(markerManager.selectedGroup.mapMarkers.Count);
	}

	public void CloseViewer() {
		gameObject.SetActive(false);
		generalViewer.SetActive(true);
	}
}
