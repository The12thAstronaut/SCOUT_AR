using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ModuleManager// : MonoBehaviour
{
    //public static ModuleManager Instance;

	public enum Preset {
		Basic,
		SUITS_Moon,
		SUITS_Mars,
		Holograms,
		Full_Nav
	}

	private static bool navigationActive = false;
	private static bool vitalsActive = false;
	private static bool telemetryActive = false;
	private static bool proceduresActive = false;
	private static bool logsActive = false;
	private static bool compassActive = false;
	private static bool hologramsActive = false;
	private static bool mapActive = false;
	private static bool diagnosticsActive = false;
	private static bool moonDatasetActive = false;

	/*private void Awake() {

		if (Instance != null) { // Changing scenese
			Destroy(gameObject);
			return;
		}

		
		//Instance = this;
		//DontDestroyOnLoad(gameObject);

		SelectPreset(Preset.NoVitals);
	}*/

	public static void SelectPreset(Preset preset) {
		ClearModules();

		if (preset == Preset.Basic) {
			navigationActive = true;
		} else if (preset == Preset.SUITS_Moon) {
			navigationActive = true;
			proceduresActive = true;
			telemetryActive = true;
			logsActive = true;
			vitalsActive = true;
		}
	}

	public static void SetNavigationActive(bool active) {
		navigationActive = active;
	}
	public static bool GetNavigationActive() {
		return navigationActive;
	}

	public static void SetVitalsActive(bool active) {
		vitalsActive = active;
	}
	public static bool GetVitalsActive() {
		return vitalsActive;
	}

	public static void SetTelemetryActive(bool active) {
		telemetryActive = active;
	}
	public static bool GetTelemetryActive() {
		return telemetryActive;
	}

	public static void SetProceduresActive(bool active) {
		proceduresActive = active;
	}
	public static bool GetProceduresActive() {
		return proceduresActive;
	}

	public static void SetLogsActive(bool active) {
		logsActive = active;
	}
	public static bool GetLogsActive() {
		return logsActive;
	}

	public static void SetCompassActive(bool active) {
		compassActive = active;
	}
	public static bool GetCompassActive() {
		return compassActive;
	}

	public static void SetMapActive(bool active) {
		mapActive = active;
	}
	public static bool GetMapActive() {
		return mapActive;
	}

	public static void SetHologramsActive(bool active) {
		hologramsActive = active;
	}
	public static bool GetHologramsActive() {
		return hologramsActive;
	}

	public static void SetDiagnosticsActive(bool active) {
		diagnosticsActive = active;
	}
	public static bool GetDiagnosticsActive() {
		return diagnosticsActive;
	}

	public static void SetMoonDatasetActive(bool active) {
		moonDatasetActive = active;
	}
	public static bool GetMoonDatasetActive() {
		return moonDatasetActive;
	}

	public static void ClearModules() {
		navigationActive = false;
		vitalsActive = false;
		telemetryActive = false;
		proceduresActive = false;
		logsActive = false;
		compassActive = false;
		mapActive = false;
		hologramsActive = false;
		diagnosticsActive = false;
		moonDatasetActive = false;
	}
}
