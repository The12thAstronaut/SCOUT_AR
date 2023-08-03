using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MarkerViewer : MonoBehaviour
{
    public MarkerManager markerManager;
    public TMP_InputField nameInput;
    public TMP_InputField descriptionInput;
    public GameObject iconPicker;
    public GameObject generalViewer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInfo() {
        nameInput.text = markerManager.selectedMarker.markerName;
        descriptionInput.text = markerManager.selectedMarker.markerDescription;
    }

    public void OpenViewer() {
		generalViewer.SetActive(false);
		gameObject.SetActive(true);
    }

    public void CloseViewer() {
        gameObject.SetActive(false);
		generalViewer.SetActive(true);
	}

    public void RemoveMarker() {
        markerManager.RemoveMarker(markerManager.selectedMarker.index);
        CloseViewer();
    }

    public void SetIcon(FontIconSelector fontIcon) {
        markerManager.selectedMarker.currentIconName = fontIcon.CurrentIconName;
        markerManager.selectedMarker.UpdateInfo();
    }

    public void SetName() {
		markerManager.selectedMarker.markerName = nameInput.text;
		markerManager.selectedMarker.UpdateInfo();
	}

    public void SetDescription () {
        markerManager.selectedMarker.markerDescription = descriptionInput.text;
        markerManager.selectedMarker.UpdateInfo();
    }

    public void MoveMarker() {
        markerManager.mapMarkers[markerManager.selectedMarker.index].SetBeingPlaced(true);
		markerManager.mapMarkers[markerManager.selectedMarker.index].GetComponent<TapToPlace>().StartPlacement();
	}
}
