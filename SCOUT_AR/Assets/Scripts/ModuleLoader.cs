using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleLoader : MonoBehaviour
{
	public List<GameObject> vitalsObjects = new List<GameObject>();
    public List<GameObject> navigationObjects = new List<GameObject>();
	public List<GameObject> telemetryObjects = new List<GameObject>();
	public List<GameObject> procedureObjects = new List<GameObject>();
	public List<GameObject> logObjects = new List<GameObject>();

	// Start is called before the first frame update
	void Start()
    {
        LoadModules();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadModules() {

        foreach (GameObject obj in vitalsObjects) {
            obj.SetActive(ModuleManager.GetVitalsActive());
        }

		foreach (GameObject obj in navigationObjects) {
			obj.SetActive(ModuleManager.GetNavigationActive());
		}

		foreach (GameObject obj in telemetryObjects) {
			obj.SetActive(ModuleManager.GetTelemetryActive());
		}

		foreach (GameObject obj in procedureObjects) {
			obj.SetActive(ModuleManager.GetProceduresActive());
		}

		foreach (GameObject obj in logObjects) {
			obj.SetActive(ModuleManager.GetLogsActive());
		}
	}
}
