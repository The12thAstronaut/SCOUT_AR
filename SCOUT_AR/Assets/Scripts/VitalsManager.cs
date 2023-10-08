using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class VitalsManager : MonoBehaviour
{
    public SuitVital[] suitVitals;
	public Color errorColor = new Color(188, 0, 0);
	public Color goodColor = new Color(0, 159, 15);
	public GameObject warningPanel;
	public GameObject vitalsMenu;

	private float[] values;

	private IndicatorManager indicatorManager;

	// Start is called before the first frame update
	void Start() {
		indicatorManager = FindObjectOfType<IndicatorManager>();
		values = new float[suitVitals.Length];

		for (int i = 0; i < suitVitals.Length; i++) {
			suitVitals[i].vitalsManager = this;
			suitVitals[i].indicatorManager = indicatorManager;
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
	public Transform mainBarFullText;
	public string decimalFormat = "0.";
	public bool inWarning = false;

	public int index { get; set; }
	public float slope { get; private set; }
	public float yOffset { get; private set; }
	public float timeToMin { get; private set; }


	public float maxVal => nominalMax > errorMax ? nominalMax : errorMax;
	public float minVal => nominalMin < errorMin ? nominalMin : errorMin;

	public IndicatorManager indicatorManager { get; set; }
	public VitalsManager vitalsManager { get; set; }

	private float[] prevVals = new float[5];
	private float[] prevTimes = new float[5];
	private float[] prevTimesAtMin = new float[5];


	public void SetBounds(Color errorColor, Color goodColor) {
		slider.nominalMax = nominalMax;
		slider.nominalMin = nominalMin;
		slider.errorMax = errorMax;
		slider.errorMin = errorMin;
		slider.errorColor = errorColor;
		slider.goodColor = goodColor;
		slider.minVal = minVal;
		slider.maxVal = maxVal;
		slider.value = value;

		vitalInfoCard.vitals = this;
		vitalInfoCard.Initialize();
	}
	public void SetValue(float newValue) {
		Array.Copy(prevVals, 1, prevVals, 0, prevVals.Length - 1);
		Array.Copy(prevTimes, 1, prevTimes, 0, prevTimes.Length - 1);

		prevVals[prevVals.Length - 1] = newValue;
		prevTimes[prevTimes.Length - 1] = Time.time;

		value = newValue;

		double b = 0;
		double m = 0;

		List<Vector2> points = new List<Vector2>();

		for (int i = 0; i < prevVals.Length; i++) {
			points.Add(new Vector2(prevVals[i], prevTimes[i]));
		}

		FindLinearLeastSquaresFit(points, out m, out b);
		slope = (float)m;
		yOffset = (float)b;

		if (name.Equals("Battery Capacity")) {
			Array.Copy(prevTimesAtMin, 1, prevTimesAtMin, 0, prevTimesAtMin.Length - 1);
			prevTimesAtMin[prevTimesAtMin.Length - 1] = (minVal - yOffset) / slope;

			timeToMin = prevTimesAtMin.Average() - Time.time;
			indicatorManager.UpdateBatteryTimer(timeToMin);
		}

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
		Array.Copy(prevVals, 1, prevVals, 0, prevVals.Length - 1);
		Array.Copy(prevTimes, 1, prevTimes, 0, prevTimes.Length - 1);

		value = minVal + Mathf.PingPong(Time.time * (maxVal - minVal) / 12/*(Random.Range(-8.0f, 8.0f))*/, maxVal - minVal);

		prevVals[prevVals.Length - 1] = value;
		prevTimes[prevTimes.Length - 1] = Time.time;

		double b = 0;
		double m = 0;

		List<Vector2> points = new List<Vector2>();

		for (int i = 0; i < prevVals.Length; i++) {
			points.Add(new Vector2(prevTimes[i], prevVals[i]));
		}

		FindLinearLeastSquaresFit(points, out m, out b);
		slope = (float)m;
		yOffset = (float)b;

		if (name.Equals("Battery Capacity")) {
			Array.Copy(prevTimesAtMin, 1, prevTimesAtMin, 0, prevTimesAtMin.Length - 1);
			prevTimesAtMin[prevTimesAtMin.Length - 1] = (minVal - yOffset) / slope;

			timeToMin = prevTimesAtMin.Average() - Time.time;
			indicatorManager.UpdateBatteryTimer(timeToMin);
		}

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

			foreach (TextMeshProUGUI text in mainBarFullText.GetComponentsInChildren<TextMeshProUGUI>()) {
				text.color = vitalsManager.errorColor;
			}

		} else {
			warningButton.SetActive(false);

			foreach (TextMeshProUGUI text in mainBarFullText.GetComponentsInChildren<TextMeshProUGUI>()) {
				text.color = vitalsManager.goodColor;
			}

		}
	}

	public static double ErrorSquared(List<Vector2> points, double m, double b) {
		double total = 0;
		foreach (Vector2 pt in points) {
			double dy = pt.y - (m * pt.x + b);
			total += dy * dy;
		}
		return total;
	}

	public static double FindLinearLeastSquaresFit(List<Vector2> points, out double m, out double b) {
		// Perform the calculation.
		// Find the values S1, Sx, Sy, Sxx, and Sxy.
		double S1 = points.Count;
		double Sx = 0;
		double Sy = 0;
		double Sxx = 0;
		double Sxy = 0;
		foreach (Vector2 pt in points) {
			Sx += pt.x;
			Sy += pt.y;
			Sxx += pt.x * pt.x;
			Sxy += pt.x * pt.y;
		}

		// Solve for m and b.
		m = (Sxy * S1 - Sx * Sy) / (Sxx * S1 - Sx * Sx);
		b = (Sxy * Sx - Sy * Sxx) / (Sx * Sx - S1 * Sxx);

		return System.Math.Sqrt(ErrorSquared(points, m, b));
	}
}
