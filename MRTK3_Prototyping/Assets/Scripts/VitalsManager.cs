using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VitalsManager : MonoBehaviour
{
    public SuitVital[] suitVitals;
	public Color errorColor = new Color(188, 0, 0);
	public Color goodColor = new Color(0, 159, 15);
	public GameObject warningPanel;
	public GameObject vitalsMenu;

	private float[] values;

	// Start is called before the first frame update
	void Start()
    {
		values = new float[suitVitals.Length];

		for (int i = 0; i < suitVitals.Length; i++) {
			suitVitals[i].vitalsManager = this;
			suitVitals[i].index = i;
			suitVitals[i].SetBounds(errorColor, goodColor);
			//suitVitals[i].SetValue();
        }
    }

    // Update is called once per frame
    void Update()
    {
		bool warningActive = false;
		foreach (SuitVital suitVital in suitVitals) {
			suitVital.SetValue();
			if (suitVital.inWarning) warningActive = true;
		}

		if (warningActive && !vitalsMenu.activeSelf) {
			warningPanel.SetActive(true);
		} else {
			warningPanel.SetActive(false);
		}
	}

}

[System.Serializable]
public class SuitVital {
	public string name;
	public float nominalMax;
	public float nominalMin;
	public float errorMax;
	public float errorMin;
	public float value;
    public TextMeshProUGUI vitalsLabel;
    public VitalsSlider slider;
    public VitalInfoCard vitalInfoCard;
	public GameObject warningButton;
	public string decimalFormat = "0.";
	public bool inWarning = false;

	public int index { get; set; }

	public float maxVal => nominalMax > errorMax ? nominalMax : errorMax;
	public float minVal => nominalMin < errorMin ? nominalMin : errorMin;

	public VitalsManager vitalsManager { get; set; }

	public void SetBounds(Color errorColor, Color goodColor) {
		slider.nominalMax = nominalMax;
		slider.nominalMin = nominalMin;
		slider.errorMax = errorMax;
		slider.errorMin = errorMin;
		slider.errorColor = errorColor;
		slider.minVal = minVal;
		slider.maxVal = maxVal;
		slider.value = value;

		vitalInfoCard.vitals = this;
		vitalInfoCard.Initialize();
	}

    public void SetValue(float newValue) {
		value = newValue;

		if (nominalMax > errorMax && value <= errorMax) {
			inWarning = true;
		} else if (nominalMax < errorMax && value >= errorMin) {
			inWarning = true;
		} else {
			inWarning = false;
		}

		vitalsLabel.text = value.ToString(decimalFormat);
        slider.value = value;
        vitalInfoCard.value = value;

		if (inWarning) {
			warningButton.SetActive(true);
		} else {
			warningButton.SetActive(false);
		}
    }

	public void SetValue() {
		value = minVal + Mathf.PingPong(Time.time * (maxVal - minVal) / 12/*(Random.Range(-8.0f, 8.0f))*/, maxVal - minVal);

		if (nominalMax > errorMax && value <= errorMax) {
			inWarning = true;
		} else if (nominalMax < errorMax && value >= errorMin) {
			inWarning = true;
		} else {
			inWarning = false;
		}

		vitalsLabel.text = value.ToString(decimalFormat);
		slider.value = value;
		vitalInfoCard.value = value;

		if (inWarning) {
			warningButton.SetActive(true);
		} else {
			warningButton.SetActive(false);
		}
	}
}
