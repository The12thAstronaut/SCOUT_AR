using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using Slider = Microsoft.MixedReality.Toolkit.UX.Slider;

public class VitalInfoCard : MonoBehaviour
{
	/*public float nominalMax { get; set; }
	public float nominalMin { get; set; }
	public float errorMax { get; set; }
	public float errorMin { get; set; }
	public string decimalFormat { get; set; }
	public string vitalName { get; set; }
	public Color errorColor { get; set; }
	public Color goodColor { get; set; }
	public float maxVal { get; set; }
	public float minVal { get; set; }*/

	public SuitVital vitals { get; set; }

	private Material fillMat;
	private Color originalColor;

	private float normalizedValue;

	public Slider slider;
	public TextMeshProUGUI minText;
	public TextMeshProUGUI maxText;
	public TextMeshProUGUI errorText;
	public RectTransform valueRT;
	public TextMeshProUGUI valueText;
	public TextMeshProUGUI nameText;
	

	private float sliderWidth;

	public float value;

	// Start is called before the first frame update
	void Start()
    {
		
	}

	public void Initialize() {
		sliderWidth = slider.gameObject.GetComponent<RectTransform>().sizeDelta.x;

		minText.text = vitals.minVal.ToString(vitals.decimalFormat);
		maxText.text = vitals.maxVal.ToString(vitals.decimalFormat);
		valueText.text = value.ToString(vitals.decimalFormat);
		nameText.text = vitals.name;

		RectTransform errorRT = errorText.gameObject.GetComponent<RectTransform>();
		if (vitals.nominalMax > vitals.errorMax) {
			errorText.text = vitals.errorMax.ToString(vitals.decimalFormat);
			errorRT.localPosition = new Vector3(((vitals.errorMax - vitals.minVal) / (vitals.maxVal - vitals.minVal) - 0.5f) * sliderWidth, errorRT.localPosition.y, errorRT.localPosition.z);

			maxText.color = vitals.vitalsManager.goodColor;
			maxText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = vitals.vitalsManager.goodColor;
			minText.color = vitals.vitalsManager.errorColor;
			minText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = vitals.vitalsManager.errorColor;
		} else {
			errorText.text = vitals.errorMin.ToString(vitals.decimalFormat);
			errorRT.localPosition = new Vector3(((vitals.errorMin - vitals.minVal) / (vitals.maxVal - vitals.minVal) - 0.5f) * sliderWidth, errorRT.localPosition.y, errorRT.localPosition.z);
		}

		valueRT.localPosition = new Vector3(((value - vitals.minVal) / (vitals.maxVal - vitals.minVal) - 0.5f) * sliderWidth, valueRT.localPosition.y, valueRT.localPosition.z);

		fillMat = Instantiate<Material>(slider.gameObject.GetNamedChild("SliderTrack").transform.GetChild(0).GetComponent<RawImage>().material);
		slider.gameObject.GetNamedChild("SliderTrack").transform.GetChild(0).GetComponent<RawImage>().material = fillMat;

		originalColor = fillMat.GetColor("_Color");
		UpdateSlider();
	}

	// Update is called once per frame
	void Update() {
		valueRT.localPosition = new Vector3(((value - vitals.minVal) / (vitals.maxVal - vitals.minVal) - 0.5f) * sliderWidth, valueRT.localPosition.y, valueRT.localPosition.z);
		valueText.text = value.ToString(vitals.decimalFormat);
		UpdateSlider();
	}

	private void ClampVal() {
		if (value > vitals.maxVal) {
			value = vitals.maxVal;
		} else if (value < vitals.minVal) {
			value = vitals.minVal;
		}
	}

	private void UpdateSlider() {
		ClampVal();
		normalizedValue = (value - vitals.minVal) / (vitals.maxVal - vitals.minVal);
		slider.Value = normalizedValue;

		if (vitals.inWarning) {
			fillMat.SetColor("_Color", vitals.vitalsManager.errorColor);
			valueRT.GetComponent<Image>().color = vitals.vitalsManager.errorColor;
		} else {
			fillMat.SetColor("_Color", originalColor);
			valueRT.GetComponent<Image>().color = originalColor;
		}
	}
}
