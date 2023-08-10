using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;
using TMPro;

public class MarkerGroupViewer : MonoBehaviour {

	public MarkerManager markerManager;
	public GameObject generalViewer;

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
		Debug.Log(markerManager.selectedGroup.mapMarkers.Count);
		Debug.Log(markerManager.markerGroupScrollList.ItemCount);
	}

	public void CloseViewer() {
		gameObject.SetActive(false);
		generalViewer.SetActive(true);
	}
}
