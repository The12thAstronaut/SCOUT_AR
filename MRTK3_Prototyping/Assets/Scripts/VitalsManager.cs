using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VitalsManager : MonoBehaviour
{
    public SuitVital[] suitVitals;
	public Color errorColor = new Color(188, 0, 0);
	public Color goodColor = new Color(0, 159, 15);

	// Start is called before the first frame update
	void Start()
    {
        foreach (SuitVital suitVital in suitVitals) {
            suitVital.SetBounds(errorColor, goodColor);
            suitVital.SetValue();
        }
    }

    // Update is called once per frame
    void Update()
    {
		foreach (SuitVital suitVital in suitVitals) {
			suitVital.SetValue();
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
	public string decimalFormat = "0.";

	private float maxVal => nominalMax > errorMax ? nominalMax : errorMax;
	private float minVal => nominalMin < errorMin ? nominalMin : errorMin;

	public void SetBounds(Color errorColor, Color goodColor) {
		slider.nominalMax = nominalMax;
		slider.nominalMin = nominalMin;
		slider.errorMax = errorMax;
		slider.errorMin = errorMin;
		slider.errorColor = errorColor;
		slider.minVal = minVal;
		slider.maxVal = maxVal;
		slider.value = value;

		vitalInfoCard.nominalMax = nominalMax;
		vitalInfoCard.nominalMin = nominalMin;
		vitalInfoCard.errorMax = errorMax;
		vitalInfoCard.errorMin = errorMin;
		vitalInfoCard.vitalName = name;
		vitalInfoCard.decimalFormat = decimalFormat;
		vitalInfoCard.goodColor = goodColor;
		vitalInfoCard.errorColor = errorColor;
		vitalInfoCard.value = value;
		vitalInfoCard.minVal = minVal;
		vitalInfoCard.maxVal = maxVal;
		vitalInfoCard.Initialize();
	}

    public void SetValue() {
		value = minVal + Mathf.PingPong(Time.time * (maxVal - minVal) / 4, maxVal - minVal);

		vitalsLabel.text = value.ToString(decimalFormat);
        slider.value = value;
        vitalInfoCard.value = value;
    }
}
