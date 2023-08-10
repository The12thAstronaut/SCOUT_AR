using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerSelectorButton : MonoBehaviour
{
    public int markerIndex { get; set; }
    public MarkerManager markerManager { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectMarker() {
        markerManager.markerGroupViewer.CloseViewer();
        markerManager.selectedMarker = markerManager.selectedGroup.mapMarkers[markerIndex].worldMarker;
        markerManager.markerViewer.OpenViewer();
    }
}
