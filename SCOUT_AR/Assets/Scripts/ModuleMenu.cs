using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.SceneManagement;

public class ModuleMenu : MonoBehaviour
{
	public GameObject PresetList;
	public GameObject ModuleList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Load() {
		SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
	}

	public void Clear() {

		foreach (PressableButton btn in PresetList.GetComponentsInChildren<PressableButton>()) {
			btn.ForceSetToggled(false);
		}

		DeselectModules();

		ModuleManager.ClearModules();
	}

    public void SelectMoonPreset() {

		DeselectModules();
		ModuleManager.SelectPreset(ModuleManager.Preset.SUITS_Moon);
		SelectActiveModules();
	}

	public void SelectMarsPreset() {

		DeselectModules();
		ModuleManager.SelectPreset(ModuleManager.Preset.SUITS_Mars);
		SelectActiveModules();
	}

	public void SelectHologramsPreset() {

		DeselectModules();
		ModuleManager.SelectPreset(ModuleManager.Preset.Holograms);
		SelectActiveModules();
	}

	public void SelectFullNavPreset() {

		DeselectModules();
		ModuleManager.SelectPreset(ModuleManager.Preset.Full_Nav);
		SelectActiveModules();
	}

	public void SelectBasicPreset() {

		DeselectModules();
		ModuleManager.SelectPreset(ModuleManager.Preset.Basic);
		SelectActiveModules();
	}

	public void SetModule(string name) {

		foreach (PressableButton btn in PresetList.GetComponentsInChildren<PressableButton>()) {
			btn.ForceSetToggled(false);
		}

		if (name.Equals("Navigation")) {
			ModuleManager.SetNavigationActive(!ModuleManager.GetNavigationActive());
		} else if (name.Equals("Vitals")) {
			ModuleManager.SetVitalsActive(!ModuleManager.GetVitalsActive());
		} else if (name.Equals("Telemetry")) {
			ModuleManager.SetTelemetryActive(!ModuleManager.GetTelemetryActive());
		} else if (name.Equals("Procedures")) {
			ModuleManager.SetProceduresActive(!ModuleManager.GetProceduresActive());
		} else if (name.Equals("Logs")) {
			ModuleManager.SetLogsActive(!ModuleManager.GetLogsActive());
		} else if (name.Equals("Compass")) {
			ModuleManager.SetCompassActive(!ModuleManager.GetCompassActive());
		} else if (name.Equals("Holograms")) {
			ModuleManager.SetHologramsActive(!ModuleManager.GetHologramsActive());
		} else if (name.Equals("Map")) {
			ModuleManager.SetMapActive(!ModuleManager.GetMapActive());
		} else if (name.Equals("Diagnostics")) {
			ModuleManager.SetDiagnosticsActive(!ModuleManager.GetDiagnosticsActive());
		} else if (name.Equals("Moon Dataset")) {
			ModuleManager.SetMoonDatasetActive(!ModuleManager.GetMoonDatasetActive());
		}
	}

	private void DeselectModules() {
		foreach (PressableButton btn in ModuleList.GetComponentsInChildren<PressableButton>()) {
			btn.ForceSetToggled(false);
		}
	}

	private void SelectActiveModules() {
		foreach (PressableButton btn in ModuleList.GetComponentsInChildren<PressableButton>()) {
			name = btn.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;
			if (name.Equals("Navigation")) {
				btn.ForceSetToggled(ModuleManager.GetNavigationActive());
			} else if (name.Equals("Vitals")) {
				btn.ForceSetToggled(ModuleManager.GetVitalsActive());
			} else if (name.Equals("Telemetry")) {
				btn.ForceSetToggled(ModuleManager.GetTelemetryActive());
			} else if (name.Equals("Procedures")) {
				btn.ForceSetToggled(ModuleManager.GetProceduresActive());
			} else if (name.Equals("Logs")) {
				btn.ForceSetToggled(ModuleManager.GetLogsActive());
			} else if (name.Equals("Compass")) {
				btn.ForceSetToggled(ModuleManager.GetCompassActive());
			} else if (name.Equals("Holograms")) {
				btn.ForceSetToggled(ModuleManager.GetHologramsActive());
			} else if (name.Equals("Map")) {
				btn.ForceSetToggled(ModuleManager.GetMapActive());
			} else if (name.Equals("Diagnostics")) {
				btn.ForceSetToggled(ModuleManager.GetDiagnosticsActive());
			} else if (name.Equals("Moon Dataset")) {
				btn.ForceSetToggled(ModuleManager.GetMoonDatasetActive());
			}
		}
	}
}
