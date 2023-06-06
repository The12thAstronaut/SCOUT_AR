using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class WaypointManager : MonoBehaviour
{

    public GameObject waypointPrefab;
    public Transform waypointCollection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateAnchor() {
        var instance = Instantiate(waypointPrefab, Camera.main.transform.position, Quaternion.identity, waypointCollection);

        if (instance.GetComponent<ARAnchor>() == null) {
            instance.AddComponent<ARAnchor>();
        }
    }
}
