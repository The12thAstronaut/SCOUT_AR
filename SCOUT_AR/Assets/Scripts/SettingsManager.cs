using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public Setting[] settings;

    public Follow[] mainBar;
    public PositionMenu[] menus;
	public FuzzyGazeInteractor gazeInteractor;

	public Slider[] sliders;

	private float startTime;
	private bool enable = true;
	private bool beta = false;

	// Start is called before the first frame update
	void Start()
    {
		startTime = Time.time;

		for (int i = 0; i < sliders.Length; i++) {
			sliders[i].Value = settings[i].GetNormalizedValue();
			sliders[i].transform.GetComponent<CanvasSliderVisuals>().enabled = false;
			sliders[i].transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text = settings[i].settingName;
			sliders[i].transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().text = settings[i].value.ToString("0.##");
		}

	}

    // Update is called once per frame
    void Update()
    {
		if (!enable && Time.time - startTime >= .01) {
			foreach (Slider slider in sliders) {
				slider.transform.GetComponent<CanvasSliderVisuals>().enabled = true;
			}

			enable = true;
			beta = true;
		}
	}

	public void UpdateSetting(string setting) {
		if (!beta) {
			return;
		}

		for (int i = 0; i < settings.Length; i++) {
			if (settings[i].settingName == setting) {
				settings[i].value = sliders[i].Value * (settings[i].maxValue - settings[i].minValue) + settings[i].minValue;
				sliders[i].transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().text = settings[i].value.ToString("0.##");
			}
		}

		UpdateGameSettings();
	}

	public void UpdateGameSettings() {
		foreach (PositionMenu obj in menus) {
			obj.distance = settings[0].value;

			float angle = Mathf.Atan(0.5f / 0.5f);
			float newHalfSize = Mathf.Tan(angle) * settings[0].value;

			//obj.transform.localScale = new Vector3(settings[2].value, settings[2].value, 1);
			obj.transform.localScale = new Vector3(newHalfSize * 2, newHalfSize * 2, newHalfSize * 2);

			obj.ChangeDistance();
			obj.UpdateBackplates();
		}

		foreach (Follow obj in mainBar) {
            obj.DefaultDistance = settings[1].value;
            obj.MinDistance = settings[1].value;
            obj.MaxDistance = settings[1].value + 0.01f;
			//obj.transform.localScale = new Vector3(settings[2].value / 1000, settings[2].value / 1000, .001f);
			// scaling for waypoint creator needs fix
		}

		gazeInteractor.hoverTimeToSelect = settings[3].value;
    }

	public void ActivateSliders() {
		enable = false;
		startTime = Time.time;
	}
}

[System.Serializable]
public class Setting {
    public string settingName;
    public float maxValue;
    public float minValue;
    public float value;

    public float GetNormalizedValue() {
        return (value - minValue) / (maxValue - minValue);
	}
}