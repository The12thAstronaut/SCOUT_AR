using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using Microsoft.MixedReality.Toolkit.UX;
using System;
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
    public PressableButton targetMarkerButton;
    public PressableButton mapButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenViewer() {
        mapButton.ForceSetToggled(true);

		generalViewer.SetActive(false);
		gameObject.SetActive(true);
		nameInput.text = markerManager.selectedMarker.markerName;
		descriptionInput.text = markerManager.selectedMarker.markerDescription;
		iconPicker.GetComponent<ToggleCollection>().CurrentIndex = Array.IndexOf(markerManager.markerIconNames, markerManager.selectedMarker.currentIconName);

        if (markerManager.selectedMarker == markerManager.targetedMarker) {
            targetMarkerButton.ForceSetToggled(true);
        }
	}

    public void CloseViewer() {
        if (markerManager.selectedMarker.isTargeted) {
			targetMarkerButton.ForceSetToggled(false);
			markerManager.TargetSelected(true);
		}
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
        markerManager.selectedMarker.mapMarker.UpdateMapIcon();
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
		markerManager.mapMarkers[markerManager.selectedMarker.index].GetComponent<TapToPlace>().StartPlacement();
	}
}
